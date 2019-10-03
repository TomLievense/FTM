using System;
using System.Net;
using System.IO;
using System.IO.Compression;


public static partial class Command
{
    public static void Clone(string[] options, string[] args)
    {
        if (!Verification.ProfileIsValid())
            return;

        if (args.Length == 0)
        {
            Msg.InfoCount();
            return;
        }

        if (args.Length > 1)
        {
            ErrorMsg.TooManyArgs();
            return;
        }

        if (options.Length > 0)
        {
            ErrorMsg.TooManyOptions();
            return;
        }

        string url = args[0];
        string name = Path.GetFileNameWithoutExtension(url);
        string workspacePath = Program.settings.Read("workspace");

        if (url.EndsWith(".git", StringComparison.OrdinalIgnoreCase))
            url = url.Replace(".git", @"/archive/master.zip");

        string outPath = workspacePath + "/~webpac.zip";
        WebClient webClient = new WebClient();

        try
        {
            webClient.DownloadFile(url, outPath);

            ZipFile.ExtractToDirectory(outPath, workspacePath, true);
            Directory.Move(workspacePath + "/" + name + "-master", workspacePath + "/" + name);
            ProjectConfig.Create(workspacePath + "/" + name + "/.ftm");
        }
        catch
        {
            Print.ErrorMessage("Could not clone project.");
            return;
        }

        try
        {
            File.Delete(outPath);
        }
        catch { }
    }
}