using System;
using System.Runtime.Serialization;

namespace MoveLikeJogger.DataContracts.Moves
{
    [DataContract]
    public class MoveDTO
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string UserId { get; set; }

        [DataMember]
        public bool IsDeleted { get; set; }

        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public int Distance { get; set; }

        [DataMember]
        public int Duration { get; set; }
    }
}
