// This file contains additional data for the AutoComplete class

public enum DirType
{
    none,
    remoteDirs,
    workspaceDirs,
    allDirs,
    remoteFiles,
    workspaceFiles,
    allFiles
};

public class Suggestion
{
    public string command;
    public string[] options;
    public string[] args;
    public string info;
    public DirType dirType = DirType.none;
}

public static partial class AutoComplete
{
    static readonly Suggestion[] suggestion =
    {
        new Suggestion
        {
            command = "help",
            info = "Returns available commands"
        },

        new Suggestion
        {
            command = "ls",
            options = new string[] { "--workspace", "--remote" },
            info = "ls --workspace | -w | --remote | -r"
        },

        new Suggestion
        {
            command = "config",
            options = new string[] { "--user", "--device", "--workspace", "--remote" },
            info = "[argument] --workspace | -w | --user | -u | --remote | -r"
        },

        new Suggestion
        {
            command = "init",
            dirType = DirType.workspaceDirs,
            info = "init [project name]"
        },

        new Suggestion
        {
            command = "push",
            dirType = DirType.workspaceDirs,
            info = "push [project]"
        },

        new Suggestion
        {
            command = "pull",
            dirType = DirType.remoteDirs,
            info = "pull -[rev no] [project]"
        },

        new Suggestion
        {
            command = "count",
            options = new string[] { "--status" },
            dirType = DirType.workspaceDirs,
            info = "count --status [project]"
        },

        new Suggestion
        {
            command = "checkin",
            options = new string[] { "--force" },
            dirType = DirType.workspaceDirs,
            info = "checkin --force | -f [project]"
        },

        new Suggestion
        {
            command = "checkout",
            dirType = DirType.remoteDirs,
            info = "checkout [project]"
        },

        new Suggestion
        {
            command = "status",
            options = new string[] { "--user", "--out" },
            info = "status --user | -u | --out | -o [name]",
            dirType = DirType.remoteDirs
        },

        new Suggestion
        {
            command = "log",
            info ="log [start date] [end date] [project]"
        },

        new Suggestion
        {
            command = "rm",
            dirType = DirType.workspaceDirs,
            info = "rm [project name]"
        },

        new Suggestion
        {
            command = "find",
            options = new string[] { "--workspace", "--remote", "--case" },
            info = "find --workspace | -r | --remote | -r | --case | -c [tag]"
        },

        new Suggestion
        {
            command = "tree",
            options = new string[] { "--workspace", "--remote" },
            info = "tree --workspace | -w |--remote | -r [project name]",
            dirType = DirType.allDirs
        },

        new Suggestion
        {
            command = "create"
        },

        new Suggestion
        {
            command = "clear",
            info = "Clear terminal console"
        },

        new Suggestion
        {
            command = "exit",
            info = "close FTM"
        },

        new Suggestion
        {
            command = "show",
            options = new string[] { "--workspace", "--remote" },
            info = "show --workspace | -w | --remote | -r [file/path]",
            dirType = DirType.remoteFiles
        },

        new Suggestion
        {
            command = "scan",
            options = new string[] { "--workspace", "--remote", "--case" },
            dirType = DirType.allDirs,
            info = "scan [project name] [tag]"
        },

        new Suggestion
        {
            command = "baseline",
            dirType = DirType.remoteDirs,
            info = "baseline [project name]"
        },

        new Suggestion
        {
            command = "diff",
            options = new string[] { "--workspace", "--remote"},
            dirType = DirType.allFiles,
            info = "diff --workspace | --remote | --other | -[rev] [file 1] [file 2]"
        },

        new Suggestion
        {
            command = "clone",
            args = new string[] { "https://github.com/" },
            info = "clone [github url]"
        }
    };
}
