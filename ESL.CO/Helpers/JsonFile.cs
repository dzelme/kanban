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

namespace ESL.CO.Helpers
{
    public class JsonFile
    {
        private int boardId;
        private string json;
        private string hash;
        private string path;

        public int BoardId { get => boardId; set => boardId = value; }
        public string Json { get => json; set => json = value; }
        public string Hash { get => hash; set => hash = value; }
        public string Path { get => path; set => path = value; }


        /*
        public static string CalculateMD5(string filePath)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", String.Empty).ToLowerInvariant();
                }
            }
        }
        */

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



        public JsonFile(int boardId, string json, bool overwrite = false)
        {
            this.boardId = boardId;
            this.json = json;
            path = @".\data\" + boardId.ToString() + ".json";
            SaveToFile(overwrite);  //shouldn't call this here...
            //hash = CalculateMD5(path);
            hash = GetHashCode(path, new MD5CryptoServiceProvider());
        }

        private void SaveToFile(bool overwrite = false)
        {
            //string path = @"c:\temp\MyTest.txt";

            if (overwrite || !File.Exists(path)) { File.WriteAllText(path, json); }

            return;
            
            /*
            // Open the file to read from.
            using (StreamReader sr = File.OpenText(path))
            {
                string s = "";
                while ((s = sr.ReadLine()) != null)
                {
                    Console.WriteLine(s);
                }
            }*/

            //string s = CalculateMD5(path);
        }




        
        public bool NeedsRedraw(int boardId)
        {
            string urlIssue = "https://jira.returnonintelligence.com/rest/agile/1.0/board/" + boardId.ToString() + "/issue";   //?startAt=" + startAt;

            WebRequest myReq = WebRequest.Create(urlIssue);
            //type password in creds...
            #region Credentials
            string credentials = "user:pass";
            #endregion
            myReq.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials));
            WebResponse wr = myReq.GetResponse();
            Stream receiveStream = wr.GetResponseStream();
            StreamReader reader = new StreamReader(receiveStream, Encoding.UTF8);
            string content = reader.ReadToEnd();
            JObject j = JObject.Parse(content);

            JsonFile tempJson = new JsonFile(0, j.ToString(Newtonsoft.Json.Formatting.None));  //temporarily saves newest json to calculate its hash (boardId = 0)
            //string fileHash = CalculateMD5(@".\data\" + boardId.ToString() + ".json");
            string fileHash = GetHashCode(@".\data\" + boardId.ToString() + ".json", new MD5CryptoServiceProvider());
            if (String.Equals(tempJson.Hash, fileHash, StringComparison.Ordinal))
            { return false; }
            else { return true; }
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
    }
}
