using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardItem : MonoBehaviour
{
    [SerializeField] private Image image;
    
    public int Id { get; private set; }
    

    public void Init(Sprite sprite, int id)
    {
        image.sprite = sprite;
        Id = id;
    }
}
