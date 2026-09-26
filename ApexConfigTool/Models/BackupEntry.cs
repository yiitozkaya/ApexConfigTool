using System;

namespace ApexConfigTool.Models
{
    public class BackupEntry
    {
        public string FullPath { get; set; } = "";

        public string FileName { get; set; } = "";

        public string Type { get; set; } = "";

        public DateTime CreatedAt { get; set; }

        public long SizeBytes { get; set; }


        public string DisplayText
        {
            get
            {
                return
                    $"{CreatedAt:yyyy-MM-dd HH:mm:ss}   " +
                    $"{FileName}   " +
                    $"({FormatBytes(SizeBytes)})";
            }
        }


        private static string FormatBytes(
            long bytes)
        {
            if (bytes < 1024)
                return $"{bytes} B";

            double kb =
                bytes / 1024.0;

            if (kb < 1024)
                return $"{kb:0.0} KB";

            double mb =
                kb / 1024.0;

            return $"{mb:0.0} MB";
        }
    }
}