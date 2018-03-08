using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;
using System.IO;
using ESL.CO.React.Models;

namespace ESL.CO.React.JiraIntegration
{
    /// <summary>
    /// A class for working with application settings.
    /// </summary>
    public class AppSettings : IAppSettings
    {
        /// <summary>
        /// Reads application settings from a JSON file to an object.
        /// </summary>
        /// <returns>An object containing a list of all saved Kanban boards and their settings.</returns>
        public FullBoardList GetSavedAppSettings()
        {
            var filePath = @".\data\appSettings.json";
            FullBoardList fullBoardList = new FullBoardList();
            if (File.Exists(filePath))
            {
                using (StreamReader r = new StreamReader(filePath))
                {
                    string json = r.ReadToEnd();
                    fullBoardList = JsonConvert.DeserializeObject<FullBoardList>(json);
                }
            }
            else
            {
                return null;
            }
            return fullBoardList;
        }

        /// <summary>
        /// Saves application settings to the specified path.
        /// </summary>
        /// <param name="appSettings">An object containing a list of Kanban boards and their settings.</param>
        /// <param name="filePath">Path of the file where application settings will be stored. Default path in interface.</param>
        /// <returns>Path of the file where application settings were stored.</returns>
        public string SaveAppSettings(FullBoardList appSettings, string filePath)
        {
            // serialize JSON directly to a file
            using (StreamWriter file = System.IO.File.CreateText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, appSettings);
            }

            return filePath;

            //consider replacing with a global SaveToJson<type> method
        }

        /// <summary>
        /// Upadates the current board list obtained from Jira with the corresponding saved settings from the application settings file.
        /// </summary>
        /// <param name="saved">An object containing the stored list of all available Kanban boards and their settings.</param>
        /// <param name="current">An object containing the newly obtained list of all currently available Kanban boards with default settings.</param>
        /// <returns>An object containing a list of all currently available Kanban boards and their settings.</returns>
        public static FullBoardList MergeSettings(FullBoardList saved, FullBoardList current)
        {
            if (saved == null)
            {
                if (current == null) { return null; }
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
                        currentBoard.Type = savedBoard.Type;
                        currentBoard.RefreshRate = Math.Max(savedBoard.RefreshRate, 1000);  //should be same as range minimum in class BoardList
                        currentBoard.TimeShown = Math.Max(savedBoard.TimeShown, 5000);  //should be same as range minimum in class BoardList
                        currentBoard.Visibility = savedBoard.Visibility;
                        currentBoard.TimesShown = savedBoard.TimesShown;
                        currentBoard.LastShown = savedBoard.LastShown;
                    }
                }
            }
            return current;

            // first page read, after no connection.
            // which settings list should be used?
            // the one that was read from the first page only or 
            // the one stored in app settings (old one)??
        }

    }
}
