using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using TMPro;
using Cysharp.Threading.Tasks.CompilerServices;
using Dobrozaur.Gameplay;
using NUnit.Framework.Internal;
using static UnityEngine.GraphicsBuffer;
using Unity.Collections.LowLevel.Unsafe;

public class CardController : MonoBehaviour, IDisposable
{
    [Header("First show cards")]
    [SerializeField] private float firstShowDelay = 1.0f;
    [SerializeField] private float firstShowRowDelay = 0.1f;

    [SerializeField] private Card cardPrefab;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private float closeCardDelay = 0.5f;
    
    [SerializeField] private float rowDistance = 0.98f;
    [SerializeField] private float columnDistance = 1.2f;
    
    private int _rows = 4;
    private int _columns = 6;
    private int _identicalCardsCount = 2;

    private CancellationTokenSource _cancellationTokenSource;
    private List<Card>[,] _cardsInGrid;
    private Dictionary<Card, Vector2Int> _cardsCoordinate;
    private Dictionary<Vector2Int, Vector2> _positionMatrix;
    private int _cardsCount;
    private int _cardGroupCount;
    private float _matrixWidth;
    private float _matrixHeight;
    private List<Card> _remainingCards;
    private List<Card> _selectedCards;
    private Queue<Sprite> _spritesQueue;
    private bool _canInteract;
    private bool _isPause;
    
    public int Attempts { get; private set; }
    public int Misses { get; private set; }
    public bool CanInteract => _canInteract && !_isPause;
    public event Action GameStarted;
    public event Action GameFinished;
    public event Action<Card> CardOpened;
    public event Action<IEnumerable<Card>> CardsChecked;

    
    public void Init(Level.LevelSetting setting)
    {
        ClearCards();
        
        _rows = setting.Rows;
        _columns = setting.Columns;
        _identicalCardsCount = setting.IdenticalCards;
        
        _cardsCount = _rows * _columns;
        _cardGroupCount = _cardsCount / _identicalCardsCount;
        _matrixWidth = _columns * columnDistance;
        _matrixHeight = _rows * rowDistance;

        _cancellationTokenSource = new CancellationTokenSource();
        _cardsInGrid = new List<Card>[_columns, _rows];
        _cardsCoordinate = new Dictionary<Card, Vector2Int>(_cardsCount);
        _selectedCards = new List<Card>();
        _positionMatrix = new Dictionary<Vector2Int, Vector2>();
        _remainingCards = new List<Card>();
        _spritesQueue = GetQueueSprites(_cardsCount, _identicalCardsCount);

        Attempts = 0;
        Misses = 0;
        
        var totalSpritesOnBoard = GetQueueSprites(_cardsCount, _identicalCardsCount);

        for (int y = 0; y < _rows; y++)
        {
            for (int x = 0; x < _columns; x++)
            {
                var sprite = totalSpritesOnBoard.Dequeue();
                var cardPos = GetPosition(x, y);
                var coordinate = new Vector2Int(x, y);
                var card = CreateCard(sprite, cardPos);

                _cardsInGrid[x, y] = new List<Card>(new Card[] { card });
                _cardsCoordinate.Add(card, coordinate);

                _positionMatrix.Add(coordinate, cardPos);
            }
        }

        _isPause = false;
        _canInteract = false;

        ShowCardsFirstTime(_cancellationTokenSource.Token).Forget();
    }

    private Queue<Sprite> GetQueueSprites(int cardsCount, int identicalCards)
    {
        var cardGroups = cardsCount / identicalCards;
        var spriteList = new List<Sprite>(sprites);
        var totalSprites = new List<Sprite>(cardsCount);
        var result = new Queue<Sprite>(cardsCount);

        for (int i = 0; i < cardGroups; i++)
        {
            var randomCardIndex = UnityEngine.Random.Range(0, spriteList.Count);
            for (int j = 0; j < identicalCards; j++)
            {
                totalSprites.Add(spriteList[randomCardIndex]);
            }
            spriteList.RemoveAt(randomCardIndex);
        }

        // randomize
        for (int i = 0; i < cardsCount; i++)
        {
            var random = UnityEngine.Random.Range(0, totalSprites.Count);
            result.Enqueue(totalSprites[random]);
            totalSprites.RemoveAt(random);
        }

        return result;
    }

    private Card CreateCard(Sprite sprite, Vector2 position)
    {
        var card = Instantiate(cardPrefab, transform);

        card.Init(sprite, position);
        card.OnCardClicked += OnCardClicked;

        _remainingCards.Add(card);

        return card;
    }


    private Vector2 GetPosition(int x, int y)
    {
        var posX = (columnDistance / 2 + columnDistance * x) - _matrixWidth / 2;
        var posY = (rowDistance / 2 + rowDistance * y) - _matrixHeight / 2;

        return new Vector2(posX, posY);
    }

    private async UniTaskVoid ShowCardsFirstTime(CancellationToken cancellationToken)
    {
        await UniTask.Delay(
            millisecondsDelay:(int)1000f, 
            cancellationToken: cancellationToken);

        await OpenCards(cancellationToken);

        await UniTask.Delay(
            millisecondsDelay:(int)(firstShowDelay * 1000f),
            cancellationToken: cancellationToken);

        await CloseCards(cancellationToken);

        //await ShuffleCards();

        _canInteract = true;
        GameStarted?.Invoke();
    }

    private async UniTask OpenCards(CancellationToken cancellationToken)
    {
        for (int y = 0; y < _rows; y++)
        {
            for (int x = 0; x < _columns; x++)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;
                
                var card = GetCard(x, y);
                card?.Open();
            }
            
            await UniTask.Delay(
                millisecondsDelay:(int)(firstShowRowDelay * 1000f),
                cancellationToken: cancellationToken);
        }
    }

    private async UniTask CloseCards(CancellationToken cancellationToken)
    {
        for (int y = 0; y < _rows; y++)
        {
            for (int x = 0; x < _columns; x++)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var card = GetCard(x, y);
                card?.Close();
            }
            
            await UniTask.Delay(
                millisecondsDelay:(int)(firstShowRowDelay * 1000f),
                cancellationToken: cancellationToken);
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
        var nextCard = _cardsInGrid[randomKey.x, randomKey.y];

        while (keys.Count > 1)
        {
            var movedCardIndex = randomKey;
            keys.RemoveAt(randomKeyIndex);

            randomKeyIndex = UnityEngine.Random.Range(0, keys.Count);
            randomKey = keys[randomKeyIndex];

            MoveCard(movedCardIndex, randomKey, moveCardSpeed).Forget();

            //Vector2 startPos = movedCard.transform.position;
            //Vector2 targetPos = _positionMatrix[randomKey];
            //float distance = (targetPos - startPos).magnitude;
            //var moveCardDuration = distance / moveCardSpeed;

            //await UniTask.Delay((int)(moveCardDuration / 16 * 1000f));
        }

        await MoveCard(randomKey, firstKey, moveCardSpeed);
    }

    private UniTask MoveCard(Vector2Int cardIndex, Vector2Int target, float speed)
    {
        Card card = GetCard(cardIndex.x, cardIndex.y);
        var task = MoveCard(card, target, speed);

        _cardsInGrid[cardIndex.x, cardIndex.y].Remove(card);

        return task;
    }

    private async UniTask MoveCard(Card card, Vector2Int target, float speed)
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

        _cardsCoordinate[card] = target;
        _cardsInGrid[target.x, target.y].Add(card);
    }

    private void OnCardClicked(Card card)
    {
        if (!CanInteract || _selectedCards.Contains(card)) 
            return;

        _selectedCards.Add(card);
        card.Open();
        CardOpened?.Invoke(card);

        if (_selectedCards.Count >= 2)
        {
            CheckCards(_selectedCards.ToArray(), _selectedCards[0]);
        }
    }

    private void CheckCards(Card[] cards, Card matchCard)
    {
        var isIdentical = cards.All(card => card.HashCode == matchCard.HashCode);

        _canInteract = false;

        if (isIdentical)
        {
            if (cards.Count() >= _identicalCardsCount)
            {
                Attempts++;
                CompleteCards(cards).Forget();
            }
            else
            {
                _canInteract = true;
            }
            _canInteract = true;

        }
        else
        {
            Attempts++;
            Misses++;
            CloseCards(cards, closeCardDelay).Forget();
        }
        
        CardsChecked?.Invoke(cards);     
    }

    private async UniTaskVoid CloseCards(IEnumerable<Card> cards, float delay)
    {
        await UniTask.Delay((int)(delay * 1000f));

        foreach (var card in cards)
        {
            card.Close();
        }

        _selectedCards.Clear();
        _canInteract = true;
    }

    private async UniTaskVoid OpenCards(IEnumerable<Card> cards, float delay)
    {
        await UniTask.Delay((int)(delay * 1000f));

        foreach (var card in cards)
        {
            card.Open();
        }
    }

    public void OpenCards(float duration = 2)
    {
        OpenCardAsync(duration, _cancellationTokenSource.Token).Forget();
    }

    private async UniTaskVoid OpenCardAsync(float duration, CancellationToken cancellationToken)
    {
        await OpenCards(cancellationToken);

        await UniTask.Delay(
            millisecondsDelay:(int)(duration * 1000f),
            cancellationToken: cancellationToken);

        await CloseCards(cancellationToken);
    }

    //private void CompleteCards(IEnumerable<(int, int)> cardsIndex)
    //{
    //    foreach (var cardIndex in cardsIndex)
    //    {
    //        var cardList = _cards[cardIndex.Item1, cardIndex.Item2];
    //        var last = _card
    //        card.Complete();
    //        card.OnCardClicked -= OnCardClicked;

    //        _remainingCards.Remove(card);

    //        Destroy(card.gameObject);
    //    }

    //    _selectedCards.Clear();

    //    if (_remainingCards.Count > 0)
    //    {
    //        CanInteract = true;
    //    }
    //    else
    //    {
    //        OnGameFinished();
    //    }

    //}

    //private void CompleteCard(Card card)
    //{

    //}


    private async UniTaskVoid CompleteCards(Card[] cards)
    {
        List<Vector2Int> completedCards = new List<Vector2Int>();
        HashSet<int> columnCompleted = new HashSet<int>();

        var lastCard = cards.LastOrDefault();

        foreach (var card in cards)
        {
            var coord = _cardsCoordinate[card];

            card.OnCardClicked -= OnCardClicked;

            _remainingCards.Remove(card);
            _cardsInGrid[coord.x, coord.y].Remove(card);

            columnCompleted.Add(coord.x);
        }

        CheckGameComplete();


        await UniTask.WaitUntil(() => lastCard.IsOpened());

        foreach (var card in cards)
        {
            card.Complete();
        }

        //await UniTask.Delay((int)(closeCardDelay * 1000f));

        foreach (var card in cards)
        {
            await UniTask.WaitUntil(() => lastCard.IsComplete());
            
            // Destroy(card.gameObject);
        }

        // Shift cards and create new on the top
        // ShiftCardsByColumn(columnCompleted);

    }

    private void ShiftCardsByColumn(IEnumerable<int> columns)
    {
        foreach (var collumn in columns)
        {
            var offset = ShiftCardsDown(collumn);

            for (int i = 0; i < offset; i++)
            {
                var sprite = _spritesQueue.Dequeue();
                var pos = GetPosition(collumn, _rows + i);
                var card = CreateCard(sprite, pos);

                MoveCard(card, new Vector2Int(collumn, _rows - offset + i), 6).Forget();
                
                _remainingCards.Add(card);

                if (_spritesQueue.Count == 0)
                {
                    _spritesQueue = GetQueueSprites(_cardsCount, _identicalCardsCount);
                }
            }
        }
    }

    private void CheckGameComplete()
    {
        _selectedCards.Clear();

        if (_remainingCards.Count > 0)
        {
            _canInteract = true;
        }
        else
        {
            OnGameFinished();
        }
    }

    private int ShiftCardsDown(int collumn)
    {
        Card card = null;
        int offset = 0;

        for (int rowIndex = 0; rowIndex < _rows; rowIndex++)
        {
            card = GetCard(collumn, rowIndex);

            if (card == null)
            {
                offset++;
                continue;
            }
            if (offset < 0) 
                continue;

            MoveCard(new Vector2Int(collumn, rowIndex), new Vector2Int(collumn, rowIndex - offset), 6).Forget();
        }

        return offset;
    }

    private Card GetCard(int indexX, int indexY)
    {
        if (indexX < 0 || indexY < 0 || indexX >= _columns || indexY >= _rows)
            return null;

        var list = _cardsInGrid[indexX, indexY];
        return list.Count - 1 >= 0 ? list[list.Count - 1] : null;
    }

    private void OnGameFinished()
    {
        Debug.Log("OnGameFinished");
        GameFinished?.Invoke();
    }

    //private void OnValidate()
    //{
    //    if (Application.isPlaying)
    //    {
    //        Init();
    //    }
    //}

    private void SetPosition(Card card, int x, int y)
    {
        card.transform.position = _positionMatrix[new Vector2Int(x, y)];
    }

    public void SetPause(bool pause)
    {
        _isPause = pause;
    }

    public void ClearCards()
    {
        if (_cardsInGrid != null)
        {
            for (int y = 0; y < _rows; y++)
            {
                for (int x = 0; x < _columns; x++)
                {
                    for (int c = 0; c < _cardsInGrid[x, y].Count; c++)
                    {
                        Destroy(_cardsInGrid[x, y][c].gameObject);
                    }
                }
            }

            _cardsInGrid = null;
        }
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource?.Dispose();
    }
}
