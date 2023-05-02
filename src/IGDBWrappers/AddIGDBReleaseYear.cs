using IGDB;
using IGDB.Models;

using GameListDB.Model;
using GameListDB.Model.Extensions;

using System.Linq;
using System.Threading.Tasks;

namespace GamelistDB.IGDBWrappers
{

    public class AddIGDBReleaseYear : IGDBQueryBase
    {

        ReleaseDate[] DateList;

        public AddIGDBReleaseYear(ref IGDBClient igdb) : base(ref igdb){}
        public async Task RunAsync()
        {
            foreach (Backlog game in gamelistdb.GetMissingReleaseYear())
            {
                if (gamelistdb.GamesIds.Where(element => element.Id == game.Id && element.IgdbId.HasValue).Count() == 0) { continue; }
                System.Console.WriteLine("Getting releaseyear of " + game.Name);
                DateList = await this.RequestReleaseDateQuery("y",gamelistdb.GetIgdbId(game)); // Y means year, 
                if (DateList.Length == 0 || ! DateList.FirstOrDefault().Year.HasValue){System.Console.WriteLine("It doesn't have releaseyear ");continue;}
                game.Releaseyear = DateList.FirstOrDefault().Year;
                gamelistdb.Backlogs.Update(game);
            }

            gamelistdb.SaveChanges();

        }


    }
}