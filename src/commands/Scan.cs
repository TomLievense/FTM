using System;
using System.IO;


public static partial class Command
{
    public static void Scan(string[] options, string[] args)
    {
        if (!Verification.ProfileIsValid())
            return;

        if (args.Length == 0 || args[0] == null)
        {
            Msg.InfoScan();
            return;
        }

        if (args.Length > 2)
        {
            ErrorMsg.TooManyArgs();
            return;
        }

        if (args.Length < 2)
        {
            ErrorMsg.NoArgs();
            return;
        }

        bool wOption = false; // Search in workspace instead of remote location
        bool cOption = false; // Search case sensitive

        foreach (string option in options)
        {
            switch (option)
            {
                case "--workspace":
                case "-w":
                    wOption = true;
                    break;

                case "--remote":
                case "-r":
                    wOption = false;
                    break;

                case "--case":
                case "-c":
                    cOption = true;
                    break;

                default:
                    ErrorMsg.UnknownOption();
                    break;
            }
        }

        string tag = args[1];
        string workspacePath = Program.settings.Read("workspace");
        string serverPath = Program.settings.Read("remote");
        string path = wOption? workspacePath + "/" + args[0] : serverPath + "/" + args[0];

        if(!Directory.Exists(path))
        {
            ErrorMsg.ProjectIncorrect();
            return;
        }

        string[] files;
        try
        {
            files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
        }
        catch
        {
            Print.ErrorMessage("Could not list files.");
            return;
        }

        foreach(string file in files)
        {
            string content = null;
            try
            {
                content = File.ReadAllText(file);
            }
            catch
            {
                string filePath = wOption ? file : Path.GetDirectoryName(file);
                string relativePath = filePath.Substring(workspacePath.Length - 1);

                Print.ErrorMessage("Could not read '" + relativePath + "'");
                continue;
            }

            StringComparison compareMethod = cOption ?
                StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            if(content.Contains(tag, compareMethod))
            {
                string filePath = wOption ?
                    file :
                    Path.GetDirectoryName(file);

                string relativePath = filePath.Substring(serverPath.Length - 1);
                Print.Message(relativePath);
            }
        }
    }
}
