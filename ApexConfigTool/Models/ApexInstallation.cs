namespace ApexConfigTool.Models
{
    public class ApexInstallation
    {
        public bool ConfigFound { get; set; }

        public string VideoConfigPath { get; set; } = "";

        public string ConfigDirectory { get; set; } = "";


        public bool GameFound { get; set; }

        public string GameDirectory { get; set; } = "";

        public string ExecutablePath { get; set; } = "";

        public string CfgDirectory { get; set; } = "";

        public string AutoexecPath { get; set; } = "";

        public string Launcher { get; set; } = "Unknown";
    }
}