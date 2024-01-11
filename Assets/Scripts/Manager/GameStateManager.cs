using System;
using Dobrozaur.Core;
using Dobrozaur.Manager.GameState;

namespace Dobrozaur.Manager
{
    public class GameStateManager : Core.Manager
    {
        private readonly UIManager _uiManager;
        private readonly NetworkManager _networkManager;

        private readonly IState[] _states;
        private readonly FSM _gameFsm;
        
        
        public GameStateManager(UIManager uiManager, NetworkManager networkManager)
        {
            _uiManager = uiManager;
            _networkManager = networkManager;
            
            _states = new IState[]
            {
                new PreloadResourcesState(),
                new MainMenuState(_uiManager, _networkManager),
            };
            _gameFsm = new FSM(_states);
        }

        public override void Initialize()
        {
            _gameFsm.SetEntryState<PreloadResourcesState>();
            _gameFsm.Start();
        }
    }
}