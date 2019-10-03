using System;
using System.Collections.Generic;

// Input commands must have the following (Unix-like) format: 
// Command [Options...] [Arguments...]

// Due to the design of the "tokenizer", the position of [Options] 
// and [arguments] can be swapped.

// Options must have the prefix '-' character and can also be combined.

// The following input examples have the same result:
// status --user --out tom
// status -u -o tom
// status -ou tom
// status tom -uo

// Quotations has to be used if an argument contains spaces.

static class Parser
{
    public static void Tokenize(string input)
    {
        string command = null;

        List<string> args = new List<string>();
        List<string> options = new List<string>();

        char[] token = input.ToCharArray();
        string word = null;
        bool isQuoted = false;

        void DetermineType()
        {
            if (word == null || word.Length < 1)
                return;

            if (command == null)
            {
                command = word;
            }
            else if (word.Substring(0, 1) == "-")
            {
                // Check if options are combined:
                // Options can also be integers with more than one digit.
                // Example: pull -10 myproject. Without applying the TryParse method,
                // the value -10 will be split into -1 and -0.
                if (word.Length > 2 && word.Substring(0, 2) != "--" &&
                        !int.TryParse(word, out int tryResult))
                {
                    char[] split = word.ToCharArray();

                    for (int i = 1; i < split.Length; i++)
                    {
                        string newOption = "-" + split[i];
                        options.Add(newOption);
                    }

                }
                else
                {
                    options.Add(word);
                }
            }
            else
            {
                args.Add(word);
            }
            word = null;
        }

        for (int i = 0; i < token.Length; i++)
        {
            if ((token[i] == ' ' || token[i] == '\t') && !isQuoted)
            {
                DetermineType();
                continue;
            }

            if (token[i] == '"')
                isQuoted = !isQuoted;
            else
                word += token[i];
        }

        // Word can be null if input contains a space/tab at the end or
        // a double space/tab between the words.
        if (word != null) DetermineType();

        if (isQuoted) // Check if there is also a closing tag.
        {
            ErrorMsg.QuotationMarkMissing();
            return;
        }
        Parse(command, options.ToArray(), args.ToArray());
    }

    static void Parse(string command, string[] options, string[] args)
    {
        switch (command)
        {
            case "ls":
                Command.GetList(options, args);
                break;

            case "tree":
                Command.Tree(options, args);
                break;

            case "help":
                Command.Help(options, args);
                break;

            case "config":
                Command.Config(options, args);
                break;

            case "pull":
                Command.Pull(options, args, false);
                break;

            case "push":
                Command.Push(options, args, false);
                break;

            case "checkin":
                Command.Push(options, args, true);
                break;

            case "checkout":
                Command.Pull(options, args, true);
                break;

            case "status":
                Command.GetStatus(options, args);
                break;

            case "log":
                Command.GetLog(options, args);
                break;

            case "rm":
                Command.Delete(options, args);
                break;

            case "find":
                Command.Find(options, args);
                break;

            case "scan":
                Command.Scan(options, args);
                break;

            case "create":
                Command.Create(options, args);
                break;

            case "show":
                Command.Show(options, args);
                break;

            case "count":
                Command.Count(options, args);
                break;

            case "clear":
                Command.Clear(options, args);
                break;

            case "diff":
                Command.Diff(options, args);
                break;

            case "baseline":
                Command.Baseline(options, args);
                break;

            case "init":
                Command.Init(options, args);
                break;

            case "clone":
                Command.Clone(options, args);
                break;

            case "exit":
                Environment.Exit(0);
                break;

            default:
                ErrorMsg.UnknownCommand();
                break;
        }
    }
}

