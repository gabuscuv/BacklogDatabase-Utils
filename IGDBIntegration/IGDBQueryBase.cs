using IGDB;
using IGDB.Models;

using System;
using System.Linq;
using System.Data;
using System.Threading.Tasks;

namespace GameListDB.IGDBIntegration
{

    public class IGDBQueryBase
    {
        private IGDBClient igdbClient{get;}

        public IGDBQueryBase(ref IGDBClient _igdb)
        {
            igdbClient = _igdb;
        }

        public async Task<Game[]> RequestQuery(string query,long id)
        {
            try{
            return await igdbClient.QueryAsync<Game>(IGDBClient.Endpoints.Games, query: "fields "+ query +"; where id =" + id + ";");
            }
            catch(RestEase.ApiException e)
            {
                Console.WriteLine("[ERR]:"+e.Content);
                return null;
            }
        }

        public async Task<IGDB.Models.ReleaseDate[]> RequestReleaseDateQuery(string query,Game game)
        {
            return await RequestReleaseDateQuery(query, (long)game.Id);
        }

        public async Task<IGDB.Models.ReleaseDate[]> RequestReleaseDateQuery(string query,long game)
        {
            try{
            return await this.igdbClient.QueryAsync<IGDB.Models.ReleaseDate>(IGDBClient.Endpoints.ReleaseDates, 
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
        
        public async Task<string> GetBoxArtURL(long igdbid)
        {
            return "http:" + (IGDB.ImageHelper.GetImageUrl(imageId: 
            (await this.RequestQuery("cover.image_id",igdbid)).First().Cover.Value.ImageId, size: ImageSize.CoverBig, retina: false)).ToString();
        }

        public async Task<IEnumerable<Game>> GetNames(string name, int limitOfEnumerate = 20)
        {
            try{
                return await this.igdbClient.QueryAsync<Game>(IGDBClient.Endpoints.Games, 
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
            return (await this.igdbClient.QueryAsync<AlternativeName>(IGDBClient.Endpoints.AlternativeNames, query: 
            "fields id,name,game;"+"where name ~ *\"" + alternativename + "\"*;")).Select(element=>element.Game.Value);
            }  
            catch(RestEase.ApiException e)
            {
                Console.WriteLine("[ERR]:"+e.Content);
                return null;
            }         
                        
        }

        public async Task<string> getReleaseTimeFormatted(Game request)
        {
            var tmp = await this.RequestReleaseDateQuery("y",request);
            if(tmp.Length != 0 ) {return tmp[0].Year.ToString();}
            return "????";
        }

    }
}