﻿using AutoMapper;
using MediatR;
using ScoreboardApp.Application.Commons.Exceptions;
using ScoreboardApp.Application.DTOs;
using ScoreboardApp.Application.HabitTrackers.DTOs;
using ScoreboardApp.Application.HabitTrackers.Queries;
using ScoreboardApp.Domain.Entities;
using ScoreboardApp.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreboardApp.Application.EffortHabits.Queries
{
    public sealed record GetEffortHabitQuery : IRequest<GetEffortHabitQueryResponse>
    {
        public Guid Id { get; init; }
    }

    public sealed record GetEffortHabitQueryResponse
    {
        public EffortHabitDTO EffortHabit { get; init; }
    }


    public sealed class GetEffortHabitQueryHandler : IRequestHandler<GetEffortHabitQuery, GetEffortHabitQueryResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetEffortHabitQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<GetEffortHabitQueryResponse> Handle(GetEffortHabitQuery request, CancellationToken cancellationToken)
        {
            var effortHabitEntity = await _context.EffortHabits.FindAsync(new object[] { request.Id }, cancellationToken);

            if (effortHabitEntity == null)
            {
                throw new NotFoundException(nameof(HabitTracker), request.Id);
            }

            return new GetEffortHabitQueryResponse()
            {
                EffortHabit = _mapper.Map<EffortHabitDTO>(effortHabitEntity)
            };
        }
    }
}
