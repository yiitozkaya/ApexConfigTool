namespace ApexConfigTool.Models
{
    public class ConfigChange
    {
        public string Source { get; set; } = "";

        public string Setting { get; set; } = "";

        public string OldValue { get; set; } = "";

        public string NewValue { get; set; } = "";
    }
}