using System;
using System.IO;


public static partial class Command
{
    public static void GetStatus(string[] options, string[] args)
    {
        if (!Verification.ProfileIsValid())
            return;

        if (args.Length > 1)
        {
            ErrorMsg.TooManyArgs();
            return;
        }

        if (options.Length > 2)
        {
            ErrorMsg.TooManyOptions();
            return;
        }

        if (args.Length == 0)
        {
            Msg.InfoStatus();
            return;
        }

        bool userStatus = false;
        bool checkedOutStatus = false;

        foreach(string option in options)
        {
            switch (option)
            {
                case "-o":
                case "--out":
                    checkedOutStatus = true;
                    break;

                case "-u":
                case "--user":
                    userStatus = true;
                    break;

                default:
                    ErrorMsg.UnknownOption();
                    return;
            }
        }

        if (userStatus)
        {
           GetUserProjectStatus(args[0], checkedOutStatus);
        }
        else if(!checkedOutStatus)
        {
            string serverPath = Program.settings.Read("remote");
            string path = serverPath + "/" + args[0];

            GetProjectStatus(path);
        }
        else
        {
            Print.ErrorMessage("Option '--out' or '-o' cannot be used in the current context.");
        }
    }

    private static void GetProjectStatus(string path)
    {
        if (!Directory.Exists(path))
        { 
            ErrorMsg.ProjectIncorrect();
            return;
        }
        var status = GetProjectDetails(path);

        Print.Message("Status: " + status.status);
        Print.Message("Modified By: " + status.modifiedBy);
        Print.Message("Modified By Device: " + status.modifiedByDevice);
    }

    private static void GetUserProjectStatus(string user, bool checkOutStatus)
    {
        string serverPath = Program.settings.Read("remote");
        string[] projects = Directory.GetDirectories(serverPath);

        foreach (string project in projects)
        {
            var status = GetProjectDetails(project);

            if (status.modifiedBy == user)
            {
                string projectName = Path.GetFileName(project);

                if (checkOutStatus && status.status != "checked out")
                    continue;

                Print.Message(projectName + " (" + status.status + ")");
            }
        }
    }

    private static (string status, string modifiedBy, string modifiedByDevice)
        GetProjectDetails(string projectPath)
    { 
        string absPath = projectPath + "/.ftm/";
        if (!Directory.Exists(absPath))
            return (null, null, null);

        string[] logs = Directory.GetFiles(absPath);
        Array.Sort(logs);

        if (logs.Length < 1)
            return (null, null, null);

        Configurator statusConfig = new Configurator(logs[logs.Length - 1]);

        string status = statusConfig.Read("status");
        string modifiedBy = statusConfig.Read("modified by user");
        string modifiedByDevice = statusConfig.Read("modified by device");

        return (status, modifiedBy, modifiedByDevice);
    }
}
