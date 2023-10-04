using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameForm : MonoBehaviour
{
    [SerializeField] private CardItem[] cardItems;
    [SerializeField] private Sprite[] sprites;
    
    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        var tempSprites = new List<Sprite>(sprites);
        var tempCards = new List<CardItem>(cardItems);
        var id = 0;
        
        while (tempCards.Count > 0)
        {
            int randomSprite = Random.Range(0, tempSprites.Count);
            var sprite = tempSprites[randomSprite];
            var cardPairIndex = Random.Range(1, tempCards.Count);
            
            tempSprites.RemoveAt(randomSprite);
            tempCards[0].Init(sprite, id);
            tempCards[cardPairIndex].Init(sprite, id);
            
            tempCards.RemoveAt(cardPairIndex);
            tempCards.RemoveAt(0);

            id++;
        }
    }
}
