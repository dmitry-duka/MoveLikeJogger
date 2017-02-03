using System;
using MoveLikeJogger.DataMining.Common;

namespace MoveLikeJogger.DataMining.Commands.Identity
{
    public class DeleteUserCommand : CommandQueryBase, ICommand<bool, string>, ICommand<bool, string, bool>
    {
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
