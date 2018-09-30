using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Resume.Config
{
    public class Experience
    {
        [YamlMember(Alias = "company")]
        public string Company { get; set; }
        
        [YamlMember(Alias = "title")]
        public string Title { get; set; }
        
        [YamlMember(Alias = "time")]
        public string Time { get; set; }
        
        [YamlMember(Alias = "tech_used")]
        public List<string> TechUsed { get; set; }
        
        [YamlMember(Alias = "description")]
        public string Description { get; set; }
    }
}