using System;
using System.Collections;
using System.Collections.Generic;
using Dobrozaur.Difinition;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dobrozaur
{
    public class GameProgress : MonoBehaviour
    {
        [SerializeField] private TMP_Text progressText;
        [SerializeField] private TMP_Text starText;
        [SerializeField] private Image starImage;
        [SerializeField] private Sprite bronzeStarSprite;
        [SerializeField] private Sprite silverStarSprite;
        [SerializeField] private Sprite goldStarSprite;
        [SerializeField] private Sprite perfectStarSprite;
        
        private int _maxMisses;
        private int _bronzeStarMisses;
        private int _silverStarMisses;
        private int _goldStarMisses;
        
        public void Init(int maxMisses, int oneStarMisses, int twoStarMisses, int threeStarMisses)
        {
            _maxMisses = maxMisses;
            _bronzeStarMisses = oneStarMisses;
            _silverStarMisses = twoStarMisses;
            _goldStarMisses = threeStarMisses;
            
            SetMisses(0);
        }
        
        public void SetMisses(int misses)
        {
            progressText.text = $"{misses}/{_maxMisses}";
            starImage.enabled = true;
            
            if (misses > _maxMisses)
            {
                // lose
                starText.text = "Проигрыш :(";
                starImage.enabled = false;
            }
            else if (misses > _silverStarMisses)
            {
                // bronze
                starImage.sprite = bronzeStarSprite;
                starText.text = Constant.Level.BronzeStar;
            }
            else if (misses > _goldStarMisses)
            {
                // silver
                starImage.sprite = silverStarSprite;
                starText.text = Constant.Level.SilverStar;
            }
            else if (misses > 0)
            {
                // gold
                starImage.sprite = goldStarSprite;
                starText.text = Constant.Level.GoldStar;
            }
            else
            {
                // perfect
                starImage.sprite = perfectStarSprite;
                starText.text = Constant.Level.PerfectStar;
            }
        }
    }
}
