using System;
using System.Collections.Generic;

namespace GameListDB.Model;

public partial class Backlog
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Type { get; set; }

    public string? Plataform { get; set; }

    public int? Score { get; set; }

    public int? Releaseyear { get; set; }

    public int? Nsfw { get; set; }

    public string? Status { get; set; }

    public int? InfiniteGame { get; set; }

    public int? Priority { get; set; }

    public int? Beaten { get; set; }

    public int? Completed { get; set; }

    public string? Completedyear { get; set; }

    public string? CurrentTime { get; set; }

    public double? MinTime { get; set; }

    public double? MaxTime { get; set; }

    public string? GameSeriesId { get; set; }

    public string? Playsite { get; set; }

    public string? Dependence { get; set; }

    public string? WhenStart { get; set; }

    public string? Notes { get; set; }
}
