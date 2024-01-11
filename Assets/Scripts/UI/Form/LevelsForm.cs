using System.Collections.Generic;
using Dobrozaur.Gameplay;
using Dobrozaur.Manager;
using Dobrozaur.UI.Form;
using UI.Form.Part;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Form
{
    public class LevelsForm : UIForm
    {
        private readonly List<LevelItem> _levelItems = new List<LevelItem>();
        
        [SerializeField] private Button backButton;
        [SerializeField] private LevelItem levelItemPrefab;
        [SerializeField] private Transform levelsRoot;
        
        
        public override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            
            var levels = (Level[])userData;
            
            ClearLevels();
            InitLevels(levels);
            
            backButton.onClick.AddListener(async () =>
            {
                await UIManager.OpenUIFormAsync<StagesForm>();
            });
        }

        public override void OnClose(object userData)
        {
            base.OnClose(userData);
            
            backButton.onClick.RemoveAllListeners();
        }

        private void InitLevels(Level[] levels)
        {
            for (int i = 0; i < levels.Length; i++)
            {
                var level = Instantiate(levelItemPrefab, levelsRoot);
                
                level.Init(levels[i], UIManager);
                _levelItems.Add(level);
            }
        }

        private (int, int, int) GetLevelSettings()
        {
            var rows = Random.Range(3, 8);
            var collumns = Random.Range(3, 6);

            var res = rows * collumns;
            int cards = 2;
            while (res % cards != 0)
            {
                cards++;
            }

            return (rows, collumns, cards);
        }

        private void ClearLevels()
        {
            for (int i = 0; i < _levelItems.Count; i++)
            {
                Destroy(_levelItems[i].gameObject);
            }
            _levelItems.Clear();
        }
    }
}