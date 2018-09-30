using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Resume.Config
{
    public class Project
    {
        [YamlMember(Alias = "name")]
        public string Name { get; set; }
        
        [YamlMember(Alias = "link")]
        public Link Link { get; set; }
        
        [YamlMember(Alias = "icon")]
        public string Icon { get; set; }
        
        [YamlMember(Alias = "description")]
        public string Description { get; set; }
        
        [YamlMember(Alias = "tech_used")]
        public List<string> TechUsed { get; set; }
    }
}