// using System;
// using System.Collections;
// using System.Collections.Generic;
// using TMPro;
// using UnityEngine;
// using UnityEngine.UI;
//
// public class TempUI : MonoBehaviour
// {
//     [SerializeField] private CardController cardController;
//
//     [SerializeField] private GameObject menuPanel;
//     [SerializeField] private Button menuButton;
//     [SerializeField] private Button closeMenuButton;
//     [SerializeField] private Button applyButton;
//     [SerializeField] private TMP_InputField rowsText;
//     [SerializeField] private TMP_InputField columnsText;
//     [SerializeField] private TMP_InputField identicalCardsCountText;
//     [SerializeField] private TMP_Text fovText;
//     [SerializeField] private Slider fovSlider;
//
//     [SerializeField] private Camera camera;
//     
//
//     private void Awake()
//     {
//         menuButton.onClick.AddListener(() =>
//         {
//             menuPanel.SetActive(!menuPanel.activeSelf);
//         });
//
//         closeMenuButton.onClick.AddListener(() =>
//         {
//             menuPanel.SetActive(false);
//         });
//
//         applyButton.onClick.AddListener(() =>
//         {
//             if (cardController != null)
//             {
//                 int rows = System.Convert.ToInt32(rowsText.text);
//                 int columns = System.Convert.ToInt32(columnsText.text);
//                 int identicalCardsCount = System.Convert.ToInt32(identicalCardsCountText.text);
//
//
//                 cardController.Init(rows, columns, identicalCardsCount);
//             }
//             menuPanel.SetActive(false);
//
//         });
//
//         fovSlider.onValueChanged.AddListener(OnFovSliderChanged);
//
//         Time.timeScale = 1;
//     }
//
//     private void OnFovSliderChanged(float value)
//     {
//         fovText.text = value.ToString();
//         camera.orthographicSize = value;
//     }
//
//     private void Update()
//     {
//         
//     }
// }
