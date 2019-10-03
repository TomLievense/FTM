using System.IO;


public static class Verification
{ 
    public static bool ProfileIsValid()
    {
        string workspacePath = Program.settings.Read("workspace");
        string remotePath = Program.settings.Read("remote");
        string userName = Program.settings.Read("username");
        string deviceName = Program.settings.Read("device");

        if (remotePath == null)
        {
            Print.ErrorMessage("Remote path not configured.");
            Print.ErrorMessage("Use command: config --remote path/to/remote/location/");

            return false;
        }

        if (workspacePath == null)
        {
            Print.ErrorMessage("Workspace path not configured.");
            Print.ErrorMessage("Use command: config --workspace path/to/workspace/location/");

            return false;
        }

        if (!Directory.Exists(remotePath))
        {    
            Print.ErrorMessage("Remote path does not exist.");
            Print.ErrorMessage("Use command: config --remote path/to/remote/location/");

            return false;
        }

        if(!Directory.Exists(workspacePath))
        {
            Print.ErrorMessage("Workspace path does not exist.");
            Print.ErrorMessage("Use command: config --workspace path/to/workspace/location/");

            return false;
        }

        if(userName == null)
        {
            Print.ErrorMessage("Username not configured.");
            Print.ErrorMessage("Use command: config --user name..");

            return false;
        }

        if (deviceName == null)
        {
            Print.ErrorMessage("Device name not configured.");
            Print.ErrorMessage("Use command: config --device name..");
            return false;
        }

        return true;
    }
}