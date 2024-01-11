using Cysharp.Threading.Tasks;
using Dobrozaur.Gameplay;
using Dobrozaur.Manager;
using UI.Form;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dobrozaur.UI.Form
{
    public class GameForm : UIForm
    {
        [SerializeField] private Button backButton;

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
            _cardController.Init(_level.Rows, _level.Columns, _level.IdenticalCards);
            _cardController.GameFinished += OnGameFinished;
            backButton.onClick.AddListener(OpenStages);
        }
        
        public override void OnClose(object userData)
        {
            base.OnClose(userData);
            
            _cardController.ClearCards();
            _cardController.GameFinished -= OnGameFinished;
            backButton.onClick.RemoveListener(OpenStages);
        }

        private void OnGameFinished()
        {
            OpenStages(1000).Forget();
        }

        private async UniTaskVoid OpenStages(int time)
        {
            _level.CompleteLevel(3);
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