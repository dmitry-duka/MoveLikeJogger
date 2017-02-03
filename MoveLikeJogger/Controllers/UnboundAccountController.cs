using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MoveLikeJogger.DataContracts.Identity;
using MoveLikeJogger.DataModels.Identity;

namespace MoveLikeJogger.Controllers
{
    [Authorize]
    public class UnboundAccountController : ODataControllerBase
    {
        [HttpGet]
        [ODataRoute("identity")]
        public IHttpActionResult Identity()
        {
            var authenticated = User.Identity.IsAuthenticated;

            if (!authenticated)
            {
                return Unauthorized();
            }

            var user = GetCurrentUser();

            if (user == null || user.IsDeleted)
            {
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

                return Unauthorized();
            }

            return Ok(new IdentityDTO
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = GetUserRole(User)
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [ODataRoute("login")]
        public IHttpActionResult Login(ODataActionParameters parameters)
        {
            var model = parameters["data"] as LoginDTO;

            if (model == null || string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest();
            }

            var userDeleted = UserManager.Users.Where(x => x.UserName.Equals(model.UserName, StringComparison.InvariantCultureIgnoreCase)).Select(x => (bool?)x.IsDeleted).FirstOrDefault();

            if (userDeleted.HasValue && userDeleted.Value)
            {
                return BadRequest();
            }

            var result = SignInManager.PasswordSignIn(model.UserName, model.Password, model.RememberMe, shouldLockout: false);

            return OkOrBadRequest(result == SignInStatus.Success);
        }
        
        [HttpPost]
        [ODataRoute("logout")]
        public IHttpActionResult LogOut()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        [ODataRoute("register")]
        public IHttpActionResult Register(ODataActionParameters parameters)
        {
            var model = parameters["data"] as LoginRegisterDTO;

            if (model == null || string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest();
            }

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email
            };

            var result = UserManager.Create(user, model.Password);

            if (result.Succeeded)
            {
                SignInManager.SignIn(user, isPersistent: model.RememberMe, rememberBrowser: false);
            }

            return OkOrBadRequest(result);
        }

        [HttpPut]
        [ODataRoute("account")]
        public IHttpActionResult Account(ODataActionParameters parameters)
        {
            var model = parameters["data"] as LoginUpdateDTO;

            if (model == null || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Current password required.");
            }

            var user = GetCurrentUser();

            if (user == null)
            {
                return BadRequest("User not found!");
            }

            var pwdVerified = UserManager.CheckPassword(user, model.Password);

            if (!pwdVerified)
            {
                return BadRequest("You specified an incorrect current password!");
            }

            var userUpdateData = new UserUpdateData
            {
                Email = NullIfEmpty(model.Email),
                Password = NullIfEmpty(model.NewPassword)
            };

            var result = UpdateUser(user, userUpdateData);

            if (result.Succeeded)
            {
                SignInManager.SignIn(user, isPersistent: model.RememberMe, rememberBrowser: false);
            }

            return OkOrBadRequest(result);
        }
    }
}