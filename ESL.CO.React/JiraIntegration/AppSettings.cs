using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;
using System.IO;
using ESL.CO.React.Models;

namespace ESL.CO.React.JiraIntegration
{
    public class AppSettings
    {
        public FullBoardList GetSavedAppSettings()
        {
            // read from JSON to object, if file exists
            var filePath = @".\data\appSettings.json";
            FullBoardList appSettings = new FullBoardList();
            if (System.IO.File.Exists(filePath))
            {
                using (StreamReader r = new StreamReader(filePath))
                {
                    string json = r.ReadToEnd();
                    appSettings = JsonConvert.DeserializeObject<FullBoardList>(json);
                }
            }
            else
            {
                return null;
            }
            return appSettings;
        }

        public string SaveAppSettings(FullBoardList appSettings, string filePath = @".\data\appSettings.json")
        {
            // save info read from JIRA in a temp file
            // serialize JSON directly to a file
            using (StreamWriter file = System.IO.File.CreateText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, appSettings);
            }

            return filePath;
        }

        public FullBoardList MergeSettings(FullBoardList saved, FullBoardList current)
        {
            if (saved == null)
            {
                if (current == null) { return null; }
                return current;
            }
            if (current == null) { return saved; }
            
            foreach (Value currentBoard in current.AllValues)
            {
                foreach (Value savedBoard in saved.AllValues)
                {
                    if (savedBoard.Id == currentBoard.Id)
                    {
                        currentBoard.Name = savedBoard.Name;
                        currentBoard.Type = savedBoard.Type;
                        currentBoard.RefreshRate = Math.Max(savedBoard.RefreshRate, 1000);  //should be same as range minimum in class BoardList
                        currentBoard.TimeShown = Math.Max(savedBoard.TimeShown, 5000);  //should be same as range minimum in class BoardList
                        currentBoard.Visibility = savedBoard.Visibility;
                    }
                }
            }
            return current;
        }

    }
}
