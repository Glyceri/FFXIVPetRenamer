using Dalamud.Game;
using Dalamud.Utility;
using Lumina.Excel.Sheets;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Statics;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Factory;

internal static class PetSheetDataFactory
{ 
    private static readonly string[] pronounList = ["er", "e", "es", "en"];
    
    public static PetSheetData? CreatePetSheetData(DalamudServices dalamudServices, Companion companion)
    {
        if (!companion.Model.IsValid)
        {
            return null;
        }

        ModelChara? model = companion.Model.ValueNullable;

        if (model == null)
        {
            return null;
        }
        
        int         modelId        = (int)model.Value.RowId;
        int         legacyModelId  = model.Value.Model;
        string      singular       = companion.Singular.ExtractText();
        PetSkeleton petSkeleton    = new PetSkeleton((uint)modelId, SkeletonType.Minion);
        
        if (legacyModelId == 0)
        {
            return null;
        }
        
        if (singular.IsNullOrWhitespace())
        {
            return null;
        }

        singular = singular.ToTitleCase();
        
        uint   icon          = companion.Icon;
        sbyte  pronoun       = companion.Pronoun;
        uint   raceId        = companion.MinionRace.ValueNullable?.RowId ?? 0;
        string raceName      = companion.MinionRace.ValueNullable?.Name.ExtractText() ?? string.Empty;
        string behaviourName = companion.Behavior.ValueNullable?.Name.ExtractText() ?? string.Empty;
        
        ClientLanguage clientLanguage = dalamudServices.ClientState.ClientLanguage;

        if (clientLanguage == ClientLanguage.German)
        {
            singular = GermanReplace(singular, pronoun);
        }
        
        return new PetSheetData(petSkeleton, legacyModelId, icon, raceName, raceId, behaviourName, pronoun, singular, singular, 0);
    }
    
    public static PetSheetData? CreatePetSheetData(IPetSheets petSheets, Pet pet)
    {
        uint sheetSkeleton = pet.RowId;

        PetRegistration? registration = PetRegistration.GetRegistrationFromPet(sheetSkeleton);

        if (registration == null)
        {
            return null;
        }
        
        PetRegistration petRegistration = registration.Value;
        
        Action? petAction = petRegistration.GetAction(petSheets);
        
        if (petAction == null)
        {
            return null;
        }
        
        BNpcName? bnpcName = petRegistration.GetBNPCName(petSheets);

        if (bnpcName == null)
        {
            return null;
        }
        
        ushort petIcon           = petAction.Value.Icon;
        string name              = bnpcName.Value.Singular.ExtractText().ToTitleCase();
        string actionName        = petAction.Value.Name.ExtractText();
        uint   actionRowId       = petAction.Value.RowId;
        
        return new PetSheetData(petRegistration.PetSkeleton, -1, petIcon, bnpcName.Value.Pronoun, name, actionName, actionRowId);
    }
    
    private static string GermanReplace(string baseString, sbyte pronoun)
    {
        if (pronoun < 0 || pronoun >= pronounList.Length)
        {
            return baseString;
        }

        baseString = baseString.InvariantReplace("[p]", string.Empty);
        baseString = baseString.InvariantReplace("[a]", pronounList[pronoun]);   

        return baseString;
    }
}