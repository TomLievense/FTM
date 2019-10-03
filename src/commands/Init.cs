using System.IO;

// Command init is used to create the .ftm file for a new project.
// The .ftm file contains file ignore rules and a project identifier. 

public static partial class Command
{
    public static void Init(string[] options, string[] args)
    {
        if (!Verification.ProfileIsValid())
            return;

        if (args.Length == 0)
        {
            Msg.InfoInit();
            return;
        }

        if (args.Length > 1)
        {
            ErrorMsg.TooManyArgs();
            return;
        }

        if (options.Length > 0)
        {
            ErrorMsg.NoOptions();
            return;
        }

        string workspacePath = Program.settings.Read("workspace");
        string dirPath = workspacePath + "/" + args[0];

        if(!Directory.Exists(dirPath))
        {
            ErrorMsg.ProjectIncorrect();
            return;
        }

        string filePath = dirPath + "/.ftm";
        if(File.Exists(filePath))
        {
            Print.ErrorMessage("Project already initialized");
            return;
        }

        ProjectConfig.Create(filePath);
    }
}
