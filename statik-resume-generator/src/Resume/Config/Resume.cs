using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Resume.Config
{
    public class Resume
    {
        [YamlMember(Alias="career")]
        public string Career { get; set; }
        
        [YamlMember(Alias = "experience")]
        public List<Experience> Experience { get; set; }
        
        [YamlMember(Alias = "education")]
        public List<Education> Education { get; set; }
        
        [YamlMember(Alias = "projects")]
        public List<Project> Projects { get; set; }
        
        [YamlMember(Alias = "skills")]
        public List<Skills> Skills { get; set; }
    }
}