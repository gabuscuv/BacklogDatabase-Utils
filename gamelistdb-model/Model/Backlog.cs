using System;
using System.Collections.Generic;

namespace GameListDB.Model;

public partial class Backlog
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Plataform { get; set; }

    public long? Score { get; set; }

    public long? Releaseyear { get; set; }

    public long? Nsfw { get; set; }

    public string? Status { get; set; }

    public long? Priority { get; set; }

    public long? Beaten { get; set; }

    public long? Completed { get; set; }

    public long? Completedyear { get; set; }

    public string? CurrentTime { get; set; }

    public double? MinTime { get; set; }

    public double? MaxTime { get; set; }

    public long? GameSeriesId { get; set; }

    public string? Playsite { get; set; }

    public long? Dependence { get; set; }

    public string? WhenStart { get; set; }

    public string? Notes { get; set; }
    
    // These two lines needed for CI Build Test but Crash in real life. WTH
    
    public virtual Backlog? DependenceNavigation { get; set; }

    public virtual Backlog? InverseDependenceNavigation { get; set; }
}
