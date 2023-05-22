using IGDB;
using IGDB.Models;

using GameListDB.Model;
using System.Linq;
using System.Threading.Tasks;
using GameListDB.Model.Extensions;

namespace GameListDB.IGDBWrappers
{

    public class AddIGDBScores : IGDBWrapperBase
    {

        Game[] gameList;

        public AddIGDBScores(ref IGDBClient igdb, GameListDB.DTO.Options options) : base(ref igdb){}
        public async Task RunAsync()
        {
            foreach (Backlog game in gamelistdb.GetMissingScores())
            {
                if (gamelistdb.GamesIds.Where(element => element.Id == game.Id).Count() == 0) { continue; }
                System.Console.WriteLine("Getting Score of " + game.Name);
                gameList = await igdb.RequestQuery("id,rating",gamelistdb.GetIgdbId(game));
                if (gameList.Length == 0 ||! gameList.FirstOrDefault().Rating.HasValue){System.Console.WriteLine("It doesn't have score");continue;}
                game.Score = (long)gameList.FirstOrDefault().Rating;
                gamelistdb.Backlogs.Update(game);

            }

            gamelistdb.SaveChanges();

        }


    }
}