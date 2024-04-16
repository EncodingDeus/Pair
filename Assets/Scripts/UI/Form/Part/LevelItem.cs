using Cysharp.Threading.Tasks;
using Dobrozaur.Gameplay;
using Dobrozaur.Manager;
using Dobrozaur.UI.Form;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Form.Part
{
    public class LevelItem : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image[] starsImages;
        [SerializeField] private TMP_Text text;
        [SerializeField] private Image lockImage;
        [SerializeField] private CanvasGroup canvasGroup;

        private UIManager _uiManager;

        
        public void Init(Level level, UIManager uiManager)
        {
            _uiManager = uiManager;
            text.text = level.Setting.Number.ToString();
            text.gameObject.SetActive(!level.IsLocked);
            lockImage.gameObject.SetActive(level.IsLocked);
            canvasGroup.alpha = level.IsLocked ? 0.3f : 1f ;

            if (level.CompleteInfo != null)
            {
                for (int i = 0; i < level.CompleteInfo.Stars; i++)
                {
                    starsImages[i].gameObject.SetActive(true);
                }
            }

            if (!level.IsLocked)
            {
                button.onClick.AddListener(() =>
                {
                    _uiManager.OpenUIFormAsync<GameForm>(level).Forget();
                    _uiManager.CloseUIForm<LevelsForm>();
                });
            }
        }
    }
}