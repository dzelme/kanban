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
        public string GetPath(int boardId)
        {
            var filePath = Path.Combine(@".\data\", boardId.ToString() + ".json");
            return filePath;
        }

        public Board GetCachedBoard(int boardId)
        {
            // read from JSON to object, if file exists
            var filePath = GetPath(boardId);
            Board cachedBoard = new Board();
            if (System.IO.File.Exists(filePath))
            {
                using (StreamReader r = new StreamReader(filePath))
                {
                    string json = r.ReadToEnd();
                    cachedBoard = JsonConvert.DeserializeObject<Board>(json);
                }
            }
            else
            {
                cachedBoard.Message = "No connection. No cache.";
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
            using (StreamWriter file = System.IO.File.CreateText(tempPath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, board);
            }

            return tempPath;
        }

        public bool NeedsRedraw(Board board)
        {
            var filePath = GetPath(board.Id);
            var cachedHash = string.Empty;
            if (System.IO.File.Exists(filePath)) { cachedHash = GetHashCode(filePath, new MD5CryptoServiceProvider()); }

            var tempPath = SaveBoardAsTempFile(board);
            var hash = GetHashCode(tempPath, new MD5CryptoServiceProvider());

            // overwrite cached file if new file is different
            if (!String.Equals(cachedHash, hash))
            {
                System.IO.File.Copy(tempPath, filePath, true);
                System.IO.File.Delete(tempPath);
                return true;
            }

            System.IO.File.Delete(tempPath);
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
