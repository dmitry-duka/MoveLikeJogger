using System.Runtime.Serialization;

namespace MoveLikeJogger.DataContracts.Identity
{
    [DataContract]
    public class ApplicationUserUpdateDTO : ApplicationUserDTO
    {
        [DataMember]
        public string Password { get; set; }
    }
}
