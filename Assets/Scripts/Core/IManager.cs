using System;

namespace Dobrozaur.Core
{
    public interface IManager
    {
        public void Init(Action<IManager> initCompleteAction);
    }
}