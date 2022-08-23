using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace ESW.autorest.createProject
{
    /// <summary>
    /// this class parses out required data (title and version) out of the swagger json file
    /// </summary>
    public static class SwaggerJsonParser
    {
        internal static (string projectName, string projectVersion) ExtractMetadataInternal(string swaggerContents,
            string autoRestPackageVersion)
        {
            if (string.IsNullOrWhiteSpace(swaggerContents))
                throw new ArgumentException("Invalid swagger json content: null or empty");            
            
            var fragment = JsonSerializer.CreateDefault().Deserialize<SwaggerFragment>(new JsonTextReader(new StringReader(swaggerContents)));

            var projectVersion = !string.IsNullOrEmpty(autoRestPackageVersion) ?
                autoRestPackageVersion :
                SanitizeVersion(fragment.Info.Version);

            return fragment?.Info != null
                ? ($"{SanitizeTitle(fragment.Info.Title)}.AutoRestClient", projectVersion)
                : (null, null);            
        }
        
        /// <summary>
        /// Extract metadata from swagger json
        /// </summary>
        /// <param name="fileUrl">url to the json file with swagger metadata</param>
        /// <param name="autoRestPackageVersion"></param>
        /// <returns>tuple with sanitised title and version</returns>
        public static (string projectName, string projectVersion) ExtractMetadata(string fileUrl, string autoRestPackageVersion)
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
                throw new ArgumentException($"Invalid url to swagger json file {fileUrl}");

            using var client = new WebClient();
            var json = client.DownloadString(fileUrl);
            return ExtractMetadataInternal(json, autoRestPackageVersion);
        }

        /// <summary>
        /// sanitise title so that csproj can be named as such
        /// </summary>
        /// <param name="unsanitized">swagger title as it comes from swagger</param>
        /// <returns>sanitised version</returns>
        public  static string SanitizeTitle(string unsanitized)
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

        /// <summary>
        /// sanitise version coming from the swagger so that we can use it within the csproj and version the nuget package ultimately
        /// </summary>
        /// <param name="unsanitised">unsanitised version coming from the swagger</param>
        /// <returns>sanitised version</returns>
        public static string SanitizeVersion(string unsanitised)
        {
            Regex pattern = new Regex("\\d+(\\.\\d+)*");
            Match m = pattern.Match(unsanitised);
            return m?.Value ?? throw new ApplicationException($"Unrecognized version number {unsanitised}");
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
