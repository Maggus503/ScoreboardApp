﻿namespace ScoreboardApp.Domain.Entities
{
    public interface IHabit<TEntry>
    {
        string? Description { get; set; }
        IList<TEntry> HabitEntries { get; set; }
        HabitTracker HabitTracker { get; set; }
        Guid HabitTrackerId { get; set; }
        string? Title { get; set; }
    }
}