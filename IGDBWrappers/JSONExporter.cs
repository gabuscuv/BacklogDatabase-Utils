using IGDB;
using gamelist_db.Model;
using GamelistDB.Extensions;
using System; // System.String
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks; // Tasks
using System.IO;

namespace GamelistDB.IGDBWrappers
{
    class JSONExporter : IGDBQueryBase
    {
        public JSONExporter(ref IGDBClient igdb,string _defaultPath) : base(ref igdb)
        {
            defaultPath=_defaultPath;
        }

        private string defaultPath="./list.json";
        byte lastyears=14;

        public JObject loadJson()
        {
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