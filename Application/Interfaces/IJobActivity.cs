using System;

namespace Application.Interfaces
{
    public interface IJobActivity
    {
        Guid Id { get; }
        string Title { get; }
        string Status { get; }
    }
}