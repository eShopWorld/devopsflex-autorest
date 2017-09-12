using System;
using System.IO;
using Newtonsoft.Json;

namespace ESW.autorest.createProject
{
    /// <summary>
    /// this class parses out required data (title and version) out of the swagger json file
    /// </summary>
    public static class SwaggerJsonParser
    {
        /// <summary>
        /// parse out title and version and return as a tuple
        /// </summary>
        /// <param name="filePath">filepath to the json file with swagger metadata</param>
        /// <returns>tuple with sanitised title and version</returns>
        public static (string, string) ParsetOut(string filePath)
        {
            if (!File.Exists(filePath))
                throw new ApplicationException($"Invalid path to swagger json file {filePath}");

            var fragment = JsonSerializer.CreateDefault()
                .Deserialize<SwaggerFragment>(
                    new JsonTextReader(new StreamReader(new FileStream(filePath, FileMode.Open))));

            return fragment?.Info != null  ? (SanitizeTitle(fragment.Info.Title), fragment.Info.Version) : (null, null);
        }

        private static string SanitizeTitle(string unsanitized)
        {
            return unsanitized?
                .Replace(" ", "")
                .Replace("/", "")
                .Replace("?", "")
                .Replace(":", "")
                .Replace("&", "")
                .Replace("\\", "")
                .Replace("*", "")
                .Replace("\"", "")
                .Replace("<", "")
                .Replace(">", "")
                .Replace(">", "")
                .Replace("|", "")
                .Replace("#", "")
                .Replace("%", "");
        }

        public class SwaggerFragment
        {
            [JsonProperty("info")]
            public SwaggerInfoFragment Info { get; set; }
        }

        public class SwaggerInfoFragment
        {
            [JsonProperty("version")]
            public string Version { get; set; }

            [JsonProperty("title")]
            public string Title { get; set; }
        }
    }
}
