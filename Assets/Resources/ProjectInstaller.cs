using Cysharp.Threading.Tasks;
using Dobrozaur.Manager;
using Dobrozaur.UI.Form;
using UI.Form;
using UnityEngine;
using Zenject;

namespace Pair
{
    public class ProjectInstaller : MonoInstaller
    {
        [SerializeField] private UIManager uiManager;
        [SerializeField] private CardController cardController;
        
        
        // ReSharper disable Unity.PerformanceAnalysis
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<ResourceManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<NetworkManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<PoolObjectManager>().AsSingle();
            
            Container.BindFactory<string, Transform, UniTask<UIForm>, UIForm.Factory>()
                .FromFactory<UIFormFactory>();
            Container.BindInterfacesAndSelfTo<UIManager>().FromInstance(uiManager).AsSingle();
            Container.BindInterfacesAndSelfTo<CardController>().FromInstance(cardController).AsSingle();
            
            Container.BindInterfacesAndSelfTo<GameStateManager>().AsSingle();
            
            // Container.BindFactory<UIForm, UIForm.Factory>();
            // Container.BindIFactory<string, Transform, UniTask<UIForm>, UIForm.Factory, IFactory<string, Transform, UniTask<UIForm>>>();
        }
    }
}