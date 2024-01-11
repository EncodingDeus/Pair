using Dobrozaur.Core;

namespace Dobrozaur.Manager.GameState
{
    public class GameStateBase : IState
    {
        public virtual void OnEnter(FSM fsm)
        {
        }

        public virtual void OnExit(FSM fsm)
        {
        }
    }
}