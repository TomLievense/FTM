using System.Collections.Generic;
using System.IO;


public class Configurator
{
    private readonly string path;

    public Configurator(string path)
    {
        this.path = path;
    }

    private readonly List<string> settings = new List<string>();

    public string Read(string key)
    {
        if (settings.Count == 0)
        {
            try
            {
                settings.AddRange(File.ReadAllLines(path));
            }
            catch
            {
                return null;
            }
        }

        foreach (string setting in settings)
        {
            int settingLength = setting.Length;
            int keyLength = key.Length;

            if (keyLength + 1 > settingLength)
                continue;

            if (setting.Substring(0, keyLength) == key)
            {
                string[] _result = setting.Split('"');

                string result = "";
                for (int i = 1; i < _result.Length; i++)
                    result += _result[i];
               
                return result;
            }
        }
        return null;
    }

    public void Write(string key, string value)
    {
        int lengthKey = key.Length;
        string newSetting = key + @" = """ + value + @"""";

        for (int i = 0; i < settings.Count; i++)
        {
            int lSetting = settings[i].Length;

            if (lengthKey + 1 > lSetting)
                continue;

            if (settings[i].Substring(0, lengthKey) == key)
            {
                settings[i] = newSetting;
                return;
            }
        }
        settings.Add(newSetting);
    }

    public bool Save()
    {
        if (settings.Count == 0)
            return false;

        try
        {
            File.WriteAllLines(path, settings);
            return true;
        }
        catch
        {
            return false;
        }
    }
}