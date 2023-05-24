using System;
using System.IO;
using System.Threading.Tasks;
using CommandLine;
using GameListDB.DTO;

namespace GameListDB
{
    class Program
    {
        // TODO: Dirty, I should make a Dependency Injection.
        private static IGDB.IGDBClient igdb;
        private static DTO.Config config;
        private static DTO.Options options;
        static string[] Options = { "Add IGDB references", "Add Scores from IGDB", "Add Year Release from IGDB", "Add HowLongToBeat Stats", "Export GamesCompleted/Beaten", "", "", "", "", "Quit" };
        static string bufferwriter;
        static bool exit = false;
        public static async Task Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                    .WithParsed<Options>(o =>
                       {
                           config = GameListDB.ConfigUtils.ReadConfig(o.Verbose);
                           igdb = new IGDB.IGDBClient(config.IGDB_CLIENT_ID, config.IGDB_CLIENT_SECRET);
                           options = o;
                       }
                    ).WithNotParsed(e =>
                        {
                            Utils.WriteSection();
                            foreach (var a in e)
                            {
                                a.ToString();
                            }
                            System.Environment.Exit(-1);
                        }
                        );


            while (!exit)
            {
                if (options.Force)
                {
                    Utils.WriteSection();
                    System.Console.Write("WARNING: FORCE MODE");
                    Utils.WriteSection();
                }

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
                    if (!String.IsNullOrEmpty(bufferwriter)) { System.Console.WriteLine("\nPardon, Could You write again please?"); }
                    System.Console.Write("\n\nChoose a Option (9 = for exit): ");
                    bufferwriter = System.Console.ReadLine();
                } while (!Int32.TryParse(bufferwriter, out parsedvalue) || parsedvalue > Options.Length);

                bufferwriter = String.Empty;
                switch (parsedvalue)
                {
                    case 0: await new GameListDB.IGDBWrappers.AddIGDBReferences(ref igdb, options).RunAsync(); break;
                    case 1: await new GameListDB.IGDBWrappers.AddIGDBScores(ref igdb, options).RunAsync(); break;
                    case 2: await new GameListDB.IGDBWrappers.AddIGDBReleaseYear(ref igdb, options).RunAsync(); break;
                    case 3: await new GameListDB.HLTBWrappers.AddHLTBStats().RunAsync(); break;
                    case 4: await new GameListDB.IGDBWrappers.JSONExporter(ref igdb, config.LIST_DEFAULT_OUTPUT, options).RunAsync(); break;
                    case 9: exit = true; break;
                    default: break;
                }
            }
        }


    }
}
