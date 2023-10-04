using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    private readonly int OpenParam = Animator.StringToHash("Open");
    private readonly int CloseParam = Animator.StringToHash("Close");
    private readonly int CompleteParam = Animator.StringToHash("Complete");

    [SerializeField] private SpriteRenderer spriteImage;

    private Animator _animator;
    private int _hashCode;

    public int HashCode => _hashCode;

    public event Action<Card> OnCardClicked;



    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    internal void Init(Sprite sprite, Vector2 position)
    {
        _hashCode = sprite.name.GetHashCode();
        spriteImage.sprite = sprite;
        transform.position = position;
    }

    private void OnMouseDown()
    {
        OnCardClicked?.Invoke(this);
    }

    public void Open()
    {
        _animator.SetTrigger(OpenParam);
    }

    public void Close()
    {
        _animator.SetTrigger(CloseParam);
    }

    public void Complete()
    {
        _animator.SetTrigger(CompleteParam);
    }
}