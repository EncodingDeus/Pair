using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dobrozaur.Core
{
    public class FSM
    {
        private readonly Dictionary<Type, IState> _states;
        private IState _currentState;

        public FSM(IState[] states)
        {
            _states = new Dictionary<Type, IState>();
            for (int i = 0; i < states.Length; i++)
            {
                _states.Add(states[i].GetType(), states[i]);
            }
        }

        public void SetEntryState<TState>() where TState : IState
        {
            _currentState = _states[typeof(TState)];
        }
        
        public void Start()
        {
            _currentState.OnEnter(this);
        }

        public void ChangeState<TState>() where TState : IState
        {
            Debug.Log($"[FSM]({this}) Change state to: {_states[typeof(TState)]}");
            
            _currentState.OnExit(this);
            _currentState = _states[typeof(TState)];
            _currentState.OnEnter(this);
        }
    }
}