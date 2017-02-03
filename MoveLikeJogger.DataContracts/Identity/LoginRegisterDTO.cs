using System.Runtime.Serialization;

namespace MoveLikeJogger.DataContracts.Identity
{
    [DataContract]
    public class LoginRegisterDTO : LoginDTO
    {
        [DataMember]
        public string Email { get; set; }
    }
}
