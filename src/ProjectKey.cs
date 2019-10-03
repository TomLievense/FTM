using System;
using System.Collections.Generic;
using System.IO;


public static class ProjectConfig
{
    public static bool Create(string path)
    {
        Random rnd = new Random();
        char[] _key = new char[64];

        for (int i = 0; i < 64; i++)
        {
            switch (rnd.Next(0, 3))
            {
                case 0:
                    _key[i] = (char)rnd.Next(48, 57);
                    break;

                case 1:
                    _key[i] = (char)rnd.Next(65, 90);
                    break;

                case 2:
                    _key[i] = (char)rnd.Next(97, 122);
                    break;
            }
        }

        string key = new string(_key);
        string[] defaultIgnores = GetDefaultIngore();
        string ftmConfig = "";

        ftmConfig+= "Identifier<" + key + ">" + "\r\n\r\n";
        ftmConfig += "Ignore {\r\n";

        if (defaultIgnores != null)
        {
            foreach (string ignore in defaultIgnores) {
                ftmConfig += ignore + "\r\n";
            }
        }

        ftmConfig += "}";

        try
        {
            File.WriteAllText(path,ftmConfig);
            return true;

        }
        catch
        {
            return false;
        }
    }

    static private string[] GetDefaultIngore()
    {
        List<string> defaultIgnore = new List<string>();

        string ignoreConfigPath = AppDomain.CurrentDomain.BaseDirectory + "/ignore.txt";

        if (!File.Exists(ignoreConfigPath))
            return null;
       
        string[] configLines = File.ReadAllLines(ignoreConfigPath);
        bool inBody = false;

        foreach (string line in configLines)
        {
            if (line.StartsWith("Ignore", StringComparison.Ordinal) &&
               line.EndsWith("{", StringComparison.Ordinal))
            {
                inBody = true;
                continue;
            }

            if (line.StartsWith("}", StringComparison.Ordinal) && inBody)
                break;

            if (inBody)
                defaultIgnore.Add(line);
        }

        return defaultIgnore.ToArray();
    }
}