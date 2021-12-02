using System;

namespace Application.Core.JobManagement
{
    public interface IActiveJob
    {
        Guid Id { get; }
        string Title { get; }
        string Status { get; }
    }
}