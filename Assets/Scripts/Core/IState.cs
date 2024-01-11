namespace Dobrozaur.Core
{
    public interface IState
    {
        public void OnEnter(FSM fsm);
        public void OnExit(FSM fsm);
    }
}