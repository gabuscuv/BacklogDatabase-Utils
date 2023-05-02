using System;
using System.Collections.Generic;

namespace GameListDB.Model;

public partial class GamesId
{
    public long Id { get; set; }

    public long? IgdbId { get; set; }

    public long? SteamId { get; set; }

    public long? PsnProfile { get; set; }

    public long? PsstoreId { get; set; }

    public virtual GamesId IdNavigation { get; set; } = null!;

    public virtual GamesId? InverseIdNavigation { get; set; }
}
