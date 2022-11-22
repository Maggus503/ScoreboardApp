﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using ScoreboardApp.Application.Commons.Exceptions;
using ScoreboardApp.Application.Commons.Interfaces;
using ScoreboardApp.Domain.Entities;

namespace ScoreboardApp.Application.CompletionHabitEntries.Commands
{
    public sealed record DeleteCompletionHabitEntryCommand(Guid Id) : IRequest
    {
        public Guid Id { get; init; } = Id;
    }
    public class DeleteCompletionHabitEntryCommandHandler : IRequestHandler<DeleteCompletionHabitEntryCommand, Unit>
    {
        private readonly IApplicationDbContext _context;

        public DeleteCompletionHabitEntryCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteCompletionHabitEntryCommand request, CancellationToken cancellationToken)
        {
            var entryEntity = await _context.CompletionHabitEntries
                    .Where(h => h.Id == request.Id)
                    .SingleOrDefaultAsync(cancellationToken);

            if (entryEntity == null)
            {
                throw new NotFoundException(nameof(EffortHabitEntry), request.Id);
            }

            _context.CompletionHabitEntries.Remove(entryEntity);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
