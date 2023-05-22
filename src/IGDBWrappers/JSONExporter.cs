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
        public JSONExporter(ref IGDBClient igdb,string _defaultPath, Options options) : base(ref igdb)
        {
            defaultPath=_defaultPath;
            force=options.Force;
            ExceptionJSON=loadExceptionJson();
        }

        IDictionary<string,DTO.ExceptionsJSONElement> ExceptionJSON;
        private string defaultPath="./list.json";
        private string exceptionjsonPath="./Exceptions.json";

        private byte lastyears=14;
        private bool force = false;
        private JObject loadJson()
        {
            if (force){ return new JObject();}
            try {
            return JObject.Parse(File.ReadAllText(defaultPath));
            }catch
            {
                return new JObject();
            }
        }


        private IDictionary<string,ExceptionsJSONElement> loadExceptionJson()
        {
            try {
            return JObject.Parse(File.ReadAllText(exceptionjsonPath)).ToObject<Dictionary<string,ExceptionsJSONElement>>();
            }catch
            {
                return new Dictionary<string,ExceptionsJSONElement>();
            }
        }
        

        public async Task RunAsync()
        {
            JObject output = loadJson();
            bool modifiedList = false;
            string currentYear;

            for(int i=0; i != lastyears;i++)
            {
                currentYear = (DateTime.Now.Year - i).ToString();
                Utils.Log("Looking for games beaten on "+ currentYear);

                
                foreach (var game in gamelistdb.GetBeatenGames(DateTime.Now.Year - i))
                {
//                    Utils.Log("Checking " + game.Name);

                    if (output[currentYear] != null && IsInTheList(output, currentYear, game))
                    {
                        continue;
                    }

                    if (output[currentYear] == null)
                    {
                        output.Add(new JProperty(currentYear,new JArray()));
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
                            new JProperty("img", (await geturlAsync(game)))));

                }
                if (!modifiedList || output[currentYear] == null){continue;}

                output[currentYear] = new JArray(output[currentYear].OrderBy(element=>element.SelectToken("name")));
                modifiedList = false;

            }
            System.IO.File.WriteAllText(defaultPath,output.ToString());
        }

        private static bool IsInTheList(JObject output, string currentYear, Backlog game)
        {
            return output[currentYear].SelectToken("$.[?(@.name=='" + game.Name.Replace("'","\\'") + "')]") != null;
        }

        public long Wrapper(long igdbid) 
        {
             
            if(ExceptionJSON.ContainsKey(igdbid.ToString()) && ExceptionJSON[igdbid.ToString()].type.Equals("igdb"))
            {
                return (long)ExceptionJSON[igdbid.ToString()].igdbid;
            }

            return igdbid;
        }

        public async Task<string> geturlAsync(Backlog game)
        {
            try{
            var igdbid = gamelistdb.GetIgdbId(game);
            if(ExceptionJSON.ContainsKey(igdbid.ToString()) && ExceptionJSON[igdbid.ToString()].type.Equals("url"))
            {
                return ExceptionJSON[igdbid.ToString()].url;
            }
            return await igdb.GetBoxArtURL(Wrapper(igdbid));
            }catch
            {
                return "";
            }
        }
    }
}