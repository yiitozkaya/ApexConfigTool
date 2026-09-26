using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using ApexConfigTool.Models;

namespace ApexConfigTool.Services
{
    public class AutoexecService
    {
        private const string BeginMarker =
            "// ===== ApexConfigTool BEGIN =====";

        private const string EndMarker =
            "// ===== ApexConfigTool END =====";


        public string AutoexecPath { get; }

        public string CfgDirectory { get; }


        public string BackupDirectory =>
            Path.Combine(
                CfgDirectory,
                "ApexConfigTool_Backups");


        public AutoexecService(
            string autoexecPath)
        {
            AutoexecPath =
                autoexecPath;


            CfgDirectory =
                Path.GetDirectoryName(
                    autoexecPath)
                ?? throw new Exception(
                    "Invalid autoexec path.");
        }


        public bool Exists()
        {
            return File.Exists(
                AutoexecPath);
        }


        public string ReadOrEmpty()
        {
            if (!Exists())
                return "";


            return File.ReadAllText(
                AutoexecPath);
        }


        public string? CreateBackup()
        {
            if (!Exists())
                return null;


            Directory.CreateDirectory(
                BackupDirectory);


            string destination =
                Path.Combine(
                    BackupDirectory,
                    "autoexec_" +
                    DateTime.Now.ToString(
                        "yyyy-MM-dd_HH-mm-ss-fff") +
                    ".cfg");


            File.Copy(
                AutoexecPath,
                destination,
                true);


            return destination;
        }


        public List<BackupEntry> GetBackups()
        {
            if (!Directory.Exists(
                    BackupDirectory))
            {
                return new List<BackupEntry>();
            }


            return Directory
                .GetFiles(
                    BackupDirectory,
                    "autoexec_*.cfg")
                .Select(
                    path =>
                    {
                        FileInfo info =
                            new FileInfo(path);


                        return new BackupEntry
                        {
                            FullPath =
                                path,

                            FileName =
                                info.Name,

                            Type =
                                "Autoexec",

                            CreatedAt =
                                info.CreationTime,

                            SizeBytes =
                                info.Length
                        };
                    })
                .OrderByDescending(
                    x => x.CreatedAt)
                .ToList();
        }


        public void RestoreBackup(
            string backupPath)
        {
            ValidateBackupPath(
                backupPath);


            if (!File.Exists(
                    backupPath))
            {
                throw new FileNotFoundException(
                    "Autoexec backup not found.");
            }


            Directory.CreateDirectory(
                CfgDirectory);


            if (Exists())
            {
                CreateBackup();
            }


            File.Copy(
                backupPath,
                AutoexecPath,
                true);
        }


        public void DeleteBackup(
            string backupPath)
        {
            ValidateBackupPath(
                backupPath);


            if (File.Exists(
                backupPath))
            {
                File.Delete(
                    backupPath);
            }
        }


        private void ValidateBackupPath(
            string backupPath)
        {
            string root =
                Path.GetFullPath(
                    BackupDirectory)
                .TrimEnd(
                    Path.DirectorySeparatorChar)
                +
                Path.DirectorySeparatorChar;


            string target =
                Path.GetFullPath(
                    backupPath);


            if (!target.StartsWith(
                    root,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception(
                    "Invalid autoexec backup path.");
            }
        }


        public void WriteAll(
            string content)
        {
            Directory.CreateDirectory(
                CfgDirectory);


            if (Exists())
            {
                CreateBackup();
            }


            File.WriteAllText(
                AutoexecPath,
                content,
                new UTF8Encoding(false));
        }


        public Dictionary<string, string>
            ParseCommands(
                string content)
        {
            Dictionary<string, string> commands =
                new Dictionary<string, string>(
                    StringComparer.OrdinalIgnoreCase);


            if (string.IsNullOrWhiteSpace(
                    content))
            {
                return commands;
            }


            MatchCollection matches =
                Regex.Matches(
                    content,
                    "^\\s*([A-Za-z0-9_]+)\\s+\"?([^\"\\s;/]+)\"?",
                    RegexOptions.Multiline);


            foreach (Match match in matches)
            {
                commands[
                    match.Groups[1].Value] =
                    match.Groups[2].Value;
            }


            return commands;
        }


        public Dictionary<string, string>
            GetManagedCommands()
        {
            string content =
                ReadOrEmpty();


            string managed =
                ExtractManagedBlock(
                    content);


            return ParseCommands(
                managed);
        }


        public string BuildManagedContent(
            string existingContent,
            Dictionary<string, string> commands)
        {
            string baseContent =
                RemoveManagedBlock(
                    existingContent)
                .TrimEnd();


            if (commands.Count == 0)
            {
                return baseContent;
            }


            StringBuilder managed =
                new StringBuilder();


            managed.AppendLine(
                BeginMarker);


            managed.AppendLine(
                "// Managed by Apex Config Tool");


            foreach (var command
                     in commands)
            {
                if (!IsValidCommandName(
                        command.Key))
                {
                    continue;
                }


                if (!IsSafeValue(
                        command.Value))
                {
                    continue;
                }


                managed.AppendLine(
                    $"{command.Key} \"{command.Value}\"");
            }


            managed.AppendLine(
                EndMarker);


            if (string.IsNullOrWhiteSpace(
                    baseContent))
            {
                return managed.ToString();
            }


            return
                baseContent +
                Environment.NewLine +
                Environment.NewLine +
                managed;
        }


        public void UpdateManagedBlock(
            Dictionary<string, string> commands)
        {
            string result =
                BuildManagedContent(
                    ReadOrEmpty(),
                    commands);


            WriteAll(
                result);
        }


        public string RemoveManagedBlock(
            string content)
        {
            string pattern =
                Regex.Escape(BeginMarker) +
                ".*?" +
                Regex.Escape(EndMarker);


            return Regex.Replace(
                content,
                pattern,
                "",
                RegexOptions.Singleline |
                RegexOptions.IgnoreCase);
        }


        private string ExtractManagedBlock(
            string content)
        {
            string pattern =
                Regex.Escape(BeginMarker) +
                ".*?" +
                Regex.Escape(EndMarker);


            Match match =
                Regex.Match(
                    content,
                    pattern,
                    RegexOptions.Singleline |
                    RegexOptions.IgnoreCase);


            return match.Success
                ? match.Value
                : "";
        }


        public string GetCommandValue(
            string command)
        {
            Dictionary<string, string> commands =
                ParseCommands(
                    ReadOrEmpty());


            return commands.TryGetValue(
                    command,
                    out string? value)
                ? value
                : "";
        }


        private bool IsValidCommandName(
            string command)
        {
            return Regex.IsMatch(
                command,
                "^[A-Za-z0-9_]+$");
        }


        private bool IsSafeValue(
            string value)
        {
            return Regex.IsMatch(
                value,
                "^[A-Za-z0-9_.\\-]+$");
        }
    }
}