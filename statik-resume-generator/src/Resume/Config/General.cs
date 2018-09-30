using YamlDotNet.Serialization;

namespace Resume.Config
{
    public class General
    {
        [YamlMember(Alias = "resume_repo")]
        public Link ResumeRepo { get; set; }
        
        [YamlMember(Alias = "cname")]
        public string CName { get; set; }
    }
}