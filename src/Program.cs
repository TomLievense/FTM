using System;

class Program
{
    public static Configurator settings;
    public static string profileName;
    public const string REVNO = "FTM-0.926";

    public static void Main(string[] args)
    {
        string configPath = AppDomain.CurrentDomain.BaseDirectory + "config/profile.ftm";
        settings = new Configurator(configPath);

        Init();

        if (args.Length > 0)
        {
            string startupInput = string.Join(" ", args);
            Parser.Tokenize(startupInput);

            Environment.Exit(0);
        }

        while (true)
        {
            Console.Write(profileName + "# ");
            string input = AutoComplete.GetInput();

            if (input.Length > 1)
                Parser.Tokenize(input);
        }
    }

    public static void Init()
    {
        profileName = REVNO + ":" + settings.Read("username");

        if (profileName == null)
        {
            ErrorMsg.ProfileNotConfigured();
            profileName = REVNO;
        }
    }
}

