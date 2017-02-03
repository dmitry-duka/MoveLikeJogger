using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using MoveLikeJogger.DataModels.Identity;
using MoveLikeJogger.DataModels.Moves;

namespace MoveLikeJogger.DataMining.DB
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Move> Moves { get; set; }
    }
}