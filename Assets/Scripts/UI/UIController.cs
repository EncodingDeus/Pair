// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UI.Form;
// using UnityEngine;
//
// public class UIController : MonoBehaviour
// {
//     [SerializeField] private UIForm GameForm;
//     [SerializeField] private UIForm StagesForm;
//     [SerializeField] private UIForm StageLevelsForm;
//     // [SerializeField] private UIForm[] uiForms;
//
//     private HashSet<UIForm> availableUIForms = new HashSet<UIForm>();
//     private HashSet<UIForm> openedUIForms = new HashSet<UIForm>();
//
//     private UIForm _currentForm;
//     
//     public static UIController Instance { get; private set; }
//     
//     // public T OpenForm<T>() where T : UIForm
//     // {
//     //
//     //
//     // }
//
//     public void OpenStagesForm(object userData)
//     {
//         OpenForm(StagesForm, userData);
//     }
//     
//     public void OpenLevelsForm(object userData)
//     {
//         OpenForm(StageLevelsForm, userData);
//     }
//     
//     public void OpenGameForm(object userData)
//     {
//         OpenForm(GameForm, userData);
//     }
//
//
//     private void OpenForm(UIForm form, object userData)
//     {
//         if (_currentForm != null)
//         {
//             _currentForm.OnClose();
//             _currentForm.gameObject.SetActive(false);
//         }
//
//         _currentForm = form;
//         _currentForm.gameObject.SetActive(true);
//         _currentForm.OnOpen(userData);
//     }
//     
//     private void Awake()
//     {
//         if (Instance != null)
//         {
//             Debug.LogError($"{nameof(UIController)} already exist");
//             return;
//         }
//
//         Instance = this;
//         // availableUIForms = new HashSet<UIForm>(uiForms);
//     }
//
//     private void Start()
//     {
//         OpenStagesForm(null);
//     }
// }
