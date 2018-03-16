using ESL.CO.React.Models;

namespace ESL.CO.React.JiraIntegration
{
    public interface IAppSettings
    {
        int GeneratePresentationId();
        BoardPresentation GetPresentation(int id, string filePath = "");
        FullPresentationList GetPresentationList();
        FullBoardList GetSavedAppSettings();
        string SaveAppSettings(FullBoardList appSettings, string filePath = ".\\data\\appSettings.json");
        string SavePresentation(BoardPresentation boardPresentation);
    }
}