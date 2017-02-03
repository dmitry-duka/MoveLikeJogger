using System;
using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using MoveLikeJogger.DataContracts.Extensions;
using MoveLikeJogger.DataContracts.Moves;
using MoveLikeJogger.DataContracts.Utils;
using MoveLikeJogger.DataMining.Commands;
using MoveLikeJogger.DataMining.Commands.Moves;
using MoveLikeJogger.DataMining.Queries;
using MoveLikeJogger.DataModels.Moves;

namespace MoveLikeJogger.Controllers
{
    [Authorize]
    public class MovesController : ODataControllerBase
    {
        private readonly IQuery<IQueryable<MoveDTO>, bool> _movesQuery;
        private readonly ICommand<bool, Move> _saveMoveCommand;

        public MovesController(IQuery<IQueryable<MoveDTO>, bool> movesQuery, ICommand<bool, Move> saveMoveCommand)
        {
            _movesQuery = movesQuery;
            _saveMoveCommand = saveMoveCommand;
        }

        [EnableQuery]
        public IQueryable<MoveDTO> Get(bool includeDeleted = false)
        {
            return GetMovesSecure(includeDeleted);
        }

        public IHttpActionResult Get([FromODataUri] int key)
        {
            var move = GetMovesSecure(true).FirstOrDefault(x => x.Id == key);

            return OkOrNotFound(move);
        }

        public IHttpActionResult Post([FromBody] MoveDTO data)
        {
            return SaveOrUpdate(new int(), data);
        }

        public IHttpActionResult Put([FromODataUri] int key, [FromBody] MoveDTO data)
        {
            return key == new int()
                ? BadRequest("Move ID required.")
                : SaveOrUpdate(key, data);
        }

        private IHttpActionResult SaveOrUpdate(int key, MoveDTO data)
        {
            if (data == null)
            {
                return BadRequest("Move data is empty!");
            }

            if (!UserHasAccessToMove(data.UserId))
            {
                return BadRequest("Not enough rights.");
            }

            var validation = data.Validate();

            if (!validation.Key)
            {
                return BadRequest(string.Join(" ", validation.Value));
            }

            var move = new Move
            {
                Id = key,
                UserId = data.UserId,
                Date = data.Date,
                Distance = data.Distance,
                Duration = data.Duration
            };

            var result = _saveMoveCommand.Execute(move);


            return OkOrBadRequest(result);
        }
        
        public IHttpActionResult Delete([FromODataUri] int key, bool restore = false)
        {
            if (key == new int())
            {
                return BadRequest("Move ID required.");
            }

            var move = _movesQuery.Execute(true).FirstOrDefault(x => x.Id == key);

            if (move == null)
            {
                return NotFound();
            }

            if (!UserHasAccessToMove(move.UserId))
            {
                return BadRequest("Not enough rights.");
            }

            var deleteResult = new DeleteMoveCommand().Execute(key, !restore);
            
            return OkOrNotFound(deleteResult);
        }

        [ODataRoute("statistics")]
        public IHttpActionResult GetStatistics(string userId = null, int daysBeforeToday = 7)
        {
            var key = string.IsNullOrWhiteSpace(userId) ? CurrentUserId : userId;
            var endDate = DateTime.Today.AddDays(1); // today
            var startDate = endDate.AddDays(-daysBeforeToday).Date;

            if (!UserHasAccessToMove(key))
            {
                return BadRequest("Not enough rights.");
            }

            var data = _movesQuery.Execute(false)
                .Where(x => x.UserId == key && x.Date >= startDate && x.Date < endDate)
                .ToArray();

            if (data.Length <= 0)
            {
                return NotFound();
            }

            var dataByDays = data.GroupBy(x => x.Date.Date).ToArray();

            double avgSpeedTotal = 0.0f;
            var maxSpeed = 0.0f;
            var maxSpeedDate = new DateTime();
            var maxDistance = 0;
            var maxDistanceDate = new DateTime();
            var maxDuration = 0;
            var maxDurationDate = new DateTime();
            long totalDistance = 0;
            long totalDuration = 0;

            foreach (var dayData in dataByDays)
            {
                var dayDistance = 0;
                var dayDuration = 0;

                foreach (var m in dayData)
                {
                    dayDistance += m.Distance;
                    dayDuration += m.Duration;

                    var speed = MoveUtility.CalculateSpeedKmh(m.Distance, m.Duration);

                    if (speed > maxSpeed)
                    {
                        maxSpeed = speed;
                        maxSpeedDate = m.Date;
                    }

                    avgSpeedTotal += speed;

                    totalDistance += m.Distance;
                    totalDuration += m.Duration;
                }

                if (dayDistance > maxDistance)
                {
                    maxDistance = dayDistance;
                    maxDistanceDate = dayData.Key.Date;
                }

                if (dayDuration > maxDuration)
                {
                    maxDuration = dayDuration;
                    maxDurationDate = dayData.Key.Date;
                }
            }
            
            var statistics = new MoveStatisticsDTO
            {
                DaysTotal = daysBeforeToday,
                DaysActive = dataByDays.Length,

                TotalDistance = totalDistance,
                TotalDuration = totalDuration,

                MaxDistance = maxDistance,
                MaxDuration = maxDuration,
                MaxSpeed = maxSpeed,

                MaxDistanceDate = maxDistanceDate,
                MaxDurationDate = maxDurationDate,
                MaxSpeedDate = maxSpeedDate,

                AvgDistance = (int)(totalDistance / data.Length),
                AvgDuration = (int)(totalDuration / data.Length),
                AvgSpeed = (float) (avgSpeedTotal / data.Length),
            };

            return Ok(statistics);
        }

        #region Protected helpers

        protected bool UserHasAccessToMove(string moveUserId)
        {
            return (!string.IsNullOrWhiteSpace(moveUserId) && moveUserId == CurrentUserId) || CurrentUserIsAdmin;
        }

        protected IQueryable<MoveDTO> GetMovesSecure(bool includeDeleted)
        {
            var query = _movesQuery.Execute(includeDeleted);

            if (!CurrentUserIsAdmin)
            {
                query = query.Where(x => x.UserId == CurrentUserId);
            }

            return query;
        }

        #endregion
    }
}