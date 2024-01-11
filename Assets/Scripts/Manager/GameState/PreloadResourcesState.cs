using Dobrozaur.Core;

namespace Dobrozaur.Manager.GameState
{
    public class PreloadResourcesState : GameStateBase
    {
        public override void OnEnter(FSM fsm)
        {
            fsm.ChangeState<MainMenuState>();
        }

        public override void OnExit(FSM fsm)
        {
        }
    }
}