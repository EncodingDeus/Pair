using Cysharp.Threading.Tasks;
using UI.Form;
using UnityEngine;
using Zenject;

namespace Dobrozaur.UI.Form
{
    public class UIFormFactory : IFactory<string, Transform, UniTask<UIForm>>
    {
        private readonly DiContainer _container;
        private readonly ResourceManager _resourceManager;

        public UIFormFactory(DiContainer container, ResourceManager resourceManager)
        {
            _container = container;
            _resourceManager = resourceManager;
        }

        public async UniTask<UIForm> Create(string path, Transform parent)
        {
            var resource = await _resourceManager.LoadAssetAsync<GameObject>(path);
            return _container.InstantiatePrefab(resource, parent).GetComponent<UIForm>();
        }
    }
}