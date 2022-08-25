using IGDB;
using IGDB.Models;

using gamelist_db.Model;
using System.Linq;
using System.Data;
using System.Threading.Tasks;
using GamelistDB.Extensions;

namespace GamelistDB.IGDBWrappers
{

    public class AddIGDBScores : IGDBQueryBase
    {

        Game[] gameList;

        public AddIGDBScores() : base(){}
        public async Task RunAsync()
        {
            foreach (Backlog game in gamelistdb.GetMissingScores())
            {
                if (gamelistdb.GamesIds.Where(element => element.Id == game.Id).Count() == 0) { continue; }
                System.Console.WriteLine("Getting Score of " + game.Name);
                gameList = await this.RequestQuery("id,rating",game);
                if (gameList.Length == 0 ||! gameList.FirstOrDefault().Rating.HasValue){System.Console.WriteLine("It doesn't have score");continue;}
                game.Score = (long)gameList.FirstOrDefault().Rating;
                gamelistdb.Backlogs.Update(game);

            }

            gamelistdb.SaveChanges();

        }


    }
}