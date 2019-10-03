using System;
using System.Collections.Generic;
using System.IO;


public static partial class Command
{
    public static void Push(string[] options, string[] args, bool checkIn)
    {
        if (!Verification.ProfileIsValid())
            return;

        if (options.Length > 1)
        {
            ErrorMsg.TooManyOptions();
            return;
        }

        if (args.Length > 1)
        {
            ErrorMsg.TooManyArgs();
            return;
        }

        if (args.Length == 0)
        {
            if (checkIn)
                Msg.InfoCheckIn();
            else
                Msg.InfoPush();

            return;
        }

        // Option '--force' must be used to check in up-to-date projects.
        bool force = false;
        if(options.Length == 1)
        {
            switch (options[0])
            {
                case "-f":
                case "--force":
                    force = true;
                    break;

                default:
                    ErrorMsg.UnknownOption();
                    return;
            }
        }
        
        string workspacePath = Program.settings.Read("workspace");
        string serverPath = Program.settings.Read("remote");
        string path = workspacePath + "/" + args[0];

        if (!PushPermission(serverPath + "/" + args[0]))
            return;

        string timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();

        if (!Directory.Exists(path))
        {
            ErrorMsg.ProjectIncorrect();
            return;
        }

        var projectConfig = GetProjectConfig(path, serverPath + "/" + args[0]);

        if (!projectConfig.isUnique)
            return;

        string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
        bool changesMade = false;

        List<string> pushedFiles = new List<string>();

        foreach (string file in files)
        {
            string fileName = Path.GetFileName(file);

            if (fileName == ".ftm")
                continue;

            bool doIgnore = false;
            if (projectConfig.ignore != null)
            {
                foreach (string ignore in projectConfig.ignore)
                {
                    string relevantPath =
                        (ignore.Contains("/") || ignore.Contains("\\")) ?
                        file :
                        fileName;

                    if (ignore.StartsWith("*", StringComparison.Ordinal) &&
                        !ignore.EndsWith("*", StringComparison.Ordinal))
                    {
                        if (relevantPath.EndsWith(ignore.Replace("*", ""), StringComparison.Ordinal))
                        {
                            doIgnore = true;
                            break;
                        }
                    }
                    else if (ignore.StartsWith("*", StringComparison.Ordinal) &&
                        ignore.EndsWith("*", StringComparison.Ordinal))
                    {
                        if (relevantPath.Contains(ignore.Replace("*", "")))
                        {
                            doIgnore = true;
                            break;
                        }
                    }
                    else if (!ignore.StartsWith("*", StringComparison.Ordinal) &&
                        ignore.EndsWith("*", StringComparison.Ordinal))
                    {
                        if (relevantPath.StartsWith(ignore.Replace("*", ""), StringComparison.Ordinal))
                        {
                            doIgnore = true;
                            break;
                        }
                    }
                }
            }

            if (doIgnore)
                continue;

            string relativePath = file.Substring(workspacePath.Length);
            string serverFilePath = serverPath + "/" + relativePath;

            pushedFiles.Add(file);

            if (!Directory.Exists(serverFilePath))
            {
                Directory.CreateDirectory(serverFilePath);
                Print.Message("Directory '" + Path.GetFileName(serverFilePath) + "' created.");
            }
            else
            {
                string[] existingFiles = Directory.GetFiles(serverFilePath);
                Array.Sort(existingFiles);

                if (!IsNewFile(existingFiles, file))
                    continue;
            }

            Print.Message("Copying '" + Path.GetFileName(file) + "'");

            try
            {
                File.Copy(file, serverFilePath + "/" + timestamp, true);
                changesMade = true;
            }
            catch
            {
                Print.ErrorMessage("Could not copy '" + Path.GetFileName(file) + "'");
            }
        }

        if (changesMade || force)
        {
            SetStatus(serverPath + "/" + args[0], checkIn, timestamp, pushedFiles.ToArray());

            if (checkIn)
                RemoveProject(path);
        }
        else
        {
            Msg.ProjectUpToDate();
        }
    }

    private static bool IsNewFile(string[] existingFiles, string newFile)
    {
        int totalFiles = existingFiles.Length;
        if (totalFiles < 1)
            return true;
        
        string originalContent = File.ReadAllText(existingFiles[totalFiles - 1]);
        string newContent = File.ReadAllText(newFile);

        if (newContent != originalContent)
            return true;

        return false;
    }

    private static void SetStatus(string projectPath, bool checkIn, string timeStamp, string[] files)
    {            
        string absPath = projectPath + "/.ftm/";
        if (!Directory.Exists(absPath))
            Directory.CreateDirectory(absPath);

        Configurator statusConfig = new Configurator(absPath + "/" + timeStamp);

        string status = checkIn ? "checked in" : "checked out";
        string deviceName = Program.settings.Read("device");
        string userName = Program.settings.Read("username");

        statusConfig.Write("status", status);
        statusConfig.Write("modified by user", userName);
        statusConfig.Write("modified by device", deviceName);

        int lengthWorkSpacePath = Program.settings.Read("workspace").Length;

        for (int i = 0; i < files.Length; i++)
        {
            string key = "file_" + i;
            string fileName = files[i].Substring(lengthWorkSpacePath);

            statusConfig.Write(key, fileName);
        }

        statusConfig.Save();
    }

    private static (bool isUnique , string[] ignore)
        GetProjectConfig(string projectPath, string remoteProjectPath)
    {
        string keyPath = projectPath + "/.ftm";
        string remoteKeyPath = remoteProjectPath + "/.ftm/.ftm";

        if (!File.Exists(keyPath))
            ProjectConfig.Create(keyPath);

        string remoteKey = null;
        if (File.Exists(remoteKeyPath))
        {
            string[] remoteConfig = File.ReadAllLines(remoteKeyPath);
            foreach (string line in remoteConfig)
            {
                if (line.StartsWith("Identifier<", StringComparison.Ordinal))
                {
                    remoteKey = line;
                    break;
                }
            }
        }

        string settingsPath = remoteProjectPath + "/.ftm/";
        if (!Directory.Exists(settingsPath))
            Directory.CreateDirectory(settingsPath);
       
        File.Copy(keyPath, remoteKeyPath, true);

        string[] localConfig = File.ReadAllLines(keyPath);
        bool isIgnore = false;

        List<string> ignoreList = new List<string>();

        string key = null;
        foreach(string line in localConfig)
        {
            if (line.StartsWith("Identifier<", StringComparison.Ordinal)) 
                key = line;

            if (line.StartsWith("Ignore", StringComparison.Ordinal) &&
                line.EndsWith("{", StringComparison.Ordinal))
            {
                isIgnore = true;
                continue;
            }

            if(isIgnore && !line.Contains("}"))
                ignoreList.Add(line.Trim());
        }

        if (remoteKey == null || key == remoteKey)
            return (true, ignoreList.ToArray());

        Print.ErrorMessage("Remote location already contains a different project with the same name.");
        return (false, null);
    }

    private static bool PushPermission(string projectPath)
    {
        string absPath = projectPath + "/.ftm/";
        if (!Directory.Exists(absPath))
            return true;

        string[] logs = Directory.GetFiles(absPath);
        if (logs.Length < 1)
            return false;

        Configurator statusConfig = new Configurator(logs[logs.Length - 1]);

        string myDeviceName = Program.settings.Read("device");
        string myUserName = Program.settings.Read("username");

        string status = statusConfig.Read("status");
        string modifiedBy = statusConfig.Read("modified by user");
        string modifiedByDevice = statusConfig.Read("modified by device");
        
        if (status == "checked in")
        {
            ErrorMsg.ProjectAlreadyCheckedIn();
            return false;
        }
        
        if (status == "checked out")
        {
            if (myDeviceName != modifiedByDevice)
            {
                ErrorMsg.ProjectCheckedOutByOtherDevice();
                return false;
            }

            if (myUserName != modifiedBy)
            {
                ErrorMsg.ProjectCheckedOutByOtherUsr();
                return false;
            }
        }

        return true;
    }
}
