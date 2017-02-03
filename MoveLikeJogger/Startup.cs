using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using MoveLikeJogger.DataMining.DB;
using MoveLikeJogger.DataModels.Identity;
using MoveLikeJogger.Security;
using Owin;

[assembly: OwinStartup(typeof(MoveLikeJogger.Startup))]
namespace MoveLikeJogger
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureRoles();
            ConfigureAuth(app);
        }

        private void ConfigureRoles()
        {
            var context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));//new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (!roleManager.RoleExists(IdentityRoles.Admin))
            {
                CreateRole(roleManager, IdentityRoles.Admin);

                var admin = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@move.like.jogr"
                };

                var acr = userManager.Create(admin, password: "iadmin");

                if (acr.Succeeded)
                {
                    userManager.AddToRole(admin.Id, IdentityRoles.Admin);
                }
                else
                {
                    throw new Exception("Could not create Admin account: "+string.Join("; ", acr.Errors));
                }
            }

            CreateRole(roleManager, IdentityRoles.Manager);
        }

        private static void CreateRole(RoleManager<IdentityRole> roleManager, string role)
        {
            if (roleManager.RoleExists(role))
            {
                return;
            }

            if (!roleManager.Create(new IdentityRole { Name = role }).Succeeded)
            {
                throw new Exception($"Could not create '{roleManager}' role");
            }
        }
    }
}
