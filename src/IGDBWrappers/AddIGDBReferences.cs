using IGDB;
using IGDB.Models;

using GameListDB.Model;
using System; // System.String
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks; // Tasks

namespace GameListDB.IGDBWrappers
{

    public class AddIGDBReferences : IGDBWrapperBase
    {
        private const int limitOfEnumerate = 20;
        String writebuffer;
        int parsedvalue;

        public AddIGDBReferences(IGDBClient igdb, GameListDB.DTO.Options options) : base(igdb) { }
        public async Task RunAsync()
        {
            System.Console.WriteLine("Starting Searching");
            foreach (Backlog gamemissingid in GetMissingEntries())
            {
                Utils.WriteSection();
                System.Console.WriteLine("Starting Searching: " + gamemissingid.Name);

                if (await TryAdd(gamemissingid, await igdb.GetNames(gamemissingid.Name, limitOfEnumerate))) { gamelistdb.SaveChanges(); continue; }

                /*                System.Console.WriteLine("Not found " + gamemissingid.Name + "\n Searching Alternative Names...");

                                if (await TryAdd(gamemissingid, await GetAlternativeNames(gamemissingid.Name))) {continue;}
                */
                System.Console.WriteLine("Not found " + gamemissingid.Name);

            }

            gamelistdb.SaveChanges();

            gamelistdb.Dispose();
        }

        public async Task<bool> TryAdd(Backlog gamemissingid, IEnumerable<Game> gamelist)
        {


            if (gamelist != null && gamelist.Count() == 0) { return false; }

            if (gamelist.Count() == 1 && gamelist.First() != null)
            {
                System.Console.WriteLine("Adding " + gamelist.First().Name);
                gamelistdb.GamesIds.Add(new GamesId() { Id = (int)gamemissingid.Id, IgdbId = (int)gamelist.First().Id });
                Utils.WriteSection();

                return true;
            }

            for (int counter = 0; counter < limitOfEnumerate && counter < gamelist.Count(); counter++)
            {
                System.Console.WriteLine("[" + counter + "]" + gamelist.ElementAt(counter).Name + '(' + await igdb.getReleaseTimeFormatted(gamelist.ElementAt(counter)) + ")" /*+ "By:"+gameList[counter].InvolvedCompanies.Values.ToString()*/);
            }

            do
            {
                if (!String.IsNullOrEmpty(writebuffer)) { System.Console.WriteLine("\nPardon, Could You write again please?"); }
                System.Console.Write("\n\nChoose the correct game: ");
                writebuffer = System.Console.ReadLine();
            } while (!Int32.TryParse(writebuffer, out parsedvalue) || parsedvalue > gamelist.Count());

            writebuffer = String.Empty;

            if (parsedvalue < 0) { return false; }

            gamelistdb.GamesIds.Add(new GamesId() { Id = gamemissingid.Id, IgdbId = (int)gamelist.ElementAt(parsedvalue).Id });
            Utils.WriteSection();
            return true;
        }



        public IList<Backlog> GetMissingEntries()
        {
            return gamelistdb.Backlogs.Where(element => !this.gamelistdb.GamesIds.Select(element2 => element2.Id).Contains((int)element.Id)).ToList();
        }

    }

}