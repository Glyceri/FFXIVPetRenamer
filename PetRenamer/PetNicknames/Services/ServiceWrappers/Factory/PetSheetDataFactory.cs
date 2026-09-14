using Dalamud.Game;
using Dalamud.Utility;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Statics;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using XBMPet = PetRenamer.PetNicknames.Services.ServiceWrappers.Sheets.XBMPetButActuallyWorkingSinceForSomeReasonWePutInUnsusedSheetsAndNowEverythingIsABreakingChangeBecauseWhyWouldntItBeLikeWhatAreWeGenuinelyDoingHere;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Factory;

internal static class PetSheetDataFactory
{ 
    private static readonly string[] pronounList = ["er", "e", "es", "en"];
    
    public static PetSheetData? CreatePetSheetData(IPetSheets petSheets, DalamudServices dalamudServices, Companion companion)
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
        PetSkeleton petSkeleton    = new PetSkeleton(SkeletonType.Minion, (uint)modelId);
        
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
        uint   action        = companion.RowId;
        sbyte  pronoun       = companion.Pronoun;
        uint   raceId        = companion.MinionRace.ValueNullable?.RowId ?? 0;
        string raceName      = companion.MinionRace.ValueNullable?.Name.ExtractText() ?? string.Empty;
        string behaviourName = companion.Behavior.ValueNullable?.Name.ExtractText() ?? string.Empty;
        string actionName    = petSheets.GetAction(action)?.Name.ExtractText() ?? string.Empty;
        
        ClientLanguage clientLanguage = dalamudServices.ClientState.ClientLanguage;

        if (clientLanguage == ClientLanguage.German)
        {
            singular = GermanReplace(singular, pronoun);
        }
        
        return new PetSheetData(petSkeleton, legacyModelId, icon, raceName, raceId, behaviourName, pronoun, singular, actionName, action);
    }
    
    public static PetSheetData? CreatePetSheetData(IPetSheets petSheets, PetRegistration petRegistration)
    {
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
        
        ushort  petIcon         = petAction.Value.Icon;
        string  name            = bnpcName.Value.Singular.ExtractText().ToTitleCase();
        string  actionName      = petAction.Value.Name.ExtractText();
        uint    actionRowId     = petAction.Value.RowId;
        string  petType         = petSheets.GetAddonString(8588);
        Pet?    petReg          = petRegistration.GetBattlePet(petSheets);
        Action? signatureAction = GetAction(petReg);
        
        return new PetSheetData(petRegistration.PetSkeleton, -1, petIcon, petType, 0, signatureAction?.Name.ExtractText() ?? null, bnpcName.Value.Pronoun, name, actionName, actionRowId);
    }
    
    private static Action? GetAction(Pet? pet)
    {
        if (pet == null)
        {
            return null;
        }
        
        if (pet.Value.AutoAction.RowId != 0)
        {
            return pet.Value.AutoAction.ValueNullable;
        }
        
        foreach (RowRef<Action> petAction in pet.Value.Abilities)
        {
            if (petAction.RowId == 0)
            {
                continue;
            }
            
            return petAction.ValueNullable;
        }
        
        return null;
    }
    
    public static PetSheetData? CreatePetSheetDataBeastMaster(IPetSheets petSheets, PetRegistration petRegistration)
    {
        XBMPet? pet = petRegistration.GetBeastMasterPet(petSheets);
        
        if (pet == null)
        {
            return null;
        }
        
        BNpcName? bnpcName = petRegistration.GetBNPCName(petSheets);

        if (bnpcName == null)
        {
            return null;
        }
        
        uint   iconId            = pet.Value.Icon;
        sbyte  pronoun           = bnpcName.Value.Pronoun;
        string name              = bnpcName.Value.Singular.ExtractText().ToTitleCase();
        byte   aspect            = pet.Value.Action.Value.Aspect;
        string kin               = petSheets.GetAddonString(17740 + (uint)pet.Value.Classification);
        
        return new PetSheetData(petRegistration.PetSkeleton, -1, iconId, kin, aspect, null, pronoun, name, name, petRegistration.GetRawAction());
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