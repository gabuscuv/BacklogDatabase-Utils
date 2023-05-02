using IGDB;
using IGDB.Models;

using GameListDB.Model;
using GameListDB.Model.Extensions;
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
            gamelistdb = new GameListDB.Model.GameListsContext();
        }

        ~IGDBQueryBase()
        {
            gamelistdb.Dispose();
        }

        protected async Task<Game[]> RequestQuery(string query,long id)
        {
            try{
            return await igdb.QueryAsync<Game>(IGDBClient.Endpoints.Games, query: "fields "+ query +"; where id =" + id + ";");
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
        

    }
}