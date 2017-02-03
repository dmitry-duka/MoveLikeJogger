using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;
using MoveLikeJogger.DataModels.Moves;

namespace MoveLikeJogger.DataModels.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Moves = new List<Move>();
        }

        public bool IsDeleted { get; set; }

        public virtual ICollection<Move> Moves { get; set; }
    }
}