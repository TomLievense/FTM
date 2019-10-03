using System;


public static class Print
{
    private static void Write(string content)
    {
        Console.WriteLine(content);
    }

    public static void List(string[] items)
    {
        Console.ForegroundColor = ConsoleColor.Green;

        Write("*");

        for (int i = 0; i < items.Length; i++)
        {
            string prefix;

            if (i + 1 == items.Length)
                prefix = "└─ ";
            else
                prefix = "├─ ";

            Write(prefix + items[i]);
        }

        Write("");

        Console.ResetColor();
    }

    public static void Message(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public static void WriteRedWord(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write(message);
        Console.ResetColor();
    }

    public static void WriteBlueWord(string message)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write(message);
        Console.ResetColor();
    }

    public static void ErrorMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}
