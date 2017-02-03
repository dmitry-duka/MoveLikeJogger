
using System.Linq;

namespace MoveLikeJogger.Security
{
    public static class IdentityRoles
    {
        public const string Admin = "Admin";

        public const string Manager = "Manager";

        //public const string User = "User";

        public static class WhoCanManage
        {
            public const string Users = Admin + "," + Manager;
        }

        public static string[] GetAll()
        {
            return new[] {Admin, Manager};
        }

        public static bool Validate(string role)
        {
            return GetAll().Any(x => x == role);
        }
    }
}