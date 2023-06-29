using IGDB;
using GameListDB.Model;
using GameListDB.Model.Extensions;
using System; // System.String
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks; // Tasks
using System.IO;
using System.Collections;
using System.Collections.Generic;
using GameListDB.DTO;

namespace GameListDB.IGDBWrappers
{
    class JSONExporter : IGDBWrapperBase
    {
        public JSONExporter(ref IGDBClient igdb, string _defaultPath, Options options) : base(ref igdb)
        {
            defaultPath = _defaultPath;
            this.options = options;
            ExceptionJSON = loadExceptionJson();
        }

        IDictionary<string, DTO.ExceptionsJSONElement> ExceptionJSON;
        private string defaultPath = "./list.json";
        private string exceptionjsonPath = "./Exceptions.json";

        private byte lastyears = 14;
        private Options options;
        private JObject loadJson()
        {
            if (options.Force) { return new JObject(); }
            try
            {
                return JObject.Parse(File.ReadAllText(defaultPath));
            }
            catch
            {
                return new JObject();
            }
        }


        private IDictionary<string, ExceptionsJSONElement> loadExceptionJson()
        {
            try
            {
                return JObject.Parse(File.ReadAllText(exceptionjsonPath)).ToObject<Dictionary<string, ExceptionsJSONElement>>();
            }
            catch
            {
                return new Dictionary<string, ExceptionsJSONElement>();
            }
        }


        public async Task RunAsync()
        {
            JObject output = loadJson();
            bool modifiedList = false;
            string currentYear;

            for (int i = 0; i != lastyears; i++)
            {
                currentYear = (DateTime.Now.Year - i).ToString();
                Utils.Log("Looking for games beaten on " + currentYear);


                foreach (var game in gamelistdb.GetBeatenGames(DateTime.Now.Year - i))
                {
                    if (options.Verbose) Utils.Log("Checking " + game.Name);

                    if (output[currentYear] != null && IsInTheList(output, currentYear, game))
                    {
                        continue;
                    }

                    if (output[currentYear] == null)
                    {
                        output.Add(new JProperty(currentYear, new JArray()));
                    }

                    Utils.Log("Adding " + game.Name + " To JSON");
                    modifiedList = true;
                    ((JArray)output[currentYear]).Add(
                        new JObject(
                            new JProperty("name", game.Name),
                            new JProperty("status", game.Status),
                            new JProperty("nsfw", game.Nsfw),
                            new JProperty("plataform", game.Plataform),
                            new JProperty("releaseyear", game.Releaseyear),
                            new JProperty("img", (await GetBoxArtUrlAsync(game)))));

                }
                
                if (!modifiedList || output[currentYear] == null) { continue; }

                output[currentYear] = new JArray(output[currentYear].OrderBy(element => element.SelectToken("name")));
                modifiedList = false;

            }
            System.IO.File.WriteAllText(defaultPath, output.ToString());
        }

        /// <summary>
        /// Checks If a Game is already in the output list.
        /// </summary>
        /// <param name="output">Array to Check</param>
        /// <param name="currentYear">year to Check</param>
        /// <param name="game">game to Check</param>
        /// <returns>valuation</returns>
        private static bool IsInTheList(JObject output, string currentYear, Backlog game)
        {
            return output[currentYear].SelectToken("$.[?(@.name=='" + game.Name.Replace("'", "\\'") + "')]") != null;
        }

        /// <summary>
        /// This function override the default igdb id of a Game by other one. 
        /// 
        /// This only make sense when a Remake or original has worst boxart than other one.
        /// </summary>
        /// <param name="igdbid">igdbid to check in the exception.json</param>
        /// <returns>original or overrided igdbid</returns>
        public long IgdbIdReplacementWrapper(long igdbid)
        {

            if (ExceptionJSON.ContainsKey(igdbid.ToString()) && ExceptionJSON[igdbid.ToString()].type.Equals("igdb"))
            {
                return (long)ExceptionJSON[igdbid.ToString()].igdbid;
            }

            return igdbid;
        }

        /// <summary>
        /// This function gathers the BoxArt Image URL taking in mind some exceptions and others variables
        /// </summary>
        /// <param name="game">The Game to Get the BoxArt</param>
        /// <returns>the BoxArt Image URL</returns>
        public async Task<string> GetBoxArtUrlAsync(Backlog game)
        {
            try
            {
                var igdbid = gamelistdb.GetIgdbId(game);

                // Checks inside of Exception.json If It's required to change any default box art for some aesthetic reasons
                if (ExceptionJSON.ContainsKey(igdbid.ToString()) )
                {
                    if (options.Verbose) Utils.Log("Detected " + game.Name + "In the ExceptionJson which It's type " + ExceptionJSON[igdbid.ToString()].type);

                    switch (ExceptionJSON[igdbid.ToString()].type)
                    {
                        case "url": return ExceptionJSON[igdbid.ToString()].url;
                        case "steamid": return "https://cdn.cloudflare.steamstatic.com/steam/apps/"+ExceptionJSON[igdbid.ToString()].steamid+"/library_600x900.jpg";
                    }
                }

                return await igdb.GetBoxArtURL(IgdbIdReplacementWrapper(igdbid));
            }
            catch
            {
                return "";
            }
        }
    }
}