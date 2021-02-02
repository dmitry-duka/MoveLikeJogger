using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using MoveLikeJogger.DataModels.Identity;
using MoveLikeJogger.DataModels.Moves;

namespace MoveLikeJogger.DataMining.DB
{
    public interface IApplicationDbContext : IDisposable, IObjectContextAdapter
    {
        IDbSet<ApplicationUser> Users { get; set; }
        IDbSet<IdentityRole> Roles { get; set; }
        DbSet<Move> Moves { get; set; }

        int SaveChanges();

        Task<int> SaveChangesAsync();

        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Configuration.LazyLoadingEnabled = true;
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Move> Moves { get; set; }
    }
}