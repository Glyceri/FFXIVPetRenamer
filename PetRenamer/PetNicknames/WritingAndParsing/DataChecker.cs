using Dalamud.Utility;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;

namespace PetRenamer.PetNicknames.WritingAndParsing;

internal class DataChecker : IDataChecker
{
    private readonly IPetServices PetServices;
    
    public DataChecker(IPetServices petServices)
    {
        PetServices = petServices;
    }
    
    private bool CheckAllSkeletonsOfType(PetSkeleton[] petSkeletons, SkeletonType skeletonType)
    {
        int length = petSkeletons.Length;
        
        for (int i = 0; i < length; i++)
        {
            PetSkeleton skeleton = petSkeletons[i];
            
            if (skeleton.SkeletonType == skeletonType)
            {
                continue;
            }
            
            return false;
        }
        
        return true;
    }
    
    public bool CheckModernData(ulong contentId, IModernParseResult modernParseResult)
    {
        if (modernParseResult.ContentId != contentId)
        {
            return false;
        }
        
        if (!CheckAllSkeletonsOfType(modernParseResult.SoftSkeletons, SkeletonType.BattlePet))
        {
            return false;
        }
        
        return CheckData(modernParseResult);
    }

    public bool CheckData(IBaseParseResult baseParseResult)
    {
        int idsLength         = baseParseResult.IDs.Length;
        int namesLength       = baseParseResult.Names.Length;
        int edgeColoursLength = baseParseResult.EdgeColous.Length;
        int textColoursLength = baseParseResult.TextColours.Length;
        
        if (idsLength != namesLength || idsLength != edgeColoursLength || idsLength != textColoursLength)
        {
            return false;
        }
        
        if (baseParseResult.UserName.IsNullOrWhitespace())
        {
            return false;
        }
        
        if (baseParseResult.UserName.Length > PluginConstants.ffxivNameSize)
        {
            return false;
        }
        
        if (PetServices.PetSheets.GetWorldName(baseParseResult.Homeworld).IsNullOrWhitespace())
        {
            return false;
        }
        
        return true;
    }
}