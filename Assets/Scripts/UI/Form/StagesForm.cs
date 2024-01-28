using Cysharp.Threading.Tasks;
using Dobrozaur.Manager;
using Dobrozaur.UI.Form.Part;
using Dobrozaur.Utility;
using UI.Form;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Dobrozaur.UI.Form
{
    public class StagesForm : UIForm
    {
        [SerializeField] private AssetReference<StageCard> stagePrefab;
        [SerializeField] private Transform stagesRoot;
        [SerializeField] private AssetReferenceGameObject gg;

        private StageCard[] _stages;


        public override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            InitStages().Forget();
        }

        private async UniTaskVoid InitStages()
        {
            ClearStages();

            _stages = new StageCard[GameManager.Instance.Stages.Length];

            for (int i = 0; i < GameManager.Instance.Stages.Length; i++)
            {
                var stage = GameManager.Instance.Stages[i];
                var stageCard = await stagePrefab.InstantiateAsync(stagesRoot);

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
    }
}