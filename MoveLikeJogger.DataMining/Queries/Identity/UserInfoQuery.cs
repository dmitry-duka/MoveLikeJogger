using System.Linq;
using MoveLikeJogger.DataContracts.Identity;
using MoveLikeJogger.DataMining.Common;
using MoveLikeJogger.DataMining.DB;

namespace MoveLikeJogger.DataMining.Queries.Identity
{
    public class UserInfoQuery : CommandQueryBase, IQuery<IQueryable<ApplicationUserDTO>, bool>
    {
        public IQueryable<ApplicationUserDTO> Execute(bool includeDeleted)
        {
            return Db.Users.Where(x => includeDeleted || !x.IsDeleted)
                     .Select(x => new ApplicationUserDTO
                     {
                         IsDeleted = x.IsDeleted,
                         Id = x.Id,
                         UserName = x.UserName,
                         Email = x.Email,
                         Role = Db.Roles.Where(r => x.Roles.Any(ur => ur.RoleId == r.Id)).Select(r => r.Name).FirstOrDefault()
                     });
        }

        public UserInfoQuery(IApplicationDbContext context) : base(context)
        {
        }
    }
}
