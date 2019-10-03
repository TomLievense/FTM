using System.IO;


public static partial class Command
{
    public static void Find(string[] options, string[] args)
    {
        if (!Verification.ProfileIsValid())
            return;

        if (args.Length == 0)
        {
            Msg.InfoFind();
            return;
        }

        if (args.Length > 1)
        {
            ErrorMsg.TooManyArgs();
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

        string path;
        if (wOption)
            path = Program.settings.Read("workspace");
        else
            path = Program.settings.Read("remote");

        string tag = args[0];

        FindProject(tag, path, cOption);
    }

    private static void FindProject(string tag, string path, bool cOption)
    {
        string[] dirs = Directory.GetDirectories(path);
        string _tag;

        if (cOption)
            _tag = tag;

        else _tag
                = tag.ToLower();

        foreach (string dir in dirs)
        { 
            string project = Path.GetFileName(dir);

            string projectName;
            if (cOption)
                projectName = project;
            else
                projectName = project.ToLower();

            if (projectName.Contains(_tag)) 
                Print.Message(project);
        }
    }
}
