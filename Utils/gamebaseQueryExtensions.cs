using System.Collections.Generic;
using System.Linq;
using gamelist_db.Model;

namespace GamelistDB.Extensions
    {
    public static class GameListExtensions
    {
        public static IList<Backlog> GetMissingScores(this GameListsContext gamelistdb)
        {
            return gamelistdb.Backlogs.Where(element=> element.Score == null || element.Score == 0).ToList();
        }

        public static IList<Backlog> GetMissingReleaseYear(this GameListsContext gamelistdb)
        {
            return gamelistdb.Backlogs.Where(element=> element.Releaseyear == null || element.Releaseyear == 0).ToList();
        }

    }
    
    }

