using System.Linq;
using Cysharp.Threading.Tasks;
using Dobrozaur.Gameplay;
using Dobrozaur.Manager;
using TMPro;
using UI.Form;
using UnityEngine;
using UnityEngine.UI;

namespace Dobrozaur.UI.Form.Part
{
    public class StageCard : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text stageText;
        [SerializeField] private TMP_Text levelsText;
        [SerializeField] private TMP_Text starsText;
        [SerializeField] private Image stageImage;
        [SerializeField] private Image levelsImagePanel;
        [SerializeField] private GameObject lockedPanel;

        [SerializeField] private Color textUnlockedColor;
        [SerializeField] private Color textLockedColor;
        
        [SerializeField] private Color levelUnlockedColor;
        [SerializeField] private Color levelLockedColor;

        [SerializeField] private Sprite spriteUnlocked;
        [SerializeField] private Sprite spriteLocked;

        private UIManager _uiManager;

        public void Init(Stage stage, UIManager uiManager)
        {
            var completedLevels = stage.Levels.Count(l => l.IsCompleted);

            _uiManager = uiManager;
            stageText.text = $"{stage.StageNumber} этап";
            levelsText.text = $"{completedLevels}/{stage.Levels.Length}";
            starsText.text = $"{GameManager.Instance.GetStars()}/{stage.RequiredStars}";
            button.interactable = !stage.IsLocked;
            lockedPanel.SetActive(stage.IsLocked);
            
            if (stage.IsLocked)
            {
                stageText.color = textLockedColor;
                levelsImagePanel.color = levelLockedColor;
                stageImage.sprite = spriteLocked;
            }
            else
            {
                stageText.color = textUnlockedColor;
                levelsImagePanel.color = levelUnlockedColor;
                stageImage.sprite = spriteUnlocked;
            }
            
            button.onClick.AddListener(() =>
            {
                _uiManager.OpenUIFormAsync<LevelsForm>(stage.Levels).Forget();
                _uiManager.CloseUIForm<StagesForm>();
            });
        }
    }
}