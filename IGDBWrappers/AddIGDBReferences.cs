using IGDB;
using IGDB.Models;

using gamelist_db.Model;
using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GamelistDB
{

    public class AddIGDBReferences : GamelistDB.IGDBWrappers.IGDBQueryBase
    {
        private const int limitOfEnumerate = 20;
        String writebuffer; 
        int parsedvalue;

        Game[] gameList;

        public AddIGDBReferences() : base(){}
        public async Task RunAsync()
        {
            System.Console.WriteLine("Starting Searching");
             foreach (Backlog gamemissingid in GetMissingEntries())
            {
                Utils.WriteSection();
                System.Console.WriteLine("Starting Searching: "+gamemissingid.Name);
                try{
                gameList = await igdb.QueryAsync<Game>(IGDBClient.Endpoints.Games, 
                query: "fields id,name,created_at,involved_companies;"+
                ""+
                "where name ~ *\"" + gamemissingid.Name + "\"*"+
                "& (category ="+(long)IGDB.Models.Category.MainGame+
                " | category ="+(long)IGDB.Models.Category.Expansion+
                " | category ="+(long)IGDB.Models.Category.Remake+
                " | category ="+(long)IGDB.Models.Category.Remaster+
                ");"+
                "limit "+limitOfEnumerate+";");
                }
                catch(RestEase.ApiException e)
                {
                    Console.WriteLine("[ERR]:"+e.Content);
                    continue;
                }


                if (gameList.Length == 0)
                {
                    System.Console.WriteLine("Not found " + gamemissingid.Name);
                    Utils.WriteSection();
                    
                    continue;
                }

                if (gameList.Length == 1)
                {
                    System.Console.WriteLine("Adding " + gameList.First().Name);
                    gamelistdb.GamesIds.Add(new GamesId() { Id = gamemissingid.Id, IgdbId = gameList.First().Id });
                    Utils.WriteSection();

                    continue;
                }

                for (int counter = 0; counter < limitOfEnumerate && counter < gameList.Length; counter++)
                {
                    System.Console.WriteLine("[" + counter + "]" + gameList[counter].Name + '('+ await getReleaseTimeFormatted(gameList[counter]) +")" /*+ "By:"+gameList[counter].InvolvedCompanies.Values.ToString()*/);
                }

                do{
                    if (! String.IsNullOrEmpty(writebuffer)){System.Console.WriteLine("\nPardon, Could You write again please?");}
                    System.Console.Write("\n\nChoose the correct game: ");
                    writebuffer = System.Console.ReadLine();
                }while(! Int32.TryParse(writebuffer, out parsedvalue) || parsedvalue > gameList.Length);

                writebuffer = String.Empty;
                
                if (parsedvalue < 0) {continue;}

                gamelistdb.GamesIds.Add(new GamesId() { Id = gamemissingid.Id, IgdbId = gameList[parsedvalue].Id });
                Utils.WriteSection();

            }

            await gamelistdb.SaveChangesAsync();
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