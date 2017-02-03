using System.Runtime.Serialization;

namespace MoveLikeJogger.DataContracts.Identity
{
    [DataContract]
    public class IdentityDTO
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string Role { get; set; }
    }
}
