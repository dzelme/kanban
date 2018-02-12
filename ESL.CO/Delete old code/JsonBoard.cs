using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;
/*
namespace ESL.CO.Helpers
{
    public class JsonBoard : Models.Board
    {
        //private JObject j;
        private string json;
        private string hash;
        private string path;

        //public JObject J { get => j; set => j = value; }
        public string Json { get => json; set => json = value; }
        public string Hash { get => hash; set => hash = value; }
        public string Path { get => path; set => path = value; }
        

        internal static string GetHashCode(string filePath, HashAlgorithm cryptoService)
        {
            // create or use the instance of the crypto service provider
            // this can be either MD5, SHA1, SHA256, SHA384 or SHA512
            using (cryptoService)
            {
                using (var fileStream = new FileStream(filePath,
                                                       FileMode.Open,
                                                       FileAccess.Read,
                                                       FileShare.ReadWrite))
                {
                    //fileStream.Position = 0;
                    var hash = cryptoService.ComputeHash(fileStream);
                    var hashString = Convert.ToBase64String(hash);
                    return hashString.TrimEnd('=');
                }
            }
        }


        /*
        public JsonBoard(string json, bool overwrite = false)
        {
            this.json = json;
            path = @".\data\" + boardId.ToString() + ".json";
            SaveToFile(overwrite);  //shouldn't call this here...
            //hash = CalculateMD5(path);
            hash = GetHashCode(path, new MD5CryptoServiceProvider());
        }
        */

        //gets board config
        /*
        public void Initialize (int id)
        {
            json = String.Empty;
            path = @".\data\" + Id.ToString() + ".json";
            ReadBoard();  //fills board
            AggregateJson();  //creates string to be saved in json file
            SaveToFile(path, true);  //writes the file
            hash = GetHashCode(path, new MD5CryptoServiceProvider());

        }

        public JsonBoard(int id) : base(id)
        {
            Initialize(id);
        }

        private void SaveToFile(string filePath, bool overwrite = false)
        {
            //string path = @"c:\temp\MyTest.txt";
            if (overwrite || !File.Exists(filePath))
            {
                AggregateJson();
                File.WriteAllText(filePath, json);
            }

            return;
        }

        public bool UpdateBoard()
        {
            //JObject j = Connect("board/" + BoardId.ToString() + "/issue");

            if (NeedsRedraw())
            {
                Initialize(Id);
                return true;
                //ReadBoard();
                //Json = new JsonFile(BoardId, j.ToString(Newtonsoft.Json.Formatting.None), true);
            }  //+change last updated date
            else { return false; }  //las updated date stays the same
        }

        public bool NeedsRedraw()
        {
            //aggregate same id (different var for result) -> save in new file -> calculate hash

            AggregateJson();
            SaveToFile(@".\data\0.json", true);
            string newHash = GetHashCode(@".\data\0.json", new MD5CryptoServiceProvider());
            if (String.Equals(hash, newHash, StringComparison.Ordinal)) { return false; }
            else { return true; }
        }

        private void AggregateJsonIssues(int startAt = 0)
        {
            int maxResults = 50; //per request
            if (IssueCount <= (startAt + maxResults))
            {
                //ReadBoardPage(startAt);
                JObject j = Connect("board/" + Id.ToString() + "/issue?startAt=" + startAt);
                json += j.ToString(Newtonsoft.Json.Formatting.None);
            }
            else
            {
                JObject j = Connect("board/" + Id.ToString() + "/issue?startAt=" + startAt);
                json += j.ToString(Newtonsoft.Json.Formatting.None);
                AggregateJsonIssues(startAt + maxResults);
            }
            return;
        }

        private void AggregateJson()
        {
            json = String.Empty;
            JObject j = Connect("board/" + Id.ToString() + "/configuration");
            json += j.ToString(Newtonsoft.Json.Formatting.None);
            AggregateJsonIssues();
            return;
        }

        




        //*/



        /*
        public FileProcessor(IHostingEnvironment env)//, IApplicationEnvironment appEnv)
        {
            hostingEnvironment = env;
            //appEnvironment = appEnv;
            appRootFolder = appEnv.ApplicationBasePath;
        }

        private IHostingEnvironment hostingEnvironment;
        //private IApplicationEnvironment appEnvironment;
        private string appRootFolder;


        public void SaveJsonToAppFolder(string appVirtualFolderPath, string fileName, string jsonContent)
        {
            var pathToFile = appRootFolder + appVirtualFolderPath.Replace("/", Path.DirectorySeparatorChar.ToString())
            + fileName;

            using (StreamWriter s = File.CreateText(pathToFile))
            {
                await s.WriteAsync(jsonContent);
            }
        }*/
  //  }
//}