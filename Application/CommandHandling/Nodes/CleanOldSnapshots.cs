using System;
using System.Linq;
using Application.Models.Requests;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.CommandHandling.Nodes
{
    public class CleanOldSnapshots
    {
        public class Command : ValidatableCommand<Command, Command.Validator>
        {
            public TimeSpan CleanBefore { get; set; }

            public class Validator : AbstractValidator<Command>
            {
                public Validator( )
                {
                    RuleFor( x => x.CleanBefore ).NotEqual( ( TimeSpan ) default );
                }
            }
        }
    }

    public class CleanOldSnapshotsHandler : ValidatableCommandHandler<CleanOldSnapshots.Command, CleanOldSnapshots.Command.Validator>
    {
        public CleanOldSnapshotsHandler( DataContext repository )
        {
            OnSuccessfulValidation( async x =>
            {
                var removeBeforeDate = DateTime.UtcNow.Add( x.CleanBefore * -1 );
                var snapshotsToRemove = await repository.NodeSnapshots.Where( n => n.CreatedDateUtc <= removeBeforeDate ).ToListAsync( );

                foreach ( var snapshot in snapshotsToRemove )
                {
                    repository.Remove( snapshot );
                }

                await repository.SaveChangesAsync( );
            } );
        }
    }
}