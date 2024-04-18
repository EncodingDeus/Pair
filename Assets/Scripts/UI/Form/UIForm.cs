using System;
using System.Collections;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Dobrozaur.Manager;
using Dobrozaur.Utility;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Form
{
    public class UIForm : MonoBehaviour, IUIForm
    {
        private readonly string _openStateName = "Open";
        private readonly string _closeStateName = "Close";
        
        private readonly int _openParam = Animator.StringToHash("Open");
        private readonly int _closeParam = Animator.StringToHash("Close");

        protected UIManager UIManager;
        
        private Animator _animator;
        private IMemoryPool _pool;
        private GraphicRaycaster _clickable;
        private Canvas _canvas;


        public int Order => _canvas.sortingOrder;
        public bool IsActive { get; private set; }
        public virtual bool AnimationStart(string stateName) => 
            _animator == null || 
            _animator.isInitialized && 
            _animator.IsSetState(stateName);
        public virtual bool AnimationComplete() => _animator == null || _animator.isInitialized && !_animator.IsPlaying();
        

        [Inject]
        public virtual void Construct(UIManager uiManager)
        {
            UIManager = uiManager;
        }

        public void SetLayer(int orderInLayer)
        {
            _canvas.sortingLayerName = "UI";
            _canvas.sortingOrder = orderInLayer;
        }

        public virtual void OnInit(object userData)
        {
            _animator = GetComponent<Animator>();
            _clickable = GetComponent<GraphicRaycaster>();
            _canvas = GetComponent<Canvas>();
        }

        public virtual void OnOpen(object userData)
        {
            IsActive = true;
            
            gameObject.SetActive(true);
         
            _animator?.ResetTrigger(_closeParam);
            _animator?.SetTrigger(_openParam);

            StartCoroutine(CheckResume(_openStateName));
        }

        public virtual void OnClose(object userData)
        {
            IsActive = false;

            _animator?.ResetTrigger(_openParam);
            _animator?.SetTrigger(_closeParam);
            
            StartCoroutine(CheckPause(_closeStateName));
        }

        public virtual void Close(object userData = null)
        {

            // OnClose(userData);
            UIManager.CloseUIForm(this, userData);
        }
        
        private IEnumerator CheckResume(string stateName)
        {
            if (_animator != null)
            {
                yield return new WaitUntil(() => AnimationStart(stateName));
                yield return new WaitUntil(() => AnimationComplete());
            }

            _clickable.enabled = true;
        }
        
        
        private IEnumerator CheckPause(string stateName)
        {
            _clickable.enabled = false;

            if (_animator != null)
            {
                yield return new WaitUntil(() => AnimationStart(stateName));
                yield return new WaitUntil(() => AnimationComplete());
            }
            
            gameObject.SetActive(false);

            // UIManager.CloseUIForm(this);
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