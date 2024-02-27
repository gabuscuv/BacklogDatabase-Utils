using System;
using System.Collections.Generic;

namespace GameListDB.Model;

public partial class GamesId
{
    public int Id { get; set; }

    public int? IgdbId { get; set; }

    public int? SteamId { get; set; }

    public int? PsnProfile { get; set; }

    public int? PsstoreId { get; set; }

    public virtual GamesId IdNavigation { get; set; } = null!;

    public virtual GamesId? InverseIdNavigation { get; set; }
}
