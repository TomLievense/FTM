using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


public static partial class Command
{
    public static void Pull(string[] options, string[] args, bool checkOut)
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
            if (checkOut) Msg.InfoCheckOut();
            else Msg.InfoPush();

            return;
        }

        string workspacePath = Program.settings.Read("workspace");
        string serverPath = Program.settings.Read("remote");

        string path = serverPath + "/" + args[0];

        if (!Directory.Exists(path))
        {
            ErrorMsg.ProjectIncorrect();
            return;
        }

        int stepsBack = 0;
        if(options.Length == 1)
        {
            bool isDigit = int.TryParse(options[0], out stepsBack);

            if (!isDigit)
            {
                ErrorMsg.UnknownOption();
                return;
            }
        }

        if (checkOut)
        {
            var projectStatus = GetProjectDetails(serverPath + "/" + args[0]);
            if (projectStatus.status == "checked out")
            {
                ErrorMsg.ProjectAlreadyCheckedOut();
                return;
            }
        }

        (string[] projectFiles, long targetUnixStamp) =
            GetPullFiles(serverPath + "/" + args[0], checkOut, stepsBack);

        if (targetUnixStamp == 0)
            return;

        if (Directory.Exists(workspacePath + "/" + args[0]))
        {
            Print.Message("Project already exists in workspace. Do you want to overwrite the existing project? [Y/n]");
            string answer = Console.ReadLine();

            switch (answer.ToLower())
            {
                case "":
                case "yes":
                case "y":
                    RemoveProject(workspacePath + "/" + args[0]);
                    break;

                default:
                    return;
            }
        }

        foreach (string projectFile in projectFiles)
        {
            string sourceFileName = serverPath + "/" + projectFile;
            string destFileName = workspacePath + "/" + projectFile;

            PullFile(sourceFileName, destFileName , targetUnixStamp);
        }

        string keyPath = workspacePath + "/" + args[0] + "/.ftm";
        string remoteKeyPath = serverPath + "/" + args[0] + "/.ftm/.ftm";

        try
        {
            File.Copy(remoteKeyPath, keyPath, true);
        }
        catch
        {
           Print.ErrorMessage("Could not find project identifier key");
        }
    }

    private static void PullFile(string sourceFilePath, string destFile, long targetUnixStamp)
    {
        string[] revisionFiles = Directory.GetFiles(sourceFilePath);
        Array.Sort(revisionFiles);

        // Find the right unix stamp if older version is requested
        int totalRevisions = revisionFiles.Length - 1;
        string sourceFile = null;

        for(int i = totalRevisions; i >= 0; i--)
        {
            if(long.TryParse(Path.GetFileName(revisionFiles[i]), out long revisionStamp))
            {
                if (revisionStamp <= targetUnixStamp)
                {
                    sourceFile = revisionFiles[i];
                    break;
                }
            }
        }

        string destDir = Path.GetDirectoryName(destFile);
        string destFileName = Path.GetFileName(destFile);

        if (!Directory.Exists(destDir))
        {
            Directory.CreateDirectory(destDir);
            Print.Message("Creating directory '" + Path.GetFileName(destDir) + "'");
        }

        File.Copy(sourceFile, destFile, true);
        Print.Message("Copying file '" + Path.GetFileName(destFileName) + "'");
    }

    private static (string[] files, long unixStamp) GetPullFiles(string projectPath, bool checkOut, int stepsBack)
    {
        string absPath = projectPath + "/.ftm/";
        string[] logs = Directory.EnumerateFiles(absPath).Where(
            f => !f.EndsWith(".ftm", StringComparison.OrdinalIgnoreCase)).ToArray();

        Array.Sort(logs);

        if (logs.Length <= Math.Abs(stepsBack))
        {
            Print.ErrorMessage("Version does not exist.");
            return (null, 0);
        }

        string targetLog = logs[logs.Length - 1 + stepsBack];
        long targetUnixStamp = long.Parse(Path.GetFileName(targetLog));

        Configurator statusConfig = new Configurator(targetLog);

        List<string> files = new List<string>();

        string filePath = "$";
        int n = 0;

        while (filePath != null)
        {
            string key = "file_" + n;
            filePath = statusConfig.Read(key);

            if (filePath != null)
                files.Add(filePath);

            n++;
        }

        if (checkOut)
        {
            string status = "checked out";
            string deviceName = Program.settings.Read("device");
            string userName = Program.settings.Read("username");

            statusConfig.Write("status", status);
            statusConfig.Write("modified by user", userName);
            statusConfig.Write("modified by device", deviceName);

            statusConfig.Save();
        }

        return (files.ToArray(), targetUnixStamp);
    }
}

