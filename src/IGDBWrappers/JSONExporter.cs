using IGDB;
using GameListDB.Model;
using GameListDB.Model.Extensions;
using System; // System.String
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks; // Tasks
using System.IO;

namespace GameListDB.IGDBWrappers
{
    class JSONExporter : IGDBQueryBase
    {
        public JSONExporter(ref IGDBClient igdb,string _defaultPath, GameListDB.DTO.Options options) : base(ref igdb)
        {
            defaultPath=_defaultPath;
            force=options.Force;
        }

        private string defaultPath="./list.json";
        byte lastyears=14;
        bool force = false;
        public JObject loadJson()
        {
            if (force){ return new JObject();}
            try {
            return JObject.Parse(File.ReadAllText(defaultPath));
            }catch
            {
                return new JObject();
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

        public async Task<string> geturlAsync(Backlog game)
        {
            try{
            return "http:" + (IGDB.ImageHelper.GetImageUrl(imageId: 
            (await this.RequestQuery("cover.image_id",gamelistdb.GetIgdbId(game))).First().Cover.Value.ImageId, size: ImageSize.CoverBig, retina: false)).ToString();
            }catch
            {
                return "";
            }
        }
    }
}