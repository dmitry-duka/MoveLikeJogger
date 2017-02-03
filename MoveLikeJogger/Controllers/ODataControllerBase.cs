using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.OData;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using MoveLikeJogger.DataModels.Identity;
using MoveLikeJogger.Security;

namespace MoveLikeJogger.Controllers
{
    public abstract class ODataControllerBase : ODataController
    {
        #region Auth Media
        private static IOwinContext OwinContext => HttpContext.Current.GetOwinContext();

        protected IAuthenticationManager AuthenticationManager => OwinContext.Authentication;

        protected ApplicationSignInManager SignInManager => OwinContext.Get<ApplicationSignInManager>();

        protected ApplicationUserManager UserManager => OwinContext.GetUserManager<ApplicationUserManager>();

        #endregion
        
        #region Protected helpers

        #region IHttpActionResult helpers

        protected internal IHttpActionResult OkOrNotFound<T>(T model)
            where T : class
        {
            return null == model ? (IHttpActionResult)NotFound() : Ok(model);
        }

        protected internal IHttpActionResult OkOrNotFound(bool success)
        {
            return success ? Ok() : (IHttpActionResult)NotFound();
        }

        protected internal IHttpActionResult OkOrBadRequest(IdentityResult result)
        {
            return result.Succeeded ? Ok() : (IHttpActionResult)BadRequest(string.Join(" ", result.Errors));
        }

        protected internal IHttpActionResult OkOrBadRequest<T>(T model)
            where T : class
        {
            return null == model ? (IHttpActionResult)BadRequest() : Ok(model);
        }

        protected internal IHttpActionResult OkOrBadRequest(bool success)
        {
            return success ? Ok() : (IHttpActionResult)BadRequest();
        }

        #endregion

        protected string NullIfEmpty(string value)
        {
            return string.IsNullOrEmpty(value) ? null : value;
        }

        protected string CurrentUserRole => GetUserRole(User);

        protected bool CurrentUserIsAdmin => User.IsInRole(IdentityRoles.Admin);

        protected string CurrentUserId => User.Identity.GetUserId();

        protected ApplicationUser GetUser(string id)
        {
            return UserManager.FindById(id);
        }

        protected ApplicationUser GetCurrentUser()
        {
            return GetUser(CurrentUserId);
        }
        
        protected static string GetUserRole(IPrincipal user)
        {
            return user.IsInRole(IdentityRoles.Admin)
                ? IdentityRoles.Admin
                : user.IsInRole(IdentityRoles.Manager)
                    ? IdentityRoles.Manager
                    : string.Empty;
        }

        protected class UserUpdateData
        {
            public string UserName { get; set; }

            public string Password { get; set; }

            public string Email { get; set; }

            public string Role { get; set; }
        }

        protected IdentityResult UpdateUser(ApplicationUser user, UserUpdateData data)
        {
            if (!string.IsNullOrWhiteSpace(data.Password))
            {
                var pwdValid = UserManager.PasswordValidator.ValidateAsync(data.Password).Result;

                if (!pwdValid.Succeeded)
                {
                    return pwdValid;
                }

                user.PasswordHash = UserManager.PasswordHasher.HashPassword(data.Password);
            }

            if (!string.IsNullOrWhiteSpace(data.UserName))
            {
                user.UserName = data.UserName;
            }

            if (!string.IsNullOrWhiteSpace(data.Email))
            {
                user.Email = data.Email;
            }

            if (data.Role != null) // cause string.Empty is a 'remove role' command
            {
                var roleUpdated = UpdateUserRolesSecure(user, data.Role);

                if (!roleUpdated.Succeeded)
                {
                    return roleUpdated;
                }
            }

            return UserManager.Update(user);
        }

        private IdentityResult UpdateUserRolesSecure(ApplicationUser user, params string[] rolesCollection)
        {
            if (string.IsNullOrWhiteSpace(user?.Id))
            {
                throw new ArgumentException(nameof(user));
            }

            if (!CurrentUserIsAdmin)
            {
                return new IdentityResult(IdentityRoles.Admin + " role required to manage user roles!");
            }

            var roles = rolesCollection.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

            if (roles.Length > 0)
            {
                var invalidRoles = roles.Where(r => !IdentityRoles.Validate(r)).ToArray();

                if (invalidRoles.Length > 0)
                {
                    return new IdentityResult($"'{string.Join(", ", invalidRoles)}' is(are) not a correct role(s)!");
                }
            }

            var userRoles = !string.IsNullOrWhiteSpace(user.Id)
                ? UserManager.GetRoles(user.Id) ?? new List<string>()
                : new List<string>();

            var rolesToAdd = roles.Where(r => userRoles.All(x => x != r)).ToArray();
            var rolesToRemove = userRoles.Where(x => roles.All(r => r != x)).ToArray();

            if (rolesToAdd.Length > 0)
            {
                var addRoles = UserManager.AddToRoles(user.Id, rolesToAdd);

                if (!addRoles.Succeeded)
                {
                    return addRoles;
                }
            }

            if (rolesToRemove.Length > 0)
            {
                var removeRoles = UserManager.RemoveFromRoles(user.Id, rolesToRemove);

                if (!removeRoles.Succeeded)
                {
                    // try to rollback added roles
                    if (rolesToAdd.Length > 0)
                    {
                        UserManager.RemoveFromRoles(user.Id, rolesToAdd);
                    }

                    return removeRoles;
                }
            }

            return IdentityResult.Success;
        }

        #endregion
    }
}