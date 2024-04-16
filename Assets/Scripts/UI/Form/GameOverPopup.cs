using Cysharp.Threading.Tasks;
using Dobrozaur.Gameplay;
using Dobrozaur.UI.Form;
using UI.Form;
using UnityEngine;
using UnityEngine.UI;

namespace Dobrozaur
{
    public class GameOverPopup : UIForm
    {
        [SerializeField] private Button restartLevelButton;
        [SerializeField] private Button exitButton;

        private PopupParams _popupParams;

        public override void OnInit(object userData)
        {
            base.OnInit(userData);

            restartLevelButton.onClick.AddListener(() =>
            {
                Close();
                _popupParams.GameForm.RestartGame();
            });
            exitButton.onClick.AddListener(() =>
            {
                Close();
                _popupParams.GameForm.Close();
                UIManager.OpenUIFormAsync<StagesForm>().Forget();
            });
        }

        public override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            _popupParams = (PopupParams)userData;
        }

        public class PopupParams
        {
            public Level Level;
            public GameForm GameForm;
        }
    }
}
