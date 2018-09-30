using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Resume.Config
{
    public class Skills
    {
        [YamlMember(Alias = "group_name")]
        public string GroupName { get; set; }
        
        [YamlMember(Alias = "values")]
        public List<string> Values { get; set; }
    }
}