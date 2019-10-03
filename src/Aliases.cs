using System;
using System.Collections.Generic;
using System.IO;

static class Aliases
{
    private struct Alias
    {
        public string name;
        public string definition;
    }

    static Alias[] aliases;
    
    static public string GetSuggestion(string chunk)
    {
        if (aliases == null)
            aliases = GetAliases();

        foreach (Alias alias in aliases)
        {
            if (alias.name.StartsWith(chunk, StringComparison.InvariantCultureIgnoreCase))
                return alias.name;
        }
        return "";
    }

    static public string GetDefinition(string input)
    {
        if(aliases == null)
            aliases = GetAliases();

        string absInput = input;
        foreach (Alias alias in aliases)
        {
            if (input.Contains(alias.name))
                absInput = input.Replace(alias.name, alias.definition);
        }

        return absInput;
    }
    
    static private Alias[] GetAliases()
    {
        List<Alias> aliases = new List<Alias>();
        string ignoreConfigPath = AppDomain.CurrentDomain.BaseDirectory + "config/aliases.ftm";

        if (!File.Exists(ignoreConfigPath))
            return null;

        string[] configLines = File.ReadAllLines(ignoreConfigPath);
        bool inBody = false;

        foreach (string line in configLines)
        {
            if (line.StartsWith("Aliases", StringComparison.Ordinal) &&
               line.EndsWith("{", StringComparison.Ordinal))
            {
                inBody = true;
                continue;
            }

            if (line.StartsWith("}", StringComparison.Ordinal) && inBody)
                break;

            if (inBody)
            {
                string[] split = line.Split("=");
                Alias alias = new Alias
                {
                    name = ":" + split[0].Trim(),
                    definition = split[1].Trim()
                };

                aliases.Add(alias);
            }
        }

        return aliases.ToArray();
    }
}
