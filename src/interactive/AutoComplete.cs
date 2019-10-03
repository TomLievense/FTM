using System;
using System.IO;
using System.Threading.Tasks;

// The AutoComplete Class is responsible for providing
// suggestion and information in the interactive mode.

public static partial class AutoComplete
{
    private static string[] remoteFiles = new string[0];
    static string[] RemoteFiles
    {
        get
        {
            Task.Run(() =>
            {
                string path = Program.settings.Read("remote");
                remoteFiles = Directory.GetFiles(path, "*", SearchOption.AllDirectories);

                for (int i = 0; i < remoteFiles.Length; i++)
                    remoteFiles[i] = Path.GetDirectoryName(remoteFiles[i].Substring(path.Length));
            });

            return remoteFiles;
        }
    }

    private static string[] remoteProjects = new string[0];
    static string[] RemoteProjects
    {
        get
        {
            Task.Run(() =>
            {
                string path = Program.settings.Read("remote");
                remoteProjects = Directory.GetDirectories(path);

                for (int i = 0; i < remoteProjects.Length; i++)
                    remoteProjects[i] = Path.GetFileName(remoteProjects[i]);
            });

            return remoteProjects;
        }
    }

    private static string[] workSpaceFiles = new string[0];
    static string[] WorkSpaceFiles
    {
        get
        {
            Task.Run(() =>
            {
                string path = Program.settings.Read("workspace");
                try
                {
                    workSpaceFiles = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
                }
                catch { }

                for (int i = 0; i < workSpaceFiles.Length; i++)
                    workSpaceFiles[i] = workSpaceFiles[i].Substring(path.Length);
            });

            return workSpaceFiles;
        }
    }

    private static string[] workSpaceProjects = new string[0];
    static string[] WorkSpaceProjects
    {
        get
        {
            Task.Run(() =>
            {
                string path = Program.settings.Read("workspace");
                try
                {
                    workSpaceProjects = Directory.GetDirectories(path);
                }
                catch { }

                for (int i = 0; i < workSpaceProjects.Length; i++)
                    workSpaceProjects[i] = Path.GetFileName(workSpaceProjects[i]);
            });

            return workSpaceProjects;
        }
    }

    static string currentInfo = "";
    static int currentInfoRow = 0; 
    private static void WriteInfo(Suggestion _suggestion)
    {
        int cursorPosTop = Console.CursorTop;
        int cursorPosLeft = Console.CursorLeft;
        int offset = Program.profileName.Length + 2;

        if (_suggestion.info == null ||
            Console.WindowWidth <= offset + _suggestion.info.Length)
            return;

        Console.SetCursorPosition(offset, cursorPosTop + 1);
        Console.BackgroundColor = ConsoleColor.DarkRed;
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(_suggestion.info);
        Console.ResetColor();
               
        Console.SetCursorPosition(cursorPosLeft, cursorPosTop);

        currentInfo = _suggestion.info;
        currentInfoRow = cursorPosTop + 1;
    }

    private static void WriteSuggestion(string currentSuggestion)
    {
        if(currentSuggestion.Length < matchLength)
            return;

        int cursorPos = Console.CursorLeft;
        int cursorPosTop = Console.CursorTop;

        Console.CursorVisible = false;
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.Write(currentSuggestion.Substring(matchLength));
        Console.ResetColor();
        Console.SetCursorPosition(cursorPos, cursorPosTop);
        Console.CursorVisible = true;
    }

    private static void ClearLine(string input)
    {
        int offset = Program.profileName.Length + 2;
        int offsetTop = (offset + input.Length - 1) / Console.WindowWidth;

        int cursorPosTop = (Console.CursorTop - offsetTop > 0)
            ? Console.CursorTop - offsetTop
            : Console.CursorTop;

        Console.CursorVisible = false;
        Console.SetCursorPosition(offset, cursorPosTop);

        if(offsetTop == 0)
            Console.WriteLine(new string(' ', Console.WindowWidth * 2 - offset));
        else
            Console.WriteLine(new string(' ', input.Length));

        if (Console.CursorTop == cursorPosTop)
            cursorPosTop -= 2;

        Console.SetCursorPosition(offset, cursorPosTop);
        Console.CursorVisible = true;
    }

    static int matchLength;
    static string oldSuggestion = "";
    private static string GetSuggestion(string input)
    {
        string whiteSpace = "";
        string[] chunks = input.Split(' ');
        if (chunks.Length == 1 && !chunks[0].StartsWith(':'))
        {
            for (int i = 0; i < suggestion.Length; i++)
            {
                if (suggestion[i].command.StartsWith(chunks[0], StringComparison.OrdinalIgnoreCase))
                {
                    if(oldSuggestion != suggestion[i].command)
                    {
                        ClearLine(input);
                        Console.Write(input);
                        oldSuggestion = suggestion[i].command;
                    }

                    matchLength = chunks[0].Length;
                    WriteInfo(suggestion[i]);
                    WriteSuggestion(suggestion[i].command);
                                       
                    return suggestion[i].command;
                }
            }
            if(oldSuggestion != "")
            {
                ClearLine(input);
                Console.Write(input);
                oldSuggestion = "";
            }
            return "";
        }

        string lastChunk = chunks[chunks.Length - 1];

        if (lastChunk.StartsWith(":", StringComparison.OrdinalIgnoreCase))
        {
            string alias = Aliases.GetSuggestion(lastChunk);
            if (alias != "")
            {
                matchLength = lastChunk.Length;
                WriteSuggestion(alias);
                return alias;
            }
        }
                
        Suggestion currentSuggestion = new Suggestion();
        bool suggestionFound = false;

        for (int i = 0; i < suggestion.Length; i++)
        {
            if (suggestion[i].command == chunks[0])
            {
                matchLength = lastChunk.Length;
                if ((input.Length + Program.profileName.Length + 2) < Console.WindowWidth)
                    WriteInfo(suggestion[i]);

                currentSuggestion = suggestion[i];
                suggestionFound = true;
                break;
            }
        }

        if (suggestionFound)
        {   
            if (lastChunk.StartsWith("-", StringComparison.OrdinalIgnoreCase)
                && currentSuggestion.options != null)
            {
                for (int i = 0; i < currentSuggestion.options.Length; i++)
                {
                    if (currentSuggestion.options[i].StartsWith
                        (lastChunk, StringComparison.OrdinalIgnoreCase))
                    {
                        if(oldSuggestion != currentSuggestion.options[i])
                        {
                            ClearLine(input);
                            Console.Write(input);
                            oldSuggestion = currentSuggestion.options[i];
                        }

                        matchLength = lastChunk.Length;
                        WriteSuggestion(currentSuggestion.options[i]);

                        return currentSuggestion.options[i];
                    }
                }
            }
            else if (currentSuggestion.dirType != DirType.none)
            {
                string[] pathSuggestions;
                switch(currentSuggestion.dirType)
                {
                    case DirType.remoteDirs:
                        pathSuggestions = RemoteProjects;
                        break;

                    case DirType.workspaceDirs:
                        pathSuggestions = WorkSpaceProjects;
                        break;

                    case DirType.allDirs:
                        string[] _remoteProjects = RemoteProjects;
                        string[] _workSpaceProjects = WorkSpaceProjects;

                        int lenRemote = remoteProjects.Length;
                        int lenWorkSpace = workSpaceProjects.Length;

                        pathSuggestions = new string[lenRemote + lenWorkSpace];

                        for(int i = 0; i < lenRemote; i++)
                            pathSuggestions[i] = _remoteProjects[i];

                        for (int i = 0; i < lenWorkSpace; i++)
                            pathSuggestions[(lenRemote > 0) ? i + lenRemote -1 : i] = _workSpaceProjects[i];

                        break;

                    case DirType.remoteFiles:
                        pathSuggestions = RemoteFiles;
                        break;

                    case DirType.workspaceFiles:
                        pathSuggestions = WorkSpaceFiles;
                        break;

                    default:
                        string[] _remoteFiles = RemoteFiles;
                        string[] _workSpaceFiles = WorkSpaceFiles;

                        lenRemote = remoteFiles.Length;
                        lenWorkSpace = workSpaceFiles.Length;

                        pathSuggestions = new string[lenRemote + lenWorkSpace];

                        for (int i = 0; i < lenRemote; i++)
                            pathSuggestions[i] = _remoteFiles[i];

                        for (int i = 0; i < lenWorkSpace; i++)
                            pathSuggestions[(lenRemote > 0) ? i + lenRemote - 1 : i] = _workSpaceFiles[i];

                        break;
                }

                for (int i = 0; i < pathSuggestions.Length; i++)
                {
                    if (pathSuggestions[i] == null)
                        continue;

                    if (pathSuggestions[i].StartsWith
                        (lastChunk, StringComparison.OrdinalIgnoreCase))
                    {
                        if(oldSuggestion != pathSuggestions[i])
                        {
                            ClearLine(input);
                            Console.Write(input);
                            oldSuggestion = pathSuggestions[i];
                        }

                        matchLength = lastChunk.Length;
                        WriteSuggestion(pathSuggestions[i]);

                        return pathSuggestions[i];
                    }
                }
            }

            if (currentSuggestion.args != null && lastChunk.Length > 1)
            {
                for(int i = 0; i < currentSuggestion.args.Length; i++)
                {
                    if (currentSuggestion.args[i].StartsWith(lastChunk, StringComparison.OrdinalIgnoreCase))
                    {
                        if(oldSuggestion != currentSuggestion.args[i])
                        {
                            ClearLine(input);
                            Console.Write(input);
                            oldSuggestion = currentSuggestion.args[i];
                        }
                        
                        matchLength = lastChunk.Length;
                        WriteSuggestion(currentSuggestion.args[i]);

                        return currentSuggestion.args[i];
                    }
                }
            }
        }

        if(currentSuggestionText.Length > 0)
        {
            whiteSpace = new string(' ', currentSuggestionText.Length);
            WriteSuggestion(whiteSpace);
            oldSuggestion = "";
        }

        return "";
    }

        static string input = "";
        static string currentSuggestionText = "";
    public static string GetInput()
    {
        input = "";
        currentSuggestionText = "";

        while (true)
        {
            ConsoleKeyInfo inputKey = Console.ReadKey(true);

            if (inputKey.Key == ConsoleKey.Enter)
            {
                ClearLine(input);

                if (input.Contains(":"))
                    input = Aliases.GetDefinition(input);

                Console.Write(input);
                Console.WriteLine();

                if (currentSuggestionText.Length > input.Length)
                    return currentSuggestionText;

                return input;
            }

            if (inputKey.Key == ConsoleKey.Tab
                && currentSuggestionText != "")
            {
                // If the suggestions contains spaces, quotes will be added automatically.
                if (currentSuggestionText.Contains(" "))
                    currentSuggestionText = "\"" + currentSuggestionText + "\"";

                ClearLine(input);

                if (input.Length > 0)
                    input = input.Substring(0, input.Length - matchLength) + currentSuggestionText;

                currentSuggestionText = "";
                Console.Write(input);

                continue;
            }
            
            if (inputKey.Key == ConsoleKey.Backspace)
            {
                if (input.Length > 0)
                {
                    int cursorPos = Console.CursorLeft;
                    int cursorPosTop = Console.CursorTop;
                    int offset = Program.profileName.Length + 2;
                    
                    Console.SetCursorPosition(cursorPos - 1, cursorPosTop);
                    Console.Write(' ');
                    Console.SetCursorPosition(cursorPos - 1, cursorPosTop);

                    input = input.Substring(0, input.Length - 1);
                    if (input.Length > 0)
                        currentSuggestionText = GetSuggestion(input);
                    else
                        ClearLine(input);
                }

                continue;
            }

            if (char.IsControl(inputKey.KeyChar))
                continue;

            input += inputKey.KeyChar;
            Console.Write(inputKey.KeyChar);

            if (input.Length > 0)
                currentSuggestionText = GetSuggestion(input);
        }
    }
}
