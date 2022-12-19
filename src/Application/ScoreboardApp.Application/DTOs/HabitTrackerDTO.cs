﻿using ScoreboardApp.Application.Commons.Mappings;
using ScoreboardApp.Application.DTOs.Enums;
using ScoreboardApp.Application.HabitTrackers.DTOs;
using ScoreboardApp.Domain.Entities;

namespace ScoreboardApp.Application.DTOs
{
    public sealed class HabitTrackerDTO : IMapFrom<HabitTracker>
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public PriorityMapping Priority { get; set; }

        public IList<CompletionHabitDTO> CompletionHabits { get; private set; } = new List<CompletionHabitDTO>();
        public IList<EffortHabitDTO> EffortHabits { get; private set; } = new List<EffortHabitDTO>();
    }
}
