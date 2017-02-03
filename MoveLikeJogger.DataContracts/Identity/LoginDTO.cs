using System.Runtime.Serialization;

namespace MoveLikeJogger.DataContracts.Identity
{
    [DataContract]
    public class LoginDTO
    {
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public bool RememberMe { get; set; }
    }
}
