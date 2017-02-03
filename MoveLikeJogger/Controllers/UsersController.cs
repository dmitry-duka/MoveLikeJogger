using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.OData;
using Microsoft.AspNet.Identity;
using MoveLikeJogger.DataContracts.Identity;
using MoveLikeJogger.DataMining.Commands.Identity;
using MoveLikeJogger.DataMining.Queries;
using MoveLikeJogger.DataModels.Identity;
using MoveLikeJogger.Security;

namespace MoveLikeJogger.Controllers
{
    [Authorize(Roles = IdentityRoles.WhoCanManage.Users)]
    public class UsersController : ODataControllerBase
    {
        private readonly IQuery<IQueryable<ApplicationUserDTO>, bool> _userQuery;

        public UsersController(IQuery<IQueryable<ApplicationUserDTO>, bool> userQuery)
        {
            _userQuery = userQuery;
        }

        [EnableQuery]
        public IQueryable<ApplicationUserDTO> Get(bool includeDeleted = false)
        {
            return _userQuery.Execute(includeDeleted);
        }

        public IHttpActionResult Get([FromODataUri] string key)
        {
            var user = _userQuery.Execute(true).SingleOrDefault(x => x.Id == key);

            return OkOrNotFound(user);
        }

        public IHttpActionResult Post([FromBody] ApplicationUserUpdateDTO data)
        {
            if (data == null)
            {
                return BadRequest("New user data required.");
            }

            if (!string.IsNullOrWhiteSpace(data.Role) && !IdentityRoles.Validate(data.Role))
            {
                return BadRequest($"{data.Role} is not a valid role!");
            }

            var user = new ApplicationUser
            {
                UserName = data.UserName,
                Email = data.Email
            };

            var userCreation = UserManager.Create(user, data.Password);

            if (!string.IsNullOrWhiteSpace(data.Role) && userCreation.Succeeded)
            {
                UserManager.AddToRole(user.Id, data.Role);
            }

            return OkOrBadRequest(userCreation);
        }

        public IHttpActionResult Put([FromODataUri] string key, [FromBody] ApplicationUserUpdateDTO data)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return BadRequest("User ID required.");
            }

            if (data == null)
            {
                return BadRequest("User data required.");
            }

            var user = GetUser(key);

            if (user == null)
            {
                return NotFound();
            }

            var result = UpdateUser(user, new UserUpdateData
            {
                UserName = NullIfEmpty(data.UserName),
                Email = NullIfEmpty(data.Email),
                Password = NullIfEmpty(data.Password),
                Role = data.Role
            });

            return OkOrBadRequest(result);
        }

        public IHttpActionResult Delete([FromODataUri] string key, bool restore = false)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return BadRequest("User ID required.");
            }

            if (key == User.Identity.GetUserId())
            {
                return Conflict();
            }

            var deleteResult = new DeleteUserCommand().Execute(key, !restore);
            
            return OkOrNotFound(deleteResult);
        }
    }
}