using System.Collections.Generic;
using System.Linq;
using GameListDB.Model;

namespace GameListDB.Model.Extensions
    {
    public static class GameListExtensions
    {
        /// <summary>
        /// Gather from the database any Games without a score setted (A null value))
        /// </summary>
        /// <param name="gamelistdb"></param>
        /// <returns></returns>
        public static IList<Backlog> GetMissingScores(this GameListsContext gamelistdb)
        {
            return gamelistdb.Backlogs.Where(element=> element.Score == null || element.Score == 0).ToList();
        }

        /// <summary>
        /// Get from the database all the finished games from a year specifed by param.
        /// </summary>
        /// <param name="gamelistdb">The GameListDatabaseContext to search (not needed as extension)</param>
        /// <param name="year">The year to look up.</param>
        /// <returns></returns>
        public static IList<Backlog> GetBeatenGames(this GameListsContext gamelistdb, int year)
        {
            return gamelistdb.Backlogs.Where(element=>element.Completedyear == year).OrderBy(element=>element.Name).ToList();
        }

        public static IList<Backlog> GetMissingReleaseYear(this GameListsContext gamelistdb)
        {
            return gamelistdb.Backlogs.Where(element=> element.Releaseyear == null || element.Releaseyear == 0).ToList();
        }


        public static IList<Backlog> GetMissingHLTB(this GameListsContext gamelistdb)
        {
            return gamelistdb.Backlogs.Where(element=> 
                                            element.MinTime == null ||
                                            element.MinTime == 0    || 
                                            element.MaxTime == null || 
                                            element.MaxTime == 0
                                            ).ToList();
        }


        public static long GetIgdbId(this GameListsContext gamelistdb, Backlog game)
        {
            return gamelistdb.GamesIds
                            .Where(element => element.IgdbId != null)
                            .Where(element => element.Id == game.Id)
                            .First()
                            .IgdbId.GetValueOrDefault();
        }

        public static IList<Backlog> GetUnbeatenTopScoredGames(this GameListsContext gamelistdb, int startIndex = 0, int length = 10)
        {
            return gamelistdb.Backlogs.Where(game=> game.Beaten != null && game.Beaten == 0 && game.Score != null).OrderByDescending(game=> game.Score).Skip(startIndex).Take<Backlog>(length).ToList();
        }

        public static IList<Backlog> GetUnbeatenTop10PrioritesGames(this GameListsContext gamelistdb, int startIndex = 0, int length = 10)
        {
            return gamelistdb.Backlogs.Where(
                                            game=> game.Beaten != null && 
                                            game.Beaten == 0 &&
                                            game.Score != null &&
                                            game.Priority < 3
                                            )
                                            .OrderBy(game=>game.Name)
                                            .OrderBy(game=>game.Priority)
                                            .Skip(startIndex)
                                            .Take<Backlog>(length).ToList();
        }
    }
    
    }

