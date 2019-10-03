using System.IO;


public static partial class Command
{
    public static void Show(string[] options, string[] args)
    {
        if (!Verification.ProfileIsValid())
            return;

        if (options.Length == 0)
        {
            ErrorMsg.NoOptions();
            return;
        }

        if(args.Length == 0)
        {
            ErrorMsg.NoArgs();
            return;
        }

        if (options.Length > 2)
        {
            ErrorMsg.TooManyOptions();
            return;
        }

        if (args.Length > 1)
        {
            ErrorMsg.TooManyArgs();
            return;
        }

        bool wOption = false;
        bool rOption = false;

        int previousVersion = 0;
        
        foreach (string option in options)
        {
            switch (option)
            {
                case "-w":
                case "--workspace":
                    wOption = true;
                    break;

                case "-r":
                case "--remote":
                    rOption = true;
                    break;

                default:

                    // Check if option is version notation (for example -3)
                    if (int.TryParse(option, out previousVersion)) break;
                                        
                    ErrorMsg.UnknownOption();
                    return;
            }
        }

        if(wOption && !rOption)
            ShowFileWorkSpace(args[0]);

        if(rOption && !wOption)
            ShowFileRemote(args[0], previousVersion);
    }

    private static void ShowFileWorkSpace(string project)
    {
        string workspacePath = Program.settings.Read("workspace");
        string path = workspacePath + "/" + project;

        if (!File.Exists(path))
        {
            Print.ErrorMessage("File does not exist.");
            return;
        }

        string content = File.ReadAllText(path);
        Print.Message(content);
    }

    private static void ShowFileRemote(string project, int previousVersion)
    {
        string remotePath = Program.settings.Read("remote");
        string path = remotePath + "/" + project;

        if(!Directory.Exists(path))
        {
            Print.ErrorMessage("Path does not exist.");
            return;
        }

        string[] files = Directory.GetFiles(path);

        try
        {
            string content = File.ReadAllText(files[files.Length + previousVersion - 1]);
            Print.Message(content);
        }
        catch
        {
            Print.ErrorMessage("Could not read file.");
        }
    }
}