namespace PetRenamer.Core.Sharing.Importing.Data;

public class FailedImportData : ImportData
{
    public string ErrorMessage { get; set; } = string.Empty;
    public FailedImportData(string ErrorMessage)
    {
        this.ErrorMessage = ErrorMessage;
    }
}
