using System;


public static partial class Command
{
    public static void Clear(string[] options, string[] args)
    {
        if (args.Length > 0)
        {
            ErrorMsg.TooManyArgs();
            return;
        }

        if (options.Length > 0)
        {
            ErrorMsg.NoOptions();
            return;
        }

        Console.Clear();
    }
}