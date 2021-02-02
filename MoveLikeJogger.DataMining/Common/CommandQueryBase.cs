using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoveLikeJogger.DataMining.DB;

namespace MoveLikeJogger.DataMining.Common
{
    public abstract class CommandQueryBase : IDisposable
    {
        protected IApplicationDbContext Db;

        protected CommandQueryBase(IApplicationDbContext context)
        {
            Db = context;
        }

        public void Dispose()
        {
            if (Db == null)
            {
                return;
            }

            Db.Dispose();
            Db = null;
        }
    }
}
