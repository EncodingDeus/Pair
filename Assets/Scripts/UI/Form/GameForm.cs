using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Dobrozaur.Gameplay;
using Dobrozaur.Manager;
using TMPro;
using UI.Form;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dobrozaur.UI.Form
{
    public class GameForm : UIForm
    {
        [SerializeField] private Button backButton;
        [SerializeField] private GameProgress gameProgress;
        
        private Level _level;
        private CardController _cardController;

        [Inject]
        public void Construct(CardController cardController)
        {
            _cardController = cardController;
        }
        
        public override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            _level = (Level)userData;
            
            _cardController.Init(_level.Setting);
            _cardController.GameFinished += OnGameFinished;
            _cardController.CardsChecked += OnCardsChecked;

            gameProgress.Init(
                _level.Setting.MaxMisses, 
                _level.Setting.CompleteSettings[1],
                _level.Setting.CompleteSettings[2], 
                _level.Setting.CompleteSettings[3]);
            
            backButton.onClick.AddListener(OpenStages);
        }
        
        public override void OnClose(object userData)
        {
            base.OnClose(userData);

            _cardController.Dispose();
            _cardController.ClearCards();
            _cardController.GameFinished -= OnGameFinished;
            _cardController.CardsChecked -= OnCardsChecked;

            backButton.onClick.RemoveListener(OpenStages);
        }

        public void RestartGame()
        {
            _cardController.Init(_level.Setting);
            gameProgress.SetMisses(0);
        }
        
        private void OnCardsChecked(IEnumerable<Card> cards)
        {
            gameProgress.SetMisses(_cardController.Misses);
            
            if (_cardController.Misses > _level.Setting.MaxMisses)
            {
                FinishGame(false);
            }
        }

        private void OnGameFinished()
        {
            FinishGame(true);
        }

        private void FinishGame(bool win)
        {
            _cardController.SetPause(true);
            _level.CompleteLevel(_cardController.Attempts, _cardController.Misses);
            
            if (win)
            {
                OpenWinPopup(1000).Forget();
            }
            else
            {
                OpenGameOverPopup(1000).Forget();
            }
        }

        private async UniTaskVoid OpenWinPopup(int time)
        {
            await UniTask.Delay(time);
            UIManager.OpenUIFormAsync<WinGamePopup>(new WinGamePopup.PopupParams()
            {
                GameForm = this,
                Level = _level,
            }).Forget();
            // Close();
        }

        private async UniTaskVoid OpenGameOverPopup(int time)
        {
            await UniTask.Delay(time);
            UIManager.OpenUIFormAsync<GameOverPopup>(new GameOverPopup.PopupParams()
            {
                GameForm = this,
                Level = _level,
            }).Forget();
            // Close();
        }

        private async UniTaskVoid OpenStages(int time)
        {
            // _level.CompleteLevel(_cardController.Attempts, _cardController.Misses);
            await UniTask.Delay(time);

            OpenStages();
        }

        private void OpenStages()
        {
            UIManager.OpenUIFormAsync<StagesForm>().Forget();
            Close();
        }
    }
}