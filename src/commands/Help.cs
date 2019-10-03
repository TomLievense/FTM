public static partial class Command
{
    public static void Help(string[] options, string[] args)
    {
        if (args.Length > 0)
        {
            ErrorMsg.TooManyArgs();
            return;
        }

        if (options.Length > 0)
        {
            ErrorMsg.TooManyOptions();
            return;
        }

        string commandList =
            "help                     Returns available commands\n" +
            "ls -w                    Returns projects in workspace\n" +
            "ls -r                    Returns projects in remote location\n" +
            "config -w [path]         Configure workspace path\n" +
            "config -r [path]         Configure remote location path\n" +
            "config -u [name]         Configure user name\n" +
            "config -d [name]         Configure device name\n" +
            "clone [path]             Clone a Git project\n" +
            "count [project]          Count lines of code of project\n"+
            "init [project]           Initialize project\n" +
            "push [project]           Upload project to server without checking in\n" +
            "pull [project]           Download project from server without checking out\n" +
            "checkin [project]        Upload project to remote location\n" +
            "checkout [project]       Download project from remote location\n" +
            "status [project]         Returns project status\n" +
            "status -u [user]         Returns all projects modified by user\n" +
            "status -u -o [user]      Returns all projects checked out by user\n" +
            "log [date] [date] [proj] Returns change between start and end date\n" +
            "rm [project]             Remove Project from workspace\n" +
            "find [-c | -w] [tag]     Returns projects that contain 'expression' \n" +
            "tree -r                  Returns a revision tree of all files in remote location\n" +
            "tree -w                  Returns a files tree of all files in workspace\n" +
            "create [path] [text]     Creates a text file in workspace\n" +
            "diff -[0] -[1] [file]    Compare two versions of a file in remote location\n" +
            "baseline [project] [tag] Remove all irrelevant versions of a project\n" +
            "clear                    Clear terminal\n";

        Print.Message(commandList);
    }
}