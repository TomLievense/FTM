using System.Collections.Generic;
using System.IO;


public static partial class Command
{
    public static void Delete(string[] options, string[] args)
    {
        if (!Verification.ProfileIsValid())
            return;

        if (options.Length > 0)
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
            Msg.InfoPush();
            return;
        }

        // Command rm can only be used to remove projects in workspace.
        string workspacePath = Program.settings.Read("workspace");
        string path = workspacePath + "/" + args[0];

        if (!Directory.Exists(path))
        {
            ErrorMsg.ProjectIncorrect();
            return;
        }

        RemoveProject(path);
    }

    private static void RemoveProject(string path)
    {
        string[] allFiles = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
        foreach (string file in allFiles) // remove all files
        {
            try
            {
                File.Delete(file);
                Print.Message("Removing file '" + Path.GetFileName(file) + "'");
            }
            catch
            {
                Print.ErrorMessage("Could not remove file '" + Path.GetFileName(file) + "'");
            }
        }

        List<string> allDirs = new List<string>();
        allDirs.AddRange(Directory.GetDirectories(path, "*.*", SearchOption.AllDirectories));
        allDirs.Reverse();

        foreach (string dir in allDirs)
        {
            try
            {
                Directory.Delete(dir);
                Print.Message("Removing directory '" + Path.GetFileName(dir) + "'");
            }
            catch
            {
                Print.ErrorMessage("Could not remove directory '" + Path.GetFileName(dir) + "'");
            }
        }

        try
        {
            Directory.Delete(path);

        }
        catch
        {
            Print.ErrorMessage("Could not remove directory '" + Path.GetFileName(path));
        }
    }
}

