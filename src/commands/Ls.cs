using System.IO;


public static partial class Command
{
    public static void GetList(string[] options, string[] args)
    {
        if (!Verification.ProfileIsValid())
            return;

        if (args.Length > 0)
        {
            ErrorMsg.TooManyArgs();
            return;
        }

        if (options.Length > 1)
        {
            ErrorMsg.TooManyOptions();
            return;
        }

        if (options.Length == 0)
        {
            Msg.InfoLs();
            return;
        }

        switch (options[0])
        {
            case "-w":
            case "--workspace":
                PrintProjects(true);
                break;

            case "-r":
            case "--remote":
                PrintProjects(false);
                break;

            default:
                ErrorMsg.UnknownArg();
                break;
        }
    }

    private static void PrintProjects(bool workspace)
    {
        string path = workspace ?
            Program.settings.Read("workspace") :
            Program.settings.Read("remote");

        string[] projects = Directory.GetDirectories(path);

        for (int i = 0; i < projects.Length; i++)
            projects[i] = Path.GetFileName(projects[i]);
        
        Print.List(projects);
    }
}

