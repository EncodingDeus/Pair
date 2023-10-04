using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Debugger : MonoBehaviour
{
    [SerializeField] private CardController cardController;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text triesText;

    private bool _isGamePlaying;
    private float _gameTimer; 
    private float _gameTries;
    private float _gameCardChecks;
    private float _gameStartTime;


    private void Awake()
    {
        cardController.GameStarted += OnGameStarted;
        cardController.GameFinished += OnGameFinished;
        cardController.CardOpened += OnCardOpened;
        cardController.CardsChecked += OnCardsChecked;
    }

    private void OnCardsChecked(IEnumerable<Card> obj)
    {
        _gameCardChecks++;
    }

    private void OnCardOpened(Card obj)
    {
        _gameTries++;
    }

    private void OnGameFinished()
    {
        _isGamePlaying = false;
    }

    private void OnGameStarted()
    {
        _gameTimer = 0; 
        _gameTries = 0;
        _gameCardChecks = 0;
        _gameStartTime = Time.time;

        _isGamePlaying = true;
    }

    private void Update()
    {
        if (_isGamePlaying)
        {
            _gameTimer = Time.time - _gameStartTime;
            triesText.text = $"{_gameTries} ({_gameCardChecks})";
            timerText.text = $"{_gameTimer.ToString("F1")} s";
        }
    }
}
