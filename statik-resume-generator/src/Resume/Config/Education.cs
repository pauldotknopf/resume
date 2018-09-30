using YamlDotNet.Serialization;

namespace Resume.Config
{
    public class Education
    {
        [YamlMember(Alias = "major")]
        public string Major { get; set; }
        
        [YamlMember(Alias = "school")]
        public string School { get; set; }
        
        [YamlMember(Alias = "time")]
        public string Time { get; set; }
    }
}