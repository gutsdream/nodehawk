using System;
using Application.Core.Shared.Interfaces;

namespace Domain.Interfaces
{
    public interface IActiveJob
    {
        Guid Id { get; }
        string Title { get; }
        string Status { get; }
        JobType JobType { get; }
    }

    public enum JobType
    {
        Snapshot,
        CleanNode,
        Backup
    }
}