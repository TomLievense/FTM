using System;
using System.IO;
using System.Linq;


public static partial class Command
{
    public static void Baseline(string[] options, string[] args)
    {
        if (!Verification.ProfileIsValid())
            return;

        if(args.Length == 0)
        {
            Msg.InfoBaseline();
            return;
        }

        if (options.Length > 0)
        {
            ErrorMsg.TooManyOptions();
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

        string serverPath = Program.settings.Read("remote");
        string projectPath = serverPath + "/" + args[0];

        if (!Directory.Exists(projectPath))
        {
            ErrorMsg.ProjectIncorrect();
            return;
        }

        Print.Message("All history data will be lost " +
            "except for previous baselines.\n" +
            "Are you sure you want to continue? [Y/n]");

        string answer = Console.ReadLine();

        switch (answer.ToLower())
        {
            case "":
            case "yes":
            case "y":
                RemoveHistory(projectPath, args[1]);
                break;

            default:
                return;
        }
    }

    private static void RemoveHistory(string projectPath, string baselineTitle)
    {
        string absPath = projectPath + "/.ftm/";
        string[] logs = Directory.EnumerateFiles(absPath).Where(
            f => !f.EndsWith(".ftm", StringComparison.OrdinalIgnoreCase)).ToArray();

        Array.Sort(logs);
        
        long startUnixStamp = 0;
        long lastUnixStamp = long.Parse(Path.GetFileNameWithoutExtension(logs[logs.Length - 1]));

        Configurator config = new Configurator(logs[logs.Length - 1]);
        string baseLine = config.Read("baseline");
        
        if(baseLine != null)
        {
            Print.ErrorMessage("Project already baselined.");
            return;
        }

        config.Write("baseline", baselineTitle);
        config.Save();        

        // All file versions and directories added between the last 
        // baseline and the current version will be removed.
        for(int i = 0; i < logs.Length -1; i++)
        {
            Configurator statusConfig = new Configurator(logs[i]);
            baseLine = statusConfig.Read("baseline");

            if (baseLine != null)
            {
                long currentUnixStamp = long.Parse(Path.GetFileNameWithoutExtension(logs[i]));
                if (startUnixStamp < currentUnixStamp) startUnixStamp = currentUnixStamp;
            }
        }
        
        int errors = 0;
        int warnings = 0;

        string[] dirs = Directory.GetDirectories(projectPath, "*", SearchOption.AllDirectories);
        for(int i = 0; i < dirs.Length; i++)
        {
            string[] currentLogs = Directory.EnumerateFiles(dirs[i]).Where(
                f => !f.EndsWith(".ftm", StringComparison.OrdinalIgnoreCase)).ToArray();
            
            Array.Sort(currentLogs);
            for(int j = 0; j < currentLogs.Length - 1; j++)
            {
                if(!long.TryParse(Path.GetFileNameWithoutExtension(currentLogs[j]), out long unixStamp))
                    continue;

                if(unixStamp > startUnixStamp)
                {
                    Print.Message("removing '" + Path.GetDirectoryName(currentLogs[j]) + "'");
                    try
                    {
                        File.Delete(currentLogs[j]);
                    }
                    catch
                    {
                        Print.Message("could not remove '" + Path.GetDirectoryName(currentLogs[j]) + "'");
                        errors++;
                    }
                }
            }
        }

        foreach(string dir in dirs)
        {
            if(Directory.GetFiles(dir).Length == 0 &&
               Directory.GetDirectories(dir).Length == 0)
            {
                Print.Message("removing  directory '" + Path.GetDirectoryName(dir) + "'");
                try 
                {
                    Directory.Delete(dir);
                }
                catch
                {
                    Print.Message("could not remove directory '" + Path.GetDirectoryName(dir) + "'");
                    warnings++;
                }
            }
        }

        Print.Message("Operation finished.");
        Print.Message("Errors: " + errors);
        Print.Message("Warnings: " + warnings);
    }
}
