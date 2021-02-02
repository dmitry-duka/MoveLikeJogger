using System;
using MoveLikeJogger.DataMining.Common;
using MoveLikeJogger.DataMining.DB;

namespace MoveLikeJogger.DataMining.Commands.Moves
{
    public interface IDeleteMoveCommand
    {
        bool Execute(int id);
        bool Execute(int id, bool isDeleted);
    }

    public class DeleteMoveCommand : CommandQueryBase, ICommand<bool, int>, ICommand<bool, int, bool>, IDeleteMoveCommand
    {
        public DeleteMoveCommand(IApplicationDbContext context) : base(context)
        {
            
        }

        public bool Execute(int id)
        {
            return Execute(id, true);
        }

        public bool Execute(int id, bool isDeleted)
        {
            try
            {
                var user = Db.Moves.Find(id);

                if (user == null)
                {
                    return false;
                }

                user.IsDeleted = isDeleted;

                Db.SaveChanges();
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
