using System;
using System.IO;


public static partial class Command
{
    public static void Tree(string[] options, string[] args)
    {
        if (!Verification.ProfileIsValid())
            return;

        if (args.Length + options.Length == 0)
        {
            Msg.InfoTree();
            return;
        }

        if (options.Length == 0)
        {
            ErrorMsg.NoOptions();
            return;
        }

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

        string workspacePath = Program.settings.Read("workspace");
        string serverPath = Program.settings.Read("remote");

        if (args.Length > 0)
        {
            workspacePath += "/" + args[0];
            serverPath += "/" + args[0];
        }

        switch (options[0])
        {
            case "-w":
            case "--workspace":
                Print.Message(" *");
                GetTree(workspacePath, "", false);
                break;

            case "-r":
            case "--remote":
                Print.Message(" *");
                GetTree(serverPath, "", true);
                break;

            default:
                ErrorMsg.UnknownOption();
                break;
        }
    }

    private static void GetTree(string path, string level, bool viewHistory)
    {
        const string corner = " └─ ";
        const string cross = " ├─ ";
        const string line = " │ ";
        const string space = "   ";

        if (!Directory.Exists(path))
        {
            ErrorMsg.PathIncorrect();
            return;
        }

        string[] dirs = Directory.GetDirectories(path);
        string[] files = Directory.GetFiles(path);

        Array.Sort(dirs);
        Array.Sort(files);

        for (int j = 0; j < files.Length; j++)
        {
            string newFile = Path.GetFileName(files[j]);

            // Converts the UnixStamps, which are used to save files, to American DateTime format.
            // This can only be used to read projects from the "remote location" since other files have regular names.
            if (viewHistory)
            {
                bool isUnixStamp = long.TryParse(newFile, out long unixStamp);
                if (isUnixStamp) newFile = UnixStampToDate(unixStamp);
            }

            if (j == files.Length - 1 && dirs.Length == 0)
            {
                Print.Message(level + corner + newFile);
                break;
            }

            Print.Message(level + cross + newFile);
        }

        for (int i = 0; i < dirs.Length; i++)
        {
            string newDir = Path.GetFileName(dirs[i]);

            if (i == dirs.Length - 1)
            {
                Print.Message(level + corner + newDir);
                GetTree(dirs[i], level + space, viewHistory);
            }
            else
            {
                Print.Message(level + cross + newDir);
                GetTree(dirs[i], level + line, viewHistory);
            }
        }
    }

    private static string UnixStampToDate(long unixTimeStamp)
    {
        DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(unixTimeStamp).LocalDateTime;
        return dateTime.ToString("MM/dd/yyyy hh:mm tt");
    }
}
