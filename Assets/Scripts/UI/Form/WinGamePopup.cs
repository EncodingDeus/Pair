using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Dobrozaur.Gameplay;
using Dobrozaur.UI.Form;
using TMPro;
using UI.Form;
using UnityEngine;
using UnityEngine.UI;

namespace Dobrozaur
{
    public class WinGamePopup : UIForm
    {
        private const string NotPerfectCompleteText = "Уровень \nпройден";
        private const string PerfectCompleteText = "Идеально!";
        
        [SerializeField] private Image[] starImages;
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private Button restartLevelButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private TMP_Text levelCompleteText;
        [SerializeField] private TMP_Text notPerfectCompleteText;

        private PopupParams _popupParams;

        public override void OnInit(object userData)
        {
            base.OnInit(userData);

            nextLevelButton.onClick.AddListener(() => { });
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

            ResetPopup();
            ShowInfo(_popupParams.Level.CompleteInfo);
        }

        private void ShowInfo(Level.LevelCompleteInfo completeInfo)
        {
            for (int i = 0; i < completeInfo.Stars; i++)
            {
                starImages[i].enabled = true;
            }

            levelCompleteText.text = completeInfo.IsPerfect ? PerfectCompleteText : NotPerfectCompleteText;
            notPerfectCompleteText.gameObject.SetActive(!completeInfo.IsPerfect);
        }

        private void ResetPopup()
        {
            for (int i = 0; i < starImages.Length; i++)
            {
                starImages[i].enabled = false;
            }
        }

        public class PopupParams
        {
            public Level Level;
            public GameForm GameForm;
        }
    }
}