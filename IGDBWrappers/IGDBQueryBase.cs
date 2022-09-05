using IGDB;
using IGDB.Models;

using gamelist_db.Model;
using System;
using System.Linq;
using System.Data;
using System.Threading.Tasks;

namespace GamelistDB.IGDBWrappers
{

    public class IGDBQueryBase
    {
        protected IGDBClient igdb;
        protected GameListsContext gamelistdb;

        public IGDBQueryBase(ref IGDBClient _igdb)
        {
            igdb = _igdb;
            gamelistdb = new gamelist_db.Model.GameListsContext();
        }

        protected async Task<Game[]> RequestQuery(string query,Backlog game)
        {
            try{
            return await igdb.QueryAsync<Game>(IGDBClient.Endpoints.Games, query: "fields "+ query +"; where id =" + GetIgdbId(game) + ";");
            }
            catch(RestEase.ApiException e)
            {
                Console.WriteLine("[ERR]:"+e.Content);
                return null;
            }
        }

        protected async Task<IGDB.Models.ReleaseDate[]> RequestReleaseDateQuery(string query,Game game)
        {
            return await RequestReleaseDateQuery(query, (long)game.Id);
        }


        protected async Task<IGDB.Models.ReleaseDate[]> RequestReleaseDateQuery(string query,Backlog game)
        {
            return await RequestReleaseDateQuery(query, GetIgdbId(game));
        }

        protected async Task<IGDB.Models.ReleaseDate[]> RequestReleaseDateQuery(string query,long game)
        {
            try{
            return await igdb.QueryAsync<IGDB.Models.ReleaseDate>(IGDBClient.Endpoints.ReleaseDates, 
                query: "fields "+ query +",region;sort region; where game =" + game + 
                    /*"& (region="+(byte)IGDB.Models.ReleaseDateRegion.Worldwide + 
                    " | region="+(byte)IGDB.Models.ReleaseDateRegion.Europe + 
                    " | region="+(byte)IGDB.Models.ReleaseDateRegion.Japan + ")"+*/
                    ";");
            }
            catch(RestEase.ApiException e)
            {
                Console.WriteLine("[ERR]:"+e.Content);
                return null;
            }
        }
        

        protected long GetIgdbId(Backlog game)
        {
            return (long) gamelistdb.GamesIds.Where(element=>element.IgdbId != null).Where(element => element.Id == game.Id).FirstOrDefault().IgdbId;
        }
    }
}