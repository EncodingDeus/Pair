using Dobrozaur.Manager;
using Dobrozaur.UI.Form.Part;
using UI.Form;
using UnityEngine;
using UnityEngine.UI;

namespace Dobrozaur.UI.Form
{
    public class StagesForm : UIForm
    {
        [SerializeField] private Button stageCard1;
        [SerializeField] private Button stageCard2;

        [SerializeField] private StageCard stagePrefab;
        [SerializeField] private Transform stagesRoot;

        private StageCard[] _stages;

        
        public override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            InitStages();

            // stageCard1.onClick.AddListener(() =>
            // {
            //     UIController.Instance.OpenStageLevelsForm((25, 25));
            // });
            // stageCard2.onClick.AddListener(() =>
            // {
            //     UIController.Instance.OpenStageLevelsForm((7, 25));
            // });
        }

        public void InitStages()
        {
            ClearStages();

            _stages = new StageCard[GameManager.Instance.Stages.Length];

            for (int i = 0; i < GameManager.Instance.Stages.Length; i++)
            {
                var stage = GameManager.Instance.Stages[i];
                var stageCard = Instantiate(stagePrefab, stagesRoot);

                stageCard.Init(stage, UIManager);
                _stages[i] = stageCard;
            }
        }

        private void ClearStages()
        {
            if (_stages == null) return;

            for (int i = 0; i < _stages.Length; i++)
            {
                Destroy(_stages[i].gameObject);
            }

            _stages = null;
        }

        public override void OnClose(object userData)
        {
            base.OnClose(userData);

            stageCard1.onClick.RemoveAllListeners();
            stageCard2.onClick.RemoveAllListeners();
        }
    }
}