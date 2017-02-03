using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MoveLikeJogger.DataModels.Identity;

namespace MoveLikeJogger.DataModels.Moves
{
    public class Move
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime Date { get; set; }

        public int Distance { get; set; }

        public int Duration { get; set; }
        
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}