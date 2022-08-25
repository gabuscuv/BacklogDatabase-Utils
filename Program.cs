using System;
using System.Threading.Tasks;

namespace GamelistDB
{
    class Program
    {
        static string[] Options = {"Add IGDB references","Add Scores from IGDB","Add Year Release from IGDB"};
        static string writebuffer;
        static bool exit=false;
        public static async Task Main(string[] args)
        {
            while (!exit){
            for (int counter = 0; counter < Options.Length; counter++)
            {
                System.Console.WriteLine("\t[" + counter + "] - " + Options[counter]);
            }

            int parsedvalue;
            do{
                if (! String.IsNullOrEmpty(writebuffer)){System.Console.WriteLine("\nPardon, Could You write again please?");}
                    System.Console.Write("\n\nChoose a Option (9 = for exit): ");
                    writebuffer = System.Console.ReadLine();
                }while(! Int32.TryParse(writebuffer, out parsedvalue) || parsedvalue > Options.Length);

                writebuffer = String.Empty;
            switch(parsedvalue)
            {
                case 0: await new GamelistDB.AddIGDBReferences().RunAsync();break;
                case 1: await new GamelistDB.IGDBWrappers.AddIGDBScores().RunAsync();break;
                case 2: await new GamelistDB.IGDBWrappers.AddIGDBReleaseYear().RunAsync();break;
                case 9:  exit = true;break;
                default: break;
            }
            }
        }
    }
}
