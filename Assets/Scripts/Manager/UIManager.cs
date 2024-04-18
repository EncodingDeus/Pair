using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Dobrozaur.Core;
using UI.Form;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Dobrozaur.Manager
{
    public class UIManager : MonoBehaviour
    {
        private const string UIFormsPath = "Assets/UI/Form";

        [SerializeField] private int initialPoolCapacity = 8;
        [SerializeField] private float poolExpireTime = 300;
        
        private ResourceManager _resourceManager;
        private UIForm.Factory _uiFactory;
        private PoolObjectManager _poolObjectManager;
        
        private HashSet<UIForm> _openedUIForms;
        private Dictionary<UIForm, double> _uiFormsPool;
        private PoolGroup<object> _uiGroup;
        private Canvas _canvas;
        private int _formLayer;

        [Inject]
        private void Constructor(ResourceManager resourceManager, UIForm.Factory uiFactory, PoolObjectManager poolObjectManager)
        {
            _resourceManager = resourceManager;
            _uiFactory = uiFactory;
            _poolObjectManager = poolObjectManager;
            
            _openedUIForms = new HashSet<UIForm>(initialPoolCapacity);
            _uiFormsPool = new Dictionary<UIForm, double>(initialPoolCapacity);
            _canvas = GetComponentInChildren<Canvas>();
            _canvas.worldCamera = Camera.main;
            
            _uiGroup = _poolObjectManager.CreateGroup("UIForms", 64, 300);
        }

        private void Update()
        {
            
        }

        public async UniTask<T> OpenUIFormAsync<T>(object userData = null) where T : UIForm
        {
            var formName = typeof(T).Name;
            var path = UIFormsPath + $"/{formName}.prefab";
            UIForm uiForm = null;
            
            Debug.Log($"Open form: {path}");

            var formInPool =
                _uiFormsPool.FirstOrDefault(f => f.Key.GetType().Name == formName && f.Key.IsActive == false).Key;

            // _uiGroup.GetFromBack<T>();
            
            if (formInPool == null)
            {
                uiForm = await _uiFactory.Create(path, _canvas.transform);
            }
            else
            {
                uiForm = formInPool;
            }

            _uiFormsPool.TryAdd(uiForm, DateTime.Now.TimeOfDay.TotalMilliseconds);
            _openedUIForms.Add(uiForm);
            
            uiForm.OnInit(userData);
            uiForm.SetLayer(_formLayer);
            uiForm.OnOpen(userData);
            
            return uiForm.GetComponent<T>();
        }

        public void CloseUIForm<T>(object userData = null) where T : UIForm
        {
            var form = _openedUIForms.FirstOrDefault(form => form.GetType().Name == typeof(T).Name);

            if (form != null)
            {
                CloseUIForm(form, userData);
            }
            
            _uiGroup.AddObject(form);
        }
        
        public void CloseUIForm(UIForm form, object userData = null)
        {
            form.OnClose(userData);
            
            _openedUIForms.Remove(form);
        }
    }
}