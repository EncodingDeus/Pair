using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using TMPro;
using Cysharp.Threading.Tasks.CompilerServices;
using NUnit.Framework.Internal;
using static UnityEngine.GraphicsBuffer;

public class CardController : MonoBehaviour
{
    [Header("First show cards")]
    [SerializeField] private float firstShowDelay = 1.0f;
    [SerializeField] private float firstShowColumnDelay = 0.1f;

    [SerializeField] private Card cardPrefab;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private float closeCardDelay = 0.5f;


    private int _rows = 4;
    private int _columns = 6;
    private int _identicalCardsCount = 2;

    [SerializeField] private float rowDistance = 0.98f;
    [SerializeField] private float columnDistance = 1.2f;


    public bool CanInteract { get; private set; } = false;

    public event Action GameStarted;
    public event Action GameFinished;
    public event Action<Card> CardOpened;
    public event Action<IEnumerable<Card>> CardsChecked;


    private Card[,] _cards;
    private Dictionary<(int, int), Vector2> _positionMatrix;
    private float _matrixWidth;
    private float _matrixHeight;
    private List<Card> _remainingCards;
    private List<Card> _selectedCards;


    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    public void Init(int rows = 4, int columns = 5, int identicalCardsCount = 2)
    {
        ClearCards();


        List<Sprite> possibleSpriteList = new List<Sprite>(sprites);
        List<Sprite> totalSpritesOnBoard = new List<Sprite>();
        int cardGroupCount = rows * columns / identicalCardsCount;

        _rows = rows;
        _columns = columns;
        _identicalCardsCount = identicalCardsCount;
        _cards = new Card[rows, columns];
        _selectedCards = new List<Card>();
        _positionMatrix = new Dictionary<(int, int), Vector2>();
        _matrixWidth = rows * rowDistance;
        _matrixHeight = columns * columnDistance;
        _remainingCards = new List<Card>();
        CanInteract = false;

        for (int i = 0; i < cardGroupCount; i++)
        {
            var randomCardIndex = UnityEngine.Random.Range(0, possibleSpriteList.Count);
            for (int j = 0; j < identicalCardsCount; j++)
            {
                totalSpritesOnBoard.Add(possibleSpriteList[randomCardIndex]);
            }
            possibleSpriteList.RemoveAt(randomCardIndex);
        }

        for (int i = 0; i < rows; i++)
        {
            var posX = (rowDistance / 2 + rowDistance * i) - _matrixWidth / 2;
            for (int j = 0; j < columns; j++)
            {
                var posY = (columnDistance / 2 + columnDistance * j) - _matrixHeight / 2;
                var card = Instantiate(cardPrefab, transform);
                var cardPos = new Vector2(posX, posY);
                var randomCardIndex = UnityEngine.Random.Range(0, totalSpritesOnBoard.Count);

                _positionMatrix.Add((i, j), cardPos);

                card.Init(totalSpritesOnBoard[randomCardIndex], cardPos);
                card.OnCardClicked += OnCardClicked;

                totalSpritesOnBoard.RemoveAt(randomCardIndex);
                _cards[i,j] = card;
                _remainingCards.Add(card);
            }
        }

        ShowCardsFirstTime().Forget();
    }

    private async UniTaskVoid ShowCardsFirstTime()
    {
        await UniTask.Delay((int)(firstShowDelay * 1000f));

        await OpenCards();

        await UniTask.Delay((int)(firstShowDelay * 1000f));

        await CloseCards();

        await ShuffleCards();

        CanInteract = true;
        GameStarted?.Invoke();

    }

    private async UniTask OpenCards()
    {
        for (int c = 0; c < _columns; c++)
        {
            for (int r = 0; r < _rows; r++)
            {
                _cards[r, c].Open();
            }
            await UniTask.Delay((int)(firstShowColumnDelay * 1000f));
        }
    }

    private async UniTask CloseCards()
    {
        for (int c = 0; c < _columns; c++)
        {
            for (int r = 0; r < _rows; r++)
            {
                _cards[r, c].Close();
            }
            await UniTask.Delay((int)(firstShowColumnDelay * 1000f));
        }
    }

    private async UniTask ShuffleCards()
    {
        var keys = _positionMatrix.Keys.ToList();

        float moveCardSpeed = 8;

        int firstRandomKeyIndex = UnityEngine.Random.Range(0, keys.Count);
        var firstKey = keys[firstRandomKeyIndex];
        int randomKeyIndex = firstRandomKeyIndex;
        var randomKey = keys[randomKeyIndex];

        while (keys.Count > 1)
        {
            var movedCard = _cards[randomKey.Item1, randomKey.Item2];
            keys.RemoveAt(randomKeyIndex);

            randomKeyIndex = UnityEngine.Random.Range(0, keys.Count);
            randomKey = keys[randomKeyIndex];

            MoveCard(movedCard, randomKey, moveCardSpeed).Forget();

            Vector2 startPos = movedCard.transform.position;
            Vector2 targetPos = _positionMatrix[randomKey];
            float distance = (targetPos - startPos).magnitude;
            var moveCardDuration = distance / moveCardSpeed;

            await UniTask.Delay((int)(moveCardDuration / 16 * 1000f));
        }

        await MoveCard(_cards[randomKey.Item1, randomKey.Item2],
            firstKey, moveCardSpeed);
    }

    private async UniTask MoveCard(Card card, (int, int) target, float speed)
    {
        float timer = 0f;
        Vector2 startPos = card.transform.position;
        Vector2 targetPos = _positionMatrix[target];
        float distance = (targetPos - startPos).magnitude;
        float duration = distance / speed;

        while (timer < duration)
        {
            await UniTask.Yield();

            timer += Time.deltaTime;

            card.transform.position = Vector3.Lerp(startPos, targetPos, timer / duration);
        }
    }

    private void OnCardClicked(Card card)
    {
        if (!CanInteract || _selectedCards.Contains(card)) return;

        _selectedCards.Add(card);
        card.Open();
        CardOpened?.Invoke(card);

        if (_selectedCards.Count >= _identicalCardsCount)
        {
            CheckCards(_selectedCards, _selectedCards[0]);
        }
    }

    private void CheckCards(IEnumerable<Card> cards, Card matchCard)
    {
        var isIdentical = cards.All(card => card.HashCode == matchCard.HashCode);

        CanInteract = false;
        CardsChecked?.Invoke(cards);

        if (isIdentical)
        {
            CompleteCards(cards);
        }
        else
        {
            CloseCards(cards, closeCardDelay).Forget();
        }
    }

    private async UniTaskVoid CloseCards(IEnumerable<Card> cards, float delay)
    {
        await UniTask.Delay((int)(delay * 1000f));

        foreach (var card in cards)
        {
            card.Close();
        }

        _selectedCards.Clear();
        CanInteract = true;
    }

    private async UniTaskVoid OpenCards(IEnumerable<Card> cards, float delay)
    {
        await UniTask.Delay((int)(delay * 1000f));

        foreach (var card in cards)
        {
            card.Open();
        }
    }


    private void CompleteCards(IEnumerable<Card> cards)
    {
        foreach (var card in cards)
        {
            card.Complete();
            card.OnCardClicked -= OnCardClicked;

            _remainingCards.Remove(card);
        }

        _selectedCards.Clear();

        if (_remainingCards.Count > 0)
        {
            CanInteract = true;
        }
        else
        {
            OnGameFinished();
        }
    }

    private void OnGameFinished()
    {
        GameFinished?.Invoke();
    }

    //private void OnValidate()
    //{
    //    if (Application.isPlaying)
    //    {
    //        Init();
    //    }
    //}

    private void SetPosition(Card card, int row, int column)
    {
        card.transform.position = _positionMatrix[(row, column)];
    }

    private void ClearCards()
    {
        if (_cards != null)
        {
            int rows = _cards.GetLength(0);
            int columns = _cards.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    Destroy(_cards[i, j].gameObject);
                }
            }

            _cards = null;
        }
    }
}
