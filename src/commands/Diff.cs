using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// This feature is experimental and can generate unreliable results.

public static partial class Command
{ 
    public static void Diff(string[] options, string[] args)
    {
        if (!Verification.ProfileIsValid())
            return;

        if (args.Length == 0)
        {
            Msg.InfoDiff();
            return;
        }

        if (args.Length > 2)
        {
            ErrorMsg.TooManyArgs();
            return;
        }

        string rootFileOld = null;
        string rootFileNew = null;

        int? previousVersionOld = null;
        int? previousVersionNew = null;

        bool isRemoteOld = false;
        bool isRemoteNew = false;

        foreach (string option in options)
        {
            if (option == "--workspace" || option == "-w")
            {
                if(rootFileOld == null)
                    rootFileOld = Program.settings.Read("workspace") + "/";
                else
                    rootFileNew = Program.settings.Read("workspace") + "/";
            }
            else if (option == "--remote" || option == "-r")
            {
                if (rootFileOld == null)
                {
                    rootFileOld = Program.settings.Read("remote") + "/";
                    isRemoteOld = true;
                }
                else
                {
                    rootFileNew = Program.settings.Read("remote") + "/";
                    isRemoteNew = true;
                }
            }
            else if (option == "--other" || option == "-o")
            {
                if (rootFileOld == null)
                    rootFileOld = "";
                else
                    rootFileNew = "";
            }
            else
            {
                // Check if option is version notation (for example -3)
                if (int.TryParse(option, out int revisionStamp))
                {
                    if (previousVersionOld == null)
                        previousVersionOld = revisionStamp;
                    else
                        previousVersionNew = revisionStamp;
                }
            }
        }

        if (previousVersionNew == null)
            previousVersionNew = 0;

        if (previousVersionOld == null)
            previousVersionOld = 0;

        if (rootFileNew == null)
            rootFileNew = rootFileOld;

        string relPathOld = null;
        string relPathNew = null;

        foreach(string arg in args)
        {
            if (relPathOld == null)
                relPathOld = arg;
            else
                relPathNew = arg;
        }

        if (relPathNew == null)
        {
            relPathNew = relPathOld;
            isRemoteNew = isRemoteOld;
        }

        string absPathOld = rootFileOld + relPathOld;
        string absPathNew = rootFileNew + relPathNew;

        Console.WriteLine(absPathOld + " " + previousVersionOld);
        Console.WriteLine(absPathNew + " " + previousVersionNew);

        string oldText = (isRemoteOld)
            ? GetFileRemote(absPathOld, (int)previousVersionOld)
            : File.ReadAllText(absPathOld);

        string newText = (isRemoteNew)
            ? GetFileRemote(absPathNew, (int)previousVersionNew)
            : File.ReadAllText(absPathNew);

        if (newText == null || oldText == null)
            return;

        List<SubText> compareResult = GetLCScollection(oldText, newText);
        
        foreach(SubText result in compareResult)
        {
            switch(result.blockType)
            {
                case BlockType.AddedText:
                    Print.WriteBlueWord(result.text);
                    break;

                case BlockType.RemovedText:
                    Print.WriteRedWord(result.text);
                    break;

                case BlockType.LCStext:
                    Console.Write(result.text);
                    break;
            }
        }

        Console.WriteLine();
    }

    private static string GetFileRemote(string path, int previousVersion)
    {
        Console.WriteLine(previousVersion);
        
        if (!Directory.Exists(path))
        {
            Print.ErrorMessage("Path does not exist.");
            return null;
        }

        string[] files = Directory.GetFiles(path);

        try
        {
            Console.WriteLine(files[files.Length + previousVersion - 1]);
            string content = File.ReadAllText(files[files.Length + previousVersion - 1]);
            return content;
        }
        catch
        {
            Print.ErrorMessage("Could not read file.");
        }

        return null;
    }

    private static List<SubText> GetLCScollection(string _oldText, string _newText)
    {
        SubText oldText = new SubText
        {
            text = _oldText,
            newStartPos = 0,
            oldStartPos = 0
        };

        SubText newText = new SubText
        {
            text = _newText,
            newStartPos = 0,
            oldStartPos = 0
        };

        List<SubText> newTextBlocks = new List<SubText> { newText };
        List<SubText> oldTextBlocks = new List<SubText> { oldText };
        List<SubText> lcsBlocks = new List<SubText>();

        int oldPos = -1;
        int newPos = -1;

        while (true)
        {
            SubText foundLCS = new SubText { text = ""};
            for (int i = 0; i < newTextBlocks.Count && i < oldTextBlocks.Count; i++)
            {
                SubText tempLCS = GetLCS(oldTextBlocks[i], newTextBlocks[i], 3);
                
                if (tempLCS.text.Length > foundLCS.text.Length)
                {
                    foundLCS = tempLCS;
                    oldPos = i;
                    newPos = i;
                }
            }

            if (foundLCS.text.Length == 0)
                break;

            string[] newSplitTexts = new string[2];
            newSplitTexts[0] = newTextBlocks[newPos].text.Substring(0, foundLCS.newStartPos);
            newSplitTexts[1] = newTextBlocks[newPos].text.Substring(foundLCS.newStartPos + foundLCS.text.Length);

            int offset = 0;
            for(int i = 0; i < 2; i++)
            {
                if (newSplitTexts[i].Length > 0)
                {
                    SubText newSubText = new SubText
                    {
                        text = newSplitTexts[i],
                        oldStartPos = newTextBlocks[newPos].oldStartPos + offset,
                        newStartPos = newTextBlocks[newPos].newStartPos + offset
                    };

                    newTextBlocks.Add(newSubText);
                }
                offset = foundLCS.text.Length;
            }

            string[] oldSplitTexts = new string[2];
            oldSplitTexts[0] = oldTextBlocks[oldPos].text.Substring(0, foundLCS.oldStartPos);
            oldSplitTexts[1] = oldTextBlocks[oldPos].text.Substring(foundLCS.oldStartPos + foundLCS.text.Length);

            offset = 0;
            for (int i = 0; i < 2; i++)
            {
                if (oldSplitTexts[i].Length > 0)
                {
                    SubText oldSubText = new SubText
                    {
                        text = oldSplitTexts[i],
                        oldStartPos = oldTextBlocks[oldPos].oldStartPos + offset,
                        newStartPos = oldTextBlocks[oldPos].newStartPos + offset
                    };

                    oldTextBlocks.Add(oldSubText);
                }

                offset = foundLCS.text.Length;
            }

            foundLCS.newStartPos += newTextBlocks[newPos].newStartPos;
            foundLCS.oldStartPos += oldTextBlocks[oldPos].oldStartPos;

            oldTextBlocks.RemoveAt(oldPos);
            newTextBlocks.RemoveAt(newPos);

            oldTextBlocks = oldTextBlocks.OrderBy(f => f.oldStartPos).ToList();
            newTextBlocks = newTextBlocks.OrderBy(f => f.newStartPos).ToList();

            lcsBlocks.Add(foundLCS);
        }

        List<SubText> finalBlocks = new List<SubText>();

        foreach(SubText lcsBlock in lcsBlocks)
        {
            SubText block = lcsBlock;
            block.blockType = BlockType.LCStext;
            finalBlocks.Add(block);
        }

        foreach (SubText newTextBlock in newTextBlocks)
        {
            SubText block = newTextBlock;
            block.blockType = BlockType.AddedText;
            finalBlocks.Add(block);
        }

        foreach (SubText oldTextBlock in oldTextBlocks)
        {
            SubText block = oldTextBlock;
            block.blockType = BlockType.RemovedText;
            finalBlocks.Add(block);
        }

        finalBlocks = finalBlocks.OrderBy(f => f.newStartPos).ToList();

        return finalBlocks;
    }
         
    private static SubText GetLCS(SubText oldText, SubText newText, int MatchLength)
    {
        int oldLength = oldText.text.Length;
        int newLength = newText.text.Length;
        int foundLCSlength = 0;
        int endPosOld = 0;
        int endPosNew = 0;

        SubText foundLCS = new SubText { text = "" };

        for (int l = 0; l < newLength; l++)
        {
            for (int k = 0; k < oldLength; k++)
            {
                for (int j = endPosNew; j < newLength; j++)
                {
                    if (j < endPosNew)
                        j = endPosNew;

                    int newLen = newLength - j - l;
                    if (newLen < MatchLength)
                        break;

                    if (newLen <= foundLCSlength)
                        break;

                    for (int i = endPosOld; i < oldLength; i++)
                    {
                        if (endPosOld > i)
                            i = endPosOld;

                        int oldLen = oldLength - i - k;

                        if (oldLen <= foundLCSlength)
                            break;

                        if (oldLen != newLen)
                            continue;

                        string oldSub = oldText.text.Substring(i, oldLen);
                        string newSub = newText.text.Substring(j, newLen);

                        if (newSub != oldSub)
                            continue;

                        foundLCS.newStartPos = j;
                        foundLCS.oldStartPos = i;
                        foundLCS.text = newSub;

                        foundLCSlength = newLen;
                        endPosOld = i + newLen;
                        endPosNew = j + newLen;
                    }
                }
            }
        }

        return foundLCS;
    }

    struct SubText
    {
        public int newStartPos;
        public int oldStartPos;
        public string text;
        public BlockType blockType;
    }

    enum BlockType
    {
        AddedText = 0,
        RemovedText = 1,
        LCStext = 2
    }
}