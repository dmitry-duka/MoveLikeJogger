using System.Data.Entity;
using System.Linq;
using MoveLikeJogger.DataMining.Common;
using MoveLikeJogger.DataMining.DB;
using MoveLikeJogger.DataModels.Moves;

namespace MoveLikeJogger.DataMining.Commands.Moves
{
    public class SaveMoveCommand : CommandQueryBase, ICommand<bool, Move>
    {
        public bool Execute(Move payload)
        {
            try
            {
                var isNewEntry = payload.Id == new int();

                var mergeTargets =
                    Db.Moves.Where(
                        x => x.Id != payload.Id &&
                            x.UserId == payload.UserId && !x.IsDeleted &&
                            DbFunctions.DiffMinutes(x.Date, payload.Date) == 0);

                if (mergeTargets.Any())
                {
                    foreach (var target in mergeTargets)
                    {
                        payload.Distance += target.Distance;
                        payload.Duration += target.Duration;
                    }

                    Db.Moves.RemoveRange(mergeTargets);
                }

                if (isNewEntry)
                {
                    Db.Moves.Add(payload);
                }
                else
                {
                    Db.Entry(payload).State = EntityState.Modified;
                }
                
                Db.SaveChanges();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public SaveMoveCommand(IApplicationDbContext context) : base(context)
        {
        }
    }
}
