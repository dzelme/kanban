using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;
using System.IO;

using System.Security.Cryptography;
using ESL.CO.React.Models;


namespace ESL.CO.React.JiraIntegration
{
    public class CacheMethods
    {
        public string GetBoardPath(string boardId)
        {
            var filePath = Path.Combine(@".\data\", boardId.ToString() + ".json");
            return filePath;
        }

        public Board GetCachedBoard(string boardId)
        {
            // read from JSON to object, if file exists
            var filePath = GetBoardPath(boardId);
            Board cachedBoard = new Board(boardId);  // needed to keep requesting same board after no connection (not 0)
            if (File.Exists(filePath))
            {
                using (StreamReader r = new StreamReader(filePath))
                {
                    string json = r.ReadToEnd();
                    cachedBoard = JsonConvert.DeserializeObject<Board>(json);
                }
            }
            else
            {
                cachedBoard.Message = "No connection. No cache.";  //cachedBoard.Message += " No cache.";
                return cachedBoard;
            }
            cachedBoard.FromCache = true; //
            cachedBoard.Message = File.GetLastWriteTime(filePath).ToString();
            return cachedBoard;
        }

        public string SaveBoardAsTempFile(Board board, string tempPath = @".\data\temp.json")
        {
            // save info read from JIRA in a temp file
            // serialize JSON directly to a file
            using (StreamWriter file = File.CreateText(tempPath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, board);
            }

            return tempPath;
        }

        public bool NeedsRedraw(Board board)
        {
            var filePath = GetBoardPath(board.Id);
            var cachedHash = string.Empty;
            if (File.Exists(filePath)) { cachedHash = GetHashCode(filePath, new MD5CryptoServiceProvider()); }

            var tempPath = SaveBoardAsTempFile(board);
            var hash = GetHashCode(tempPath, new MD5CryptoServiceProvider());

            // overwrite cached file if new file is different
            if (!(String.Equals(cachedHash, hash)) && (board.Name != ""))  //turn it into a bool flag...
            {
                File.Copy(tempPath, filePath, true);
                File.Delete(tempPath);
                return true;
            }

            File.Delete(tempPath);
            return false;
        }


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
    }
}
