﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScoreboardApp.Domain.Entities;
using ScoreboardApp.Domain.Entities.Commons;
using ScoreboardApp.Domain.Enums;

namespace ScoreboardApp.Infrastructure.Persistence.Configurations
{
    public abstract class HabitEntryConfiguration<TEntry, THabit> : IEntityTypeConfiguration<TEntry>
        where TEntry : HabitEntry<THabit>
        where THabit : Habit<TEntry>
    {
        public virtual void Configure(EntityTypeBuilder<TEntry> builder)
        {
            builder.Property(e => e.HabitId)
                .IsRequired();

            builder.Property(e => e.EntryDate)
                .IsRequired();

        }
    }

    public class EffortHabitEntryConfiguration : HabitEntryConfiguration<EffortHabitEntry, EffortHabit>
    {
        public override void Configure(EntityTypeBuilder<EffortHabitEntry> builder)
        {
            base.Configure(builder);

            builder.Property(e => e.Effort)
                .IsRequired();
        }
    }

    public class CompletionHabitEntryConfiguration : HabitEntryConfiguration<CompletionHabitEntry, CompletionHabit>
    {
        public override void Configure(EntityTypeBuilder<CompletionHabitEntry> builder)
        {
            base.Configure(builder);
        }
    }
}