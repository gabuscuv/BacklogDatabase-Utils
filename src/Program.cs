using System;
using System.IO;
using System.Threading.Tasks;
using CommandLine;
using GameListDB.DTO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GameListDB
{
    class Program
    {
        // TODO: Dirty, I should make a Dependency Injection.

        private static ServiceProvider serviceProvider;        
        private static DTO.Options options;
        static readonly string[] Options = ["Add IGDB references", "Add Scores from IGDB", "Add Year Release from IGDB", "Add HowLongToBeat Stats", "Export GamesCompleted/Beaten", "", "", "", "", "Quit"];
        static string bufferwriter;
        static bool exit = false;

        

        public static async Task Main(string[] args)
        {
            
            Parser.Default.ParseArguments<Options>(args)
                    .WithParsed<Options>(o =>
                       {
                           DTO.Config config = ConfigUtils.ReadConfig(o.Verbose);
                            serviceProvider = new ServiceCollection()
                            // Services
                            
                            .AddSingleton<IGDB.IGDBClient>(new IGDB.IGDBClient(config.IGDB_CLIENT_ID, config.IGDB_CLIENT_SECRET))
                            .AddSingleton<Config>(config)                            
                            .AddSingleton<Options>(o)
                            // Applications
                            .AddTransient<IGDBWrappers.AddIGDBReferences>()
                            .AddTransient<IGDBWrappers.AddIGDBScores>()
                            .AddTransient<IGDBWrappers.AddIGDBReleaseYear>()
                            .AddTransient<HLTBWrappers.AddHLTBStats>()
                            .AddTransient<IGDBWrappers.JSONExporter>()
                            .BuildServiceProvider();
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
                    case 0: await serviceProvider.GetRequiredService<IGDBWrappers.AddIGDBReferences>().RunAsync(); break;
                    case 1: await serviceProvider.GetRequiredService<IGDBWrappers.AddIGDBScores>().RunAsync(); break;
                    case 2: await serviceProvider.GetRequiredService<IGDBWrappers.AddIGDBReleaseYear>().RunAsync(); break;
                    case 3: await serviceProvider.GetRequiredService<HLTBWrappers.AddHLTBStats>().RunAsync(); break;
                    case 4: await serviceProvider.GetRequiredService<IGDBWrappers.JSONExporter>().RunAsync(); break;
                    case 9: exit = true; break;
                    default: break;
                }
            }
        }


    }
}
