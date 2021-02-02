using System;
using MoveLikeJogger.DataMining.Common;
using MoveLikeJogger.DataMining.DB;

namespace MoveLikeJogger.DataMining.Commands.Identity
{
    public interface IDeleteUserCommand
    {
        bool Execute(string id);
        bool Execute(string id, bool isDeleted);
    }

    public class DeleteUserCommand : CommandQueryBase, ICommand<bool, string>, ICommand<bool, string, bool>, IDeleteUserCommand
    {
        public DeleteUserCommand(IApplicationDbContext context) : base(context)
        {
            
        }

        public bool Execute(string id)
        {
            return Execute(id, true);
        }

        public bool Execute(string id, bool isDeleted)
        {
            try
            {
                var user = Db.Users.Find(id);

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
