using System;


public static class Msg
{
    public static void InfoInit()
    {
        PrintMsg("usage: init [project]");
    }

    public static void InfoScan()
    {
        PrintMsg("usage: scan [ --case | -c | --workspace | --remote | -r ] [project] [expression]");
    }

    public static void InfoBaseline()
    {
        PrintMsg("usage: baseline [project] [description]");
    }

    public static void InfoDiff()
    {
        PrintMsg("usage: diff [ --workspace | -w | --remote | -r ] [file1] [file2]");
    }

    public static void InfoCreate()
    {
        PrintMsg("usage: create [path] [content]");
    }

    public static void InfoCount()
    {
        PrintMsg("usage: count [project]");
    }

    public static void InfoTree()
    {
        PrintMsg("usage: [ --workspace | -w | --remote | -r ] [project]");
    }

    public static void InfoConfig()
    {
        PrintMsg("usage: config [ --workspace | -w | --remote | -r | --user | -u | --device | -d ] [input]");
    }

    public static void InfoLs()
    {
        PrintMsg("usage: ls [ --workspace | -w | --remote | -r ]");
    }
    
    public static void InfoPull()
    {
        PrintMsg("usage: pull [ -[digit] ] [project name]");
    }

    public static void InfoCheckIn()
    {
        PrintMsg("usage: checkin [project name]");
    }

    public static void InfoCheckOut()
    {
        PrintMsg("usage: checkout [ -[digit] ] [project name]");
    }

    public static void InfoPush()
    {
        PrintMsg("usage: push [project name]");
    }

    public static void InfoStatus()
    {
        PrintMsg("usage: status [ --user | -u | -o | --out ] [project/name]");
    }

    public static void InfoLog()
    {
        PrintMsg("usage: log [start date] [end date] [project]");
    }

    public static void InfoFind()
    {
        PrintMsg("usage: find [ --workspace | -w | --remote -r | --case | -c ] [expression]");
    }

    public static void ProjectUpToDate()
    {
        PrintMsg("Project up-to-date.");
    }

    private static void PrintMsg(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}