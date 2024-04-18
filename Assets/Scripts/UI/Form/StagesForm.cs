using Cysharp.Threading.Tasks;
using Dobrozaur.Gameplay;
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

        private StageCard[] _stagesCard;
        private PairData _pairData;


        public override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            _pairData ??= (PairData)userData;

            InitStages(_pairData).Forget();
        }

        private async UniTaskVoid InitStages(PairData pairData)
        {
            ClearStages();

            var stars = pairData.Stages.GetStars();
            _pairData = pairData;
            _stagesCard = new StageCard[pairData.Stages.Length];

            for (int i = 0; i < pairData.Stages.Length; i++)
            {
                var stage = pairData.Stages[i];
                var stageCard = await stagePrefab.InstantiateAsync(stagesRoot);

                stageCard.Init(stage, UIManager, stars);
                _stagesCard[i] = stageCard;
            }
        }

        private void ClearStages()
        {
            if (_stagesCard == null) return;

            for (int i = 0; i < _stagesCard.Length; i++)
            {
                Destroy(_stagesCard[i].gameObject);
            }

            _stagesCard = null;
        }
    }
}