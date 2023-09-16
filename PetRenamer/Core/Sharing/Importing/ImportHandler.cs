using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Core.Sharing.Importing.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetRenamer.Core.Sharing.Importing;

public static class ImportHandler
{
    public static ImportData SetImportString(string importString)
    {
        PettableUser localUser = PluginLink.PettableUserHandler.LocalUser()!;
        if (localUser == null) return new FailedImportData("Local user not found");

        string gottenText;

        try {
            gottenText = Encoding.Unicode.GetString(Convert.FromBase64String(importString));
        }catch(Exception e) { return new FailedImportData(e.Message); }

        if (gottenText == null) return new FailedImportData("Imported Text came back as Null");
        if (gottenText.Length == 0) return new FailedImportData("Imported Text was empty");
        if (!gottenText.StartsWith("[PetExport]")) return new FailedImportData("Imported Text was not a [PetExport]");

        try
        {
            string[] splitLines = gottenText.Split('\n');
            if (splitLines.Length <= 2) return new FailedImportData("Splitlines was not of length 2");
            try
            {
                string userName = splitLines[1];
                ushort homeWorld = ushort.Parse(splitLines[2]);

                List<int> ids = new List<int>();
                List<string> names = new List<string>();
                try
                {
                    for (int i = 3; i < splitLines.Length; i++)
                    {
                        string[] splitNickname = splitLines[i].Split('^');
                        if (splitNickname.Length < 1) continue;
                        if (!int.TryParse(splitNickname[0].Replace("ID:", ""), out int ID)) { continue; }
                        string nickname = splitNickname[1].Replace("Name:", "");
                        ids.Add(ID);
                        names.Add(nickname);
                    }
                }
                catch (Exception e) { return new FailedImportData($"{e}"); }

                return new SucceededImportData(userName, homeWorld, ids.ToArray(), names.ToArray());
            }
            catch (Exception e) { return new FailedImportData($"{e}"); }
        }
        catch (Exception e) { return new FailedImportData($"{e}"); }
    }
}
