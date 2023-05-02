using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace GamelistDB
{
    class Program
    {
        // TODO: Dirty, I should make a Dependency Injection.
        private static IGDB.IGDBClient igdb;
        private static DTO.Config config;
        static string[] Options = { "Add IGDB references", "Add Scores from IGDB", "Add Year Release from IGDB", "Add HowLongToBeat Stats", "Export GamesCompleted/Beaten", "", "", "", "", "Quit" };
        static string writebuffer;
        static bool exit = false;
        public static async Task Main(string[] args)
        {
            ReadConfig();

            while (!exit)
            {
                for (int counter = 0; counter < Options.Length; counter++)
                {
                    if (!String.IsNullOrEmpty(Options[counter]))
                    {
                        System.Console.WriteLine("\t[" + counter + "] - " + Options[counter]);
                    }
                }

                int parsedvalue;
                do
                {
                    if (!String.IsNullOrEmpty(writebuffer)) { System.Console.WriteLine("\nPardon, Could You write again please?"); }
                    System.Console.Write("\n\nChoose a Option (9 = for exit): ");
                    writebuffer = System.Console.ReadLine();
                } while (!Int32.TryParse(writebuffer, out parsedvalue) || parsedvalue > Options.Length);

                writebuffer = String.Empty;
                switch (parsedvalue)
                {
                    case 0: await new GamelistDB.IGDBWrappers.AddIGDBReferences(ref igdb).RunAsync(); break;
                    case 1: await new GamelistDB.IGDBWrappers.AddIGDBScores(ref igdb).RunAsync(); break;
                    case 2: await new GamelistDB.IGDBWrappers.AddIGDBReleaseYear(ref igdb).RunAsync(); break;
            //        case 3: await new GamelistDB.HLTBWrappers.AddHLTBStats().RunAsync(); break;
                    case 4: await new GamelistDB.IGDBWrappers.JSONExporter(ref igdb,config.LIST_DEFAULT_OUTPUT).RunAsync(); break;
                    case 9: exit = true; break;
                    default: break;
                }
            }
        }

        static void ReadConfig()
        {
            if (File.Exists(Directory.GetCurrentDirectory() + "/config.json"))
            {
                GamelistDB.Utils.Log(("Reading config file:" + Directory.GetCurrentDirectory() + "/config.json"));
                config = JObject.Parse(File.ReadAllText(Directory.GetCurrentDirectory() + "/config.json")).ToObject<DTO.Config>();
                if (config != null)
                {
                    GamelistDB.Utils.Log("Config file read successfully\n");
                    igdb = new IGDB.IGDBClient(config.IGDB_CLIENT_ID, config.IGDB_CLIENT_SECRET);
                }
            }
            else if (Environment.GetEnvironmentVariable("IGDB_CLIENT_ID").Length != 0 && Environment.GetEnvironmentVariable("IGDB_CLIENT_SECRET").Length != 0)
            {
                igdb = new IGDB.IGDBClient(
                // Found in Twitch Developer portal for your app
                Environment.GetEnvironmentVariable("IGDB_CLIENT_ID"),
                Environment.GetEnvironmentVariable("IGDB_CLIENT_SECRET")
                );
            }
            else
            {
                GamelistDB.Utils.Log(
                    "It doesn't exist config file: " +
                    Directory.GetCurrentDirectory() + "/config.json" +
                    "\nPlease check the config file location");
                    System.Environment.Exit(-1);
            }
        }

    }
}
