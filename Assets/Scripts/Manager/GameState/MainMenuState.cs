using Cysharp.Threading.Tasks;
using Dobrozaur.Core;
using Dobrozaur.UI.Form;

namespace Dobrozaur.Manager.GameState
{
    public class MainMenuState : GameStateBase
    {
        private readonly UIManager _uiManager;
        private readonly NetworkManager _networkManager;

        public MainMenuState(UIManager uiManager, NetworkManager networkManager)
        {
            _uiManager = uiManager;
            _networkManager = networkManager;
        }
        
        public override void OnEnter(FSM fsm)
        {
            base.OnEnter(fsm);

            _uiManager.OpenUIFormAsync<StagesForm>().Forget();
            // _networkManager.Auth().Forget();
        }
    }
}