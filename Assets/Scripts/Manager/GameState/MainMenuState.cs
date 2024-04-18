using Cysharp.Threading.Tasks;
using Dobrozaur.Core;
using Dobrozaur.UI.Form;

namespace Dobrozaur.Manager.GameState
{
    public class MainMenuState : GameStateBase
    {
        private readonly UIManager _uiManager;
        private readonly NetworkManager _networkManager;
        private readonly GameManager _gameManager;

        public MainMenuState(UIManager uiManager, NetworkManager networkManager, GameManager gameManager)
        {
            _uiManager = uiManager;
            _networkManager = networkManager;
            _gameManager = gameManager;
        }
        
        public override void OnEnter(FSM fsm)
        {
            base.OnEnter(fsm);

            _gameManager.InitGames();
            _uiManager.OpenUIFormAsync<StagesForm>(_gameManager.PairData).Forget();
            // _networkManager.Auth().Forget();
        }
    }
}