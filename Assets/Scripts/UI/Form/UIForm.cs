using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Dobrozaur.Manager;
using UnityEngine;
using Zenject;

namespace UI.Form
{
    public class UIForm : MonoBehaviour, IUIForm
    {
        private readonly int openParam = Animator.StringToHash("Open");
        private readonly int closeParam = Animator.StringToHash("Close");

        protected UIManager UIManager;
        
        private Animator _animator;
        private IMemoryPool _pool;

        public bool IsActive { get; private set; }


        [Inject]
        public virtual void Construct(UIManager uiManager)
        {
            UIManager = uiManager;
        }

        public virtual void OnInit(object userData)
        {
        }

        public virtual void OnOpen(object userData)
        {
            IsActive = true;
            _animator?.SetTrigger(openParam);
            gameObject.SetActive(true);
        }

        public virtual void OnClose(object userData)
        {
            IsActive = false;
            _animator?.SetTrigger(closeParam);
            gameObject.SetActive(false);
        }

        public virtual void Close(object userData = null)
        {
            UIManager.CloseUIForm(this, userData);
        }

        // public class Factory : IFactory<string, Transform, UniTask<UIForm>>
        // {
        //     private readonly ResourceManager _resourceManager;
        //
        //     public Factory(ResourceManager resourceManager)
        //     {
        //         _resourceManager = resourceManager;
        //     }
        //     
        //     public async UniTask<UIForm> Create(string path, Transform parent = null)
        //     {
        //         var resource = await _resourceManager.LoadAssetAsync<GameObject>(path);
        //         return Instantiate(resource, parent).GetComponent<UIForm>();
        //     }
        // }
        
        
        public class Factory : PlaceholderFactory<string, Transform, UniTask<UIForm>>
        {
        }
    }
}