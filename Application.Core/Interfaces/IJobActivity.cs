using System;

namespace Application.Core.Interfaces
{
    public interface IJobActivity
    {
        Guid Id { get; }
        string Title { get; }
        string Status { get; }
    }
}