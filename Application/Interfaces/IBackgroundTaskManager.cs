using System;

namespace Application.Interfaces
{
    public interface IBackgroundTaskManager
    {
        void CreateNodeSnapshot( Guid nodeId );
    }
}