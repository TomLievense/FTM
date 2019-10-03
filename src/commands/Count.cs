using System;
using System.Collections.Generic;
using System.IO;


public static partial class Command
{
    public static void Count(string[] options, string[] args)
    {
        if (!Verification.ProfileIsValid())
            return;

        if (args.Length == 0 || args[0] == null)
        {
            Msg.InfoCount();
            return;
        }
        
        if (args.Length > 1)
        {
            ErrorMsg.TooManyArgs();
            return;
        }

        if (args.Length < 1)
        {
            ErrorMsg.NoArgs();
            return;
        }

        if (options.Length > 1)
        {
            ErrorMsg.NoOptions();
            return;
        }

        bool log = false;
        foreach(string option in options)
        {
            switch (option)
            {
                case "-s":
                case "--status":
                    log = true;
                    break;

                default:
                    ErrorMsg.UnknownOption();
                    return;
            }
        }

        string project = args[0];
        string workspacePath = Program.settings.Read("workspace");
        string path = workspacePath + "/" + project;

        if(!Directory.Exists(path))
        {
            ErrorMsg.ProjectIncorrect();
            return;
        }

        CountWorkSpace(path, log);
    }

    private static void CountWorkSpace(string projectPath, bool log)
    {
        Language[] languages = GetLanguages();
        string[] files = Directory.GetFiles(projectPath, "*.*", SearchOption.AllDirectories);

        foreach (string file in files)
        {
            for (int i = 0; i < languages.Length; i++)
            {
                foreach (string extension in languages[i].extension)
                {
                    if (file.ToLower().EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                    {
                        if (log)
                        {
                            string relativePath = file.Substring(projectPath.Length);
                            Print.Message("Reading '" + relativePath + "'");
                        }

                        string[] lines = File.ReadAllLines(file);
                        bool isBlockComment = false;
                        languages[i].files++;

                        foreach (string line in lines)
                        {
                            string cleanLine = line.Replace("\t", "");
                            cleanLine = cleanLine.Replace(" ", "");

                            if (languages[i].openComment != null && cleanLine.Contains(languages[i].openComment))
                                isBlockComment = true;
             
                            if (cleanLine == "")
                            {
                                languages[i].emptyLines++;
                                continue;
                            }

                            if (isBlockComment || (languages[i].lineComment != null && languages[i].lineComment != null &&
                                cleanLine.StartsWith(languages[i].lineComment, StringComparison.Ordinal)))
                            {
                                if (languages[i].closeComment != null && cleanLine.Contains(languages[i].closeComment))
                                    isBlockComment = false;

                                languages[i].commentLines++;
                                continue;
                            }
                            
                            languages[i].filledLines++;
                        }
                    }
                }
            }
        }

        int sumCode = 0;
        int sumBlank = 0;
        int sumComment = 0;
        int sumFiles = 0;

        foreach(Language language in languages)
        {
            if(language.emptyLines > 0 || language.commentLines > 0 || language.filledLines > 0)
            {
                sumCode += language.filledLines;
                sumBlank += language.emptyLines;
                sumComment += language.commentLines;
                sumFiles += language.files;

                Print.Message(language.name);
                Print.Message("Code: " + language.filledLines);
                Print.Message("Blank: " + language.emptyLines);
                Print.Message("Comment: " + language.commentLines);
                Print.Message("Files: " + language.files + "\n");
            }
        }

        Print.Message("<- SUM ->");
        Print.Message("Code: " + sumCode);
        Print.Message("Blank: " + sumBlank);
        Print.Message("Comment: " + sumComment);
        Print.Message("Files: " + sumFiles + "\n");
    }

    private static Language[] GetLanguages()
    {
        string langConfigPath = AppDomain.CurrentDomain.BaseDirectory + "config/languages.ftm";
        string[] configLine = File.ReadAllLines(langConfigPath);

        List<Language> languages = new List<Language>();
        Language language = new Language();

        for (int i = 0; i < configLine.Length; i++)
        {
            if(configLine[i].EndsWith("{", StringComparison.Ordinal))
            {
                language.name = configLine[i].Substring(0, configLine[i].Length - 2).Trim();
                continue;
            }

            if (configLine[i].EndsWith("}", StringComparison.Ordinal))
            {
                languages.Add(language);
                language = new Language();
                continue;
            }

            if (configLine[i].Trim().StartsWith("extension", StringComparison.Ordinal))
            {
                string[] valSplit = configLine[i].Split(':');
                string[] extension = valSplit[1].Split(',');

                for(int j = 0; j < extension.Length; j++)
                    extension[j] = extension[j].Trim();

                language.extension = extension;

                continue;
            }

            if (configLine[i].Trim().StartsWith("line_comment", StringComparison.Ordinal))
            {
                string[] valSplit = configLine[i].Split(':');
                language.lineComment = valSplit[1].Trim();

                continue;
            }

            if (configLine[i].Trim().StartsWith("open_comment", StringComparison.Ordinal))
            {
                string[] valSplit = configLine[i].Split(':');
                language.openComment = valSplit[1].Trim();

                continue;
            }

            if (configLine[i].Trim().StartsWith("close_comment", StringComparison.Ordinal))
            {
                string[] valSplit = configLine[i].Split(':');
                language.closeComment = valSplit[1].Trim();

                continue;
            }
        }

        return languages.ToArray();
    }

    private class Language
    {
        public string[] extension;
        public string name;
        public string lineComment;
        public string openComment;
        public string closeComment;

        public int emptyLines;
        public int filledLines;
        public int commentLines;
        public int files;
    }
}
