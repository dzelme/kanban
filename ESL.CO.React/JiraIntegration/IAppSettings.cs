using ESL.CO.React.Models;

namespace ESL.CO.React.JiraIntegration
{
    public interface IAppSettings
    {
        FullBoardList GetSavedAppSettings();
        FullBoardList MergeSettings(FullBoardList saved, FullBoardList current);
        string SaveAppSettings(FullBoardList appSettings, string filePath = ".\\data\\appSettings.json");
    }
}