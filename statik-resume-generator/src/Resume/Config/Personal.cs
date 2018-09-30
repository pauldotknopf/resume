using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Resume.Config
{
    public class Personal
    {   
        [YamlMember(Alias = "name")]
        public string Name { get; set; }
        
        [YamlMember(Alias = "position_title")]
        public string PositionTitle { get; set; }
        
        [YamlMember(Alias = "email")]
        public string Email { get; set; }
        
        [YamlMember(Alias = "phone")]
        public string Phone { get; set; }
        
        [YamlMember(Alias = "image")]
        public string Image { get; set; }
        
        [YamlMember(Alias = "links")]
        public List<Link> Links { get; set; }
    }
}