﻿using ScoreboardApp.Domain.Entities.Commons;

namespace ScoreboardApp.Domain.Entities
{
    internal class CompletionHabit : Habit
    {
        public IList<CompletionHabitEntry> HabitEntries { get; set; } = new List<CompletionHabitEntry>();
    }
}
