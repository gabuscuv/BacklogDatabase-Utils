using HowLongToBeat;

using gamelist_db.Model;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading.Tasks;
using GamelistDB.Extensions;
using System.Collections.Generic;

namespace GamelistDB.HLTBWrappers
{

    public class AddHLTBStats
    {

        private GameListsContext gamelistdb;
        private Regex regex;
        private string writebuffer;
        private int parsedvalue;

        private static System.Net.Http.HttpClient Client = new System.Net.Http.HttpClient();

        public AddHLTBStats()
        {
            gamelistdb = new GameListsContext();
            regex = new Regex("([0-9])", RegexOptions.Compiled);
        }
        public async Task RunAsync()
        {
            var ws = new HLTBWebScraper(Client);
            IList<Game> gamelist;
            foreach (Backlog game in gamelistdb.GetMissingHLTB())
            {
                
                Utils.WriteSection();
                Utils.Log("Searching: "+game.Name);
                
                gamelist = await ws.Search(game.Name);

                if (gamelist.Count() == 0){Utils.Log(game.Name + " Not found"); continue;}

                if (gamelist.Count() == 1)
                {
                    AddToDatabase(game,gamelist.First());
                    continue;
                }

                for (int counter = 0;counter < gamelist.Count(); counter++)
                {
                    System.Console.WriteLine("[" + counter + "]" + gamelist.ElementAt(counter).Title);
                }

                do{
                    if (! System.String.IsNullOrEmpty(writebuffer)){System.Console.WriteLine("\nPardon, Could You write again please?");}
                    
                    System.Console.Write("\n\nChoose the correct game [0]: ");
                    writebuffer = System.Console.ReadLine();
                }while(! System.Int32.TryParse(writebuffer, out parsedvalue) || parsedvalue > gamelist.Count());

                if (parsedvalue == -1){continue;}

                writebuffer = System.String.Empty;

                AddToDatabase(game,gamelist.ElementAt(parsedvalue));
            }

            gamelistdb.SaveChanges();

            gamelistdb.Dispose();

        }

        private bool AddToDatabase(Backlog game, HowLongToBeat.Game hltbgame)
        {
            long output;
            
            if (System.Int64.TryParse(regex.Match(hltbgame.Main).Value, out output))
            {
                // FIXME: Database
                // My Table doesn't use REAL numbers (float integers compatible), 
                // So It will be rounding up, Until I fix this.
                if (hltbgame.Main.Contains('½'))
                {
                    output += 1;
                }

                game.MinTime = output;

            }

            if (System.Int64.TryParse(regex.Match(hltbgame.MainAndExtras).Value, out output))
            {
                // FIXME: Database
                // My Table doesn't use REAL numbers (float integers compatible), 
                // So It will be rounding up, Until I fix this.
                if (hltbgame.MainAndExtras.Contains('½'))
                {
                    output += 1;
                }

                game.MaxTime = output;
                
            }

            gamelistdb.Backlogs.Update(game);

            return true;
        }

    }
}