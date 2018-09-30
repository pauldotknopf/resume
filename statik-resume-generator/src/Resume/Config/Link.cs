using YamlDotNet.Serialization;

namespace Resume.Config
{
    public class Link
    {
        [YamlMember(Alias = "url")]
        public string Url { get; set; }
            
        [YamlMember(Alias = "name")]
        public string Name { get; set; }
            
        [YamlMember(Alias = "icon")]
        public string Icon { get; set; }
    }
}