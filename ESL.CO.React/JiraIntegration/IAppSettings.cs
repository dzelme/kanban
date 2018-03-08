using ESL.CO.React.Models;

namespace ESL.CO.React.JiraIntegration
{
    /// <summary>
    /// An interface for AppSettings class???
    /// </summary>
    public interface IAppSettings
    {
        FullBoardList GetSavedAppSettings();
        string SaveAppSettings(FullBoardList appSettings, string filePath = ".\\data\\appSettings.json");
    }
}