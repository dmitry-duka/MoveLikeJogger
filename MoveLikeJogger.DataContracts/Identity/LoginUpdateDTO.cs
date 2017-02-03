using System.Runtime.Serialization;

namespace MoveLikeJogger.DataContracts.Identity
{
    [DataContract]
    public class LoginUpdateDTO : LoginRegisterDTO
    {
        [DataMember]
        public string NewPassword { get; set; }
    }
}
