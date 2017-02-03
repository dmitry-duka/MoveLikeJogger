using System.Runtime.Serialization;

namespace MoveLikeJogger.DataContracts.Identity
{
    [DataContract]
    public class ApplicationUserDTO
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public bool IsDeleted { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string Role { get; set; }
    }
}
