﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using ScoreboardApp.Application.Commons.Interfaces;
using ScoreboardApp.Application.Commons.Mappings;
using ScoreboardApp.Application.Commons.Models;
using ScoreboardApp.Application.Commons.Queries;
using ScoreboardApp.Application.HabitTrackers.DTOs;

namespace ScoreboardApp.Application.CompletionHabitEntries.Queries
{
    public sealed record GetAllCompletionHabitEntriesWithPaginationQuery : IPagedQuery, IRequest<PaginatedList<CompletionHabitEntryDTO>>
    {
        public Guid HabitId { get; init; }
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 10;
    }

    public sealed class GetAllCompletionHabitEntriesWithPaginationQueryHandler : IRequestHandler<GetAllCompletionHabitEntriesWithPaginationQuery, PaginatedList<CompletionHabitEntryDTO>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public GetAllCompletionHabitEntriesWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper, ICurrentUserService currentUserService)
        {
            _context = context;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<PaginatedList<CompletionHabitEntryDTO>> Handle(GetAllCompletionHabitEntriesWithPaginationQuery request, CancellationToken cancellationToken)
        {
            string? currentUserId = _currentUserService.GetUserId()!;

            return await _context.CompletionHabitEntries
                                .Where(x => x.HabitId == request.HabitId && x.UserId == currentUserId)
                                .OrderBy(x => x.EntryDate)
                                .ProjectTo<CompletionHabitEntryDTO>(_mapper.ConfigurationProvider)
                                .PaginatedListAsync(request.PageNumber, request.PageSize);


        }
    }
}
