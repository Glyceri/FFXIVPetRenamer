using Dalamud.Data;
using ImGuiNET;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;
using PetRenamer.Core;
using PetRenamer.Core.Serialization;
using System;
using System.Text;

namespace PetRenamer
{
    public class Utils
    {
        PetRenamerPlugin plugin;
        ExcelSheet<Companion> petSheet;



        public Utils(PetRenamerPlugin plugin, DataManager dataManager)
        {
            this.plugin = plugin;
            petSheet = dataManager.GetExcelSheet<Companion>()!;
        }

        public string GetCurrentPetName()
        {
            foreach (Companion pet in petSheet)
            {
                if (pet == null) continue;

                if (pet.Model.Value.Model == Globals.CurrentID)
                    return pet.Singular.ToString();
            }
            return string.Empty;
        }

        public bool Contains(int ID)
        {
            foreach(SerializableNickname nickname in plugin.Configuration.nicknames!)
            {
                if (nickname == null) continue;
                if(nickname.ID == ID) return true;
            }

            return false;
        }

        public SerializableNickname GetNickname(int ID)
        {
            for(int i = 0; i < plugin.Configuration.nicknames!.Length; i++)
            {
                if (plugin.Configuration.nicknames[i].ID == ID)
                    return plugin.Configuration.nicknames[i];
            }

            return null;
        }

        public string FromBytes(byte[] bytes) => Encoding.Default.GetString(bytes);

        public string GetName(int ID)
        {
            foreach (SerializableNickname nickname in plugin.Configuration.nicknames!)
                if (nickname.ID == ID) 
                    return nickname.Name;
            
            return string.Empty;
        }

        public byte[] GetBytes(string name)
        {
            byte[] bytes = new byte[64];
            if (name == string.Empty) return bytes;

            int smallestLength = Math.Min(name.Length, bytes.Length);
            for(int i = 0; i < smallestLength; i++)
                bytes[i] = (byte)name[i];

            return bytes;
        }

        public byte[] GetBytes(int ID) => GetBytes(GetName(ID));
    }
}
