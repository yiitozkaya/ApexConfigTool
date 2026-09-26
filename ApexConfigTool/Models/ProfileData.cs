using System;
using System.Text.Json.Serialization;

namespace ApexConfigTool.Models
{
    public class ProfileData
    {
        public string Name { get; set; } = "";

        public DateTime CreatedAt { get; set; }

        public string VideoConfigContent { get; set; } = "";

        public bool VideoConfigReadOnly { get; set; }

        public string? AutoexecContent { get; set; }


        [JsonIgnore]
        public string FilePath { get; set; } = "";


        [JsonIgnore]
        public string DisplayText
        {
            get
            {
                string autoexec =
                    AutoexecContent != null
                        ? " + Autoexec"
                        : "";

                return
                    $"{Name}   —   " +
                    $"{CreatedAt:yyyy-MM-dd HH:mm}" +
                    autoexec;
            }
        }
    }
}