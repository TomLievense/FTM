using System;
using System.IO;


public static partial class Command
{
    public static void GetLog(string[] options, string[] args)
    {
        if (!Verification.ProfileIsValid())
            return;

        if (args.Length == 0)
        {
            Msg.InfoLog();
            return;
        }

        if (args.Length < 3)
        {
            ErrorMsg.NoArgs();
            return;
        }

        if (args.Length > 3)
        {
            ErrorMsg.TooManyArgs();
            return;
        }

        if (options.Length > 0)
        {
            ErrorMsg.TooManyOptions();
            return;
        }

        DateTime startDate, endDate;
        long startTimeStamp, endTimeStamp;

        try
        {
            startDate = DateTime.Parse(args[0]);
            endDate = DateTime.Parse(args[1]);

            startTimeStamp = new DateTimeOffset(startDate).ToUnixTimeSeconds();
            endTimeStamp = new DateTimeOffset(endDate).ToUnixTimeSeconds();
        }
        catch
        {
            Print.ErrorMessage("Date format not correct.");
            return;
        }

        string project = args[2];

        string serverPath = Program.settings.Read("remote");
        string projectPath = serverPath + "/" + project + "/";

        if (!Directory.Exists(projectPath))
        {
            ErrorMsg.ProjectIncorrect();
            return;
        }

        string[] projectFiles = Directory.GetFiles(projectPath, "*.*", SearchOption.AllDirectories);

        foreach (string projectFile in projectFiles)
        {
            // If the parse fails, the filename is not a UnixStamp and can be skipped
            if (!long.TryParse(Path.GetFileName(projectFile), out long unixStamp))
                continue;

            if (unixStamp > startTimeStamp && unixStamp < endTimeStamp)
            {
                string filePath = Path.GetDirectoryName(projectFile);
                string pushDate = UnixStampToDate(unixStamp);

                Print.Message(filePath.Substring(serverPath.Length - 1) + " (" + pushDate + ")");
            }
        }
    }
}

