using System;
using Newtonsoft.Json;
using System.IO;
using ESL.CO.React.Models;
using ESL.CO.React.DbConnection;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Collections.Generic;

namespace ESL.CO.React.JiraIntegration
{
    /// <summary>
    /// A class for working with application settings.
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Upadates the current board list obtained from Jira with the corresponding saved settings from the application settings file.
        /// </summary>
        /// <param name="saved">An object containing the stored list of all available Kanban boards and their settings.</param>
        /// <param name="current">An object containing the newly obtained list of all currently available Kanban boards with default settings.</param>
        /// <returns>An object containing a list of all currently available Kanban boards and their settings.</returns>
        public static FullBoardList MergeSettings(FullBoardList saved, FullBoardList current, IOptions<UserSettings> userSettings)
        {
            if (saved == null)
            {
                if (current == null) { return null; }

                foreach (Value currentBoard in current.Values)
                {
                    currentBoard.RefreshRate = userSettings.Value.RefreshRateMin;
                    currentBoard.TimeShown = userSettings.Value.TimeShownMin;
                }
                return current;
            }
            if (current == null) { return saved; }
            
            foreach (Value currentBoard in current.Values)
            {
                foreach (Value savedBoard in saved.Values)
                {
                    if (savedBoard.Id == currentBoard.Id)
                    {
                        currentBoard.Name = savedBoard.Name;
                        //currentBoard.Type = savedBoard.Type;
                        currentBoard.RefreshRate = Math.Max(savedBoard.RefreshRate, userSettings.Value.RefreshRateMin);
                        currentBoard.TimeShown = Math.Max(savedBoard.TimeShown, userSettings.Value.TimeShownMin);
                        currentBoard.Visibility = savedBoard.Visibility;
                    }
                }
            }
            return current;
        }
    }
}
