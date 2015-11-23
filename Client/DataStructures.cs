using System.Runtime.Serialization;

namespace LianaMailerClient
{
    [DataContract]
    public class EchoResponse
    {
        [DataMember(Name = "succeed")]
        public bool Succeed { get; set; }
        [DataMember(Name = "result")]
        public string Result { get; set; }
    }
}
