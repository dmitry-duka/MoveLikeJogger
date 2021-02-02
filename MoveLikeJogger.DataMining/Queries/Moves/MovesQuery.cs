using System.Linq;
using MoveLikeJogger.DataContracts.Moves;
using MoveLikeJogger.DataMining.Common;
using MoveLikeJogger.DataMining.DB;

namespace MoveLikeJogger.DataMining.Queries.Moves
{
    public class MovesQuery : CommandQueryBase, IQuery<IQueryable<MoveDTO>, bool>
    {
        public IQueryable<MoveDTO> Execute(bool includeDeleted)
        {
            return Db.Moves.Where(x => includeDeleted || !x.IsDeleted)
                     .Select(x => new MoveDTO
                     {
                         IsDeleted = x.IsDeleted,
                         Id = x.Id,
                         Date = x.Date,
                         Distance = x.Distance,
                         Duration = x.Duration,
                         UserId = x.UserId
                     });
        }

        public MovesQuery(IApplicationDbContext context) : base(context)
        {
        }
    }
}
