using System.IO;


public static partial class Command
{
    public static void Create(string[] options, string[] args)
    {
        if (!Verification.ProfileIsValid())
            return;

        if (args.Length == 0 || args[0] == null)
        {
            Msg.InfoCreate();
            return;
        }

        if (args.Length > 2)
        {
            ErrorMsg.TooManyArgs();
            return;
        }

        if (options.Length > 0)
        {
            ErrorMsg.TooManyOptions();
            return;
        }

        if (args.Length < 2)
        {
            ErrorMsg.UnknownArg();
            return;
        }

        string workspacePath = Program.settings.Read("workspace");

        string path = workspacePath + "/" + args[0];
        string content = args[1];

        try
        {
            File.WriteAllText(path, content);
        }
        catch
        {
            Print.ErrorMessage("Could not create new file.");
        }
    }
}

