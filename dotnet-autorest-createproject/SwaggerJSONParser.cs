using System;
using System.IO;
using System.Net;
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
        /// <param name="fileUrl">url to the json file with swagger metadata</param>
        /// <returns>tuple with sanitised title and version</returns>
        public static (string, string) ParsetOut(string fileUrl)
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
                throw new ApplicationException($"Invalid url to swagger json file {fileUrl}");
            
            using (var client = new WebClient())
            {
                var json = client.DownloadString(fileUrl);
                var fragment = JsonSerializer.CreateDefault()
                    .Deserialize<SwaggerFragment>(
                        new JsonTextReader(new StringReader(json)));

                return fragment?.Info != null
                    ? (SanitizeTitle(fragment.Info.Title), fragment.Info.Version)
                    : (null, null);
            }
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
