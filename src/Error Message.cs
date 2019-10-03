using System;


public static class ErrorMsg
{
    public static void QuotationMarkMissing()
    {
        PrintErrorMsg("Closing quotation mark is missing.");
    }

    public static void UnknownCommand()
    {
        PrintErrorMsg("Command Unknown.");
    }

    public static void TooManyArgs()
    {
        PrintErrorMsg("Too many arguments for this command.");
    }

    public static void TooManyOptions()
    {
        PrintErrorMsg("Too many options for this command.");
    }

    public static void UnknownArg()
    {
        PrintErrorMsg("Unknown argument for this command.");
    }

    public static void UnknownOption()
    {
        PrintErrorMsg("Unknown option for this command.");
    }

    public static void NoArgs()
    {
        PrintErrorMsg("No arguments given.");
    }

    public static void NoOptions()
    {
        PrintErrorMsg("No options given.");
    }

    public static void ProfileNotConfigured()
    {
        PrintErrorMsg("Profile not completely configured.");
    }

    public static void InputMissing()
    {
        PrintErrorMsg("Input argument is missing.");
    }

    public static void PathIncorrect()
    {
        PrintErrorMsg("Path does not exist.");
    }

    public static void ProjectIncorrect()
    {
        PrintErrorMsg("Project does not exist.");
    }

    public static void ProjectAlreadyCheckedIn()
    {
        PrintErrorMsg("Project already checked in.");
    }

    public static void ProjectAlreadyCheckedOut()
    {
        PrintErrorMsg("Project already checked out.");
    }

    public static void ProjectCheckedOutByOtherDevice()
    {
        PrintErrorMsg("Project is checked out by another device.");
    }

    public static void ProjectCheckedOutByOtherUsr()
    {
        PrintErrorMsg("Project is checked out by another user.");
    }

    private static void PrintErrorMsg(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}