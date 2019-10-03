using System.IO;


public static partial class Command
{ 
    public static void Config(string[] options, string[] args)
    {
        if (args.Length + options.Length == 0)
        {
            Msg.InfoConfig();
            return;
        }

        if (args.Length == 0)
        {
            ErrorMsg.InputMissing();
            return;
        }

        if (args.Length > 1)
        {
            ErrorMsg.TooManyArgs();
            return;
        }

        if (options.Length == 0)
        {
            ErrorMsg.NoOptions();
            return;
        }

        switch (options[0])
        {
            case "-w":
            case "--workspace":
                SavePath(args[0], "workspace");
                break;

            case "-r":
            case "--remote":
                SavePath(args[0], "remote");
                break;

            case "-u":
            case "--user":
                SetUser(args[0]);
                break;

            case "-d":
            case "--device":
                SetDevice(args[0]);
                break;

            default:
                ErrorMsg.UnknownArg();
                break;
        }
    }

    private static void SetDevice(string deviceName)
    {
        Program.settings.Write("device", deviceName);
        Program.settings.Save();
        Program.Init();
    }

    private static void SavePath(string path, string key)
    {
        if (!Directory.Exists(path))
        {
            ErrorMsg.PathIncorrect();
            return;
        }

        Program.settings.Write(key, path);
        Program.settings.Save();
    }

    private static void SetUser(string userName)
    {
        Program.settings.Write("username", userName);
        Program.settings.Save();
        Program.Init();
    }
}

