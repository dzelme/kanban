using System;
using Newtonsoft.Json;
using System.IO;
using ESL.CO.React.Models;
using Microsoft.Extensions.Options;
using System.Linq;

namespace ESL.CO.React.JiraIntegration
{
    /// <summary>
    /// A class for working with application settings.
    /// </summary>
    public class AppSettings : IAppSettings
    {
        private readonly IOptions<Paths> paths;

        public AppSettings(IOptions<Paths> paths)
        {
            this.paths = paths;
        }

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
        public string SaveAppSettings(FullBoardList appSettings, string filePath = ".\\data\\appSettings.json")
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






        /// <summary>
        /// Generates a unique presentation id.
        /// </summary>
        /// <param name="presentationDirectoryPath">The path of the file where presentation settings will be stored. Default path in interface.</param>
        /// <returns>Unique presentation id.</returns>
        public string GeneratePresentationId()
        {
            Directory.CreateDirectory(paths.Value.PresentationDirectoryPath);
            var filePath = Path.Combine(paths.Value.PresentationDirectoryPath, @"settings.txt");
            int id = 0;

            // reads the current last id
            if (File.Exists(filePath))
            {
                using (StreamReader r = new StreamReader(filePath))
                {
                    id = int.Parse(r.ReadLine());
                }
            }

            // writes the new last id
            ++id;
            File.WriteAllText(filePath, id.ToString());

            return id.ToString();
        }

        /// <summary>
        /// Saves a given presentation to JSON file at the specified path.
        /// </summary>
        /// <param name="boardPresentation">An object containing presentation data to be saved.</param>
        /// <param name="presentationDirectoryPath">The path of the file where presentation settings will be stored. Default path in interface.</param>
        /// <returns>The path of the file where presentation settings were stored.</returns>
        public string SavePresentation(BoardPresentation boardPresentation)
        {
            Directory.CreateDirectory(paths.Value.PresentationDirectoryPath);
            boardPresentation.Id = GeneratePresentationId();
            var filePath = Path.Combine(paths.Value.PresentationDirectoryPath, @"p_" + boardPresentation.Id + @".json");

            // overwrites if exists
            File.WriteAllText(filePath, JsonConvert.SerializeObject(boardPresentation));

            return filePath;

            //consider replacing with a global SaveToJson<type> method

            //consider moving to controller and not having a separate method
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="presentationDirectoryPath"></param>
        /// <returns></returns>
        public FullPresentationList GetPresentationList()
        {
            Directory.CreateDirectory(paths.Value.PresentationDirectoryPath);
            var presentationList = new FullPresentationList();

            string[] presentationPaths = Directory.GetFiles(paths.Value.PresentationDirectoryPath, "p_*.json", SearchOption.TopDirectoryOnly);
            foreach (string path in presentationPaths)
            {
                presentationList.PresentationList.Add(GetPresentation("", path));
            }

            // sorts the presentationList by id in ascending order before returning
            var sorted = presentationList.PresentationList.OrderBy(c => int.Parse(c.Id)).ToList();
            presentationList.PresentationList = sorted;
            return presentationList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="filePath"></param>
        /// <param name="presentationDirectoryPath"></param>
        /// <returns></returns>
        public BoardPresentation GetPresentation(string id, string filePath = "")
        {
            if (filePath == "")
            {
                filePath = Path.Combine(paths.Value.PresentationDirectoryPath, @"p_" + id + @".json");
                if (!File.Exists(filePath)) { return null; }
            }

            var boardPresentation = new BoardPresentation();
            using (StreamReader r = new StreamReader(filePath))
            {
                string json = r.ReadToEnd();
                boardPresentation = JsonConvert.DeserializeObject<BoardPresentation>(json);
            }
            return boardPresentation;
        }
    }
}
