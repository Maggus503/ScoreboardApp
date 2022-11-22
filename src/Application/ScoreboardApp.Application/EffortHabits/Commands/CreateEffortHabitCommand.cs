﻿using AutoMapper;
using MediatR;
using ScoreboardApp.Application.Commons.Interfaces;
using ScoreboardApp.Application.Commons.Mappings;
using ScoreboardApp.Application.DTOs.Enums;
using ScoreboardApp.Domain.Entities;

namespace ScoreboardApp.Application.Habits.Commands
{
    public sealed record CreateEfforHabitCommand : IRequest<CreateEfforHabitCommandResponse>
    {
        public string Title { get; init; } = default!;
        public string Description { get; init; } = default!;

        public string? Unit { get; init; } = default!;
        public double? AverageGoal { get; init; }

        public EffortHabitSubtypeMapping Subtype { get; init; }
        public Guid HabitTrackerId { get; init; }
    }

    public sealed record CreateEfforHabitCommandResponse : IMapFrom<EffortHabit>
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = default!;
        public string Description { get; init; } = default!;

        public string? Unit { get; init; } = default!;
        public double? AverageGoal { get; init; }

        public EffortHabitSubtypeMapping Subtype { get; init; }
        public Guid HabitTrackerId { get; init; }
    }

    public sealed class CreateEfforHabitCommandHandler : IRequestHandler<CreateEfforHabitCommand, CreateEfforHabitCommandResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CreateEfforHabitCommandHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CreateEfforHabitCommandResponse> Handle(CreateEfforHabitCommand request, CancellationToken cancellationToken)
        {
            var habitEntity = new EffortHabit()
            {
                Title = request.Title,
                Description = request.Description,
                HabitTrackerId = request.HabitTrackerId,
                Unit = request.Unit,
                AverageGoal = request.AverageGoal
            };

            _context.EffortHabits.Add(habitEntity);

            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<CreateEfforHabitCommandResponse>(habitEntity);
        }
    }
}