using IGDB;
using IGDB.Models;

using gamelist_db.Model;
using System; // System.String
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks; // Tasks

namespace GamelistDB.IGDBWrappers
{

    public class AddIGDBReferences : IGDBQueryBase
    {
        private const int limitOfEnumerate = 20;
        String writebuffer; 
        int parsedvalue;

        public AddIGDBReferences(ref IGDBClient igdb) : base(ref igdb){}
        public async Task RunAsync()
        {
            System.Console.WriteLine("Starting Searching");
             foreach (Backlog gamemissingid in GetMissingEntries())
            {
                Utils.WriteSection();
                System.Console.WriteLine("Starting Searching: "+gamemissingid.Name);

                if(await TryAdd(gamemissingid, await GetNames(gamemissingid.Name))) { gamelistdb.SaveChanges();continue;}
                
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
            

                if (gamelist != null && gamelist.Count() == 0){return false;}

                if (gamelist.Count() == 1 && gamelist.First() != null)
                {
                    System.Console.WriteLine("Adding " + gamelist.First().Name);
                    gamelistdb.GamesIds.Add(new GamesId() { Id = gamemissingid.Id, IgdbId = gamelist.First().Id });
                    Utils.WriteSection();

                    return true;
                }

                for (int counter = 0; counter < limitOfEnumerate && counter < gamelist.Count(); counter++)
                {
                    System.Console.WriteLine("[" + counter + "]" + gamelist.ElementAt(counter).Name + '('+ await getReleaseTimeFormatted(gamelist.ElementAt(counter)) +")" /*+ "By:"+gameList[counter].InvolvedCompanies.Values.ToString()*/);
                }

                do{
                    if (! String.IsNullOrEmpty(writebuffer)){System.Console.WriteLine("\nPardon, Could You write again please?");}
                    System.Console.Write("\n\nChoose the correct game: ");
                    writebuffer = System.Console.ReadLine();
                }while(! Int32.TryParse(writebuffer, out parsedvalue) || parsedvalue > gamelist.Count());

                writebuffer = String.Empty;
                
                if (parsedvalue < 0) {return false;}

                gamelistdb.GamesIds.Add(new GamesId() { Id = gamemissingid.Id, IgdbId = gamelist.ElementAt(parsedvalue).Id });
                Utils.WriteSection();
                return true;
        }

        public async Task<IEnumerable<Game>> GetNames(string name)
        {
            try{
                return await igdb.QueryAsync<Game>(IGDBClient.Endpoints.Games, 
                query: "fields id,name,created_at,involved_companies;"+
                ""+
                "where"
                + " name ~ *\"" + name + "\"*"+
//                "| alternative_names ~ *\"" + gamemissingid.Name + "\"*"+
                " & (category ="   +    (long)IGDB.Models.Category.MainGame +
                    " | category ="+    (long)IGDB.Models.Category.Expansion +
                    " | category ="+    (long)IGDB.Models.Category.StandaloneExpansion +
                    " | category ="+    (long)IGDB.Models.Category.Bundle +
                    " | category ="+    (long)IGDB.Models.Category.ExpandedGame +
                    " | category ="+    (long)IGDB.Models.Category.Remake +
                    " | category ="+    (long)IGDB.Models.Category.Remaster +
                ")"+
                //"and "+
                ";"+
                "limit "+limitOfEnumerate+";");
                }
                catch(RestEase.ApiException e)
                {
                    Console.WriteLine("[ERR]:"+e.Content);
                    return null;
                }
        }


        public async Task<IEnumerable<Game>> GetAlternativeNames(string alternativename)
        {
            try{
            return (await igdb.QueryAsync<AlternativeName>(IGDBClient.Endpoints.AlternativeNames, query: 
            "fields id,name,game;"+"where name ~ *\"" + alternativename + "\"*;")).Select(element=>element.Game.Value);
            }  
            catch(RestEase.ApiException e)
            {
                Console.WriteLine("[ERR]:"+e.Content);
                return null;
            }         
                        
        }

        public IList<Backlog> GetMissingEntries()
        {
            return gamelistdb.Backlogs.Where(element=>! this.gamelistdb.GamesIds.Select(element2=>element2.Id).Contains((int)element.Id)).ToList();
        }

        public async Task<string> getReleaseTimeFormatted(Game request)
        {
            var tmp = await this.RequestReleaseDateQuery("y",request);
            if(tmp.Length != 0 ) {return tmp[0].Year.ToString();}
            return "????";
        }
    }

}