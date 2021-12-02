using System;

namespace Application.Core.JobState
{
    public interface IJobActivity
    {
        Guid Id { get; }
        string Title { get; }
        string Status { get; }
    }
}