using IGDB;
using IGDB.Models;

using GameListDB.Model;
using GameListDB.Model.Extensions;
using System;
using System.Linq;
using System.Data;
using System.Threading.Tasks;

using GameListDB.IGDBIntegration;

namespace GameListDB.IGDBWrappers
{

    public class IGDBWrapperBase
    {
        protected IGDBQueryBase igdb;
        protected GameListsContext gamelistdb;

        public IGDBWrapperBase(IGDBClient _igdb, GameListsContext gameListsContext)
        {
            igdb = new IGDBQueryBase(ref _igdb);
            gamelistdb = gameListsContext;
        }

    }
}