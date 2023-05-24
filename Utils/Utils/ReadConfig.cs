
using Newtonsoft.Json.Linq;

namespace GameListDB
{
    public static class ConfigUtils
    {
        public static DTO.Config ReadConfig(bool verbose)
        {
            DTO.Config config;
            if (File.Exists(Directory.GetCurrentDirectory() + "/config.json"))
            {
                if (verbose)
                {
                    GameListDB.Utils.Log(("Reading config file:" + Directory.GetCurrentDirectory() + "/config.json"));
                }
                config = JObject.Parse(File.ReadAllText(Directory.GetCurrentDirectory() + "/config.json")).ToObject<DTO.Config>();
                if (config != null)
                {
                    if (verbose)
                    {
                        GameListDB.Utils.Log("Config file read successfully\n");
                    }
                    return config;
                }
            }
            else if (Environment.GetEnvironmentVariable("IGDB_CLIENT_ID").Length != 0 && Environment.GetEnvironmentVariable("IGDB_CLIENT_SECRET").Length != 0)
            {
                return new DTO.Config
                {
                    IGDB_CLIENT_ID=Environment.GetEnvironmentVariable("IGDB_CLIENT_ID"),
                    IGDB_CLIENT_SECRET=Environment.GetEnvironmentVariable("IGDB_CLIENT_SECRET")
                };
            }

                GameListDB.Utils.Log(
                    "It doesn't exist config file: " +
                    Directory.GetCurrentDirectory() + "/config.json" +
                    "\nPlease check the config file location");
                System.Environment.Exit(-1);
                return null;
            
        }
    }
}