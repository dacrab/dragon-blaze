using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.State
{
    /// <summary>
    /// Generic finite state machine for managing entity states.
    /// </summary>
    /// <typeparam name="TStateId">Enum type for state identifiers.</typeparam>
    public class StateMachine<TStateId> where TStateId : Enum
    {
        private readonly Dictionary<TStateId, IState> _states = new();
        private IState _currentState;
        private TStateId _currentStateId;
        private bool _isInitialized;

        /// <summary>
        /// The current state identifier.
        /// </summary>
        public TStateId CurrentStateId => _currentStateId;

        /// <summary>
        /// Event fired when state changes. Parameters: (previousState, newState)
        /// </summary>
        public event Action<TStateId, TStateId> OnStateChanged;

        /// <summary>
        /// Registers a state with the state machine.
        /// </summary>
        public void RegisterState(TStateId stateId, IState state)
        {
            if (_states.ContainsKey(stateId))
            {
                Debug.LogWarning($"[StateMachine] State {stateId} already registered. Replacing.");
            }
            _states[stateId] = state;
        }

        /// <summary>
        /// Sets the initial state and calls Enter (use for setup).
        /// </summary>
        public void SetInitialState(TStateId stateId)
        {
            if (!_states.TryGetValue(stateId, out var state))
            {
                Debug.LogError($"[StateMachine] State {stateId} not registered.");
                return;
            }

            _currentStateId = stateId;
            _currentState = state;
            _isInitialized = true;
            _currentState.Enter();
        }

        /// <summary>
        /// Transitions to a new state.
        /// </summary>
        public void ChangeState(TStateId newStateId)
        {
            if (!_isInitialized)
            {
                Debug.LogError("[StateMachine] State machine not initialized. Call SetInitialState first.");
                return;
            }

            if (EqualityComparer<TStateId>.Default.Equals(_currentStateId, newStateId))
                return;

            if (!_states.TryGetValue(newStateId, out var newState))
            {
                Debug.LogError($"[StateMachine] State {newStateId} not registered.");
                return;
            }

            var previousStateId = _currentStateId;
            _currentState?.Exit();
            _currentStateId = newStateId;
            _currentState = newState;
            _currentState.Enter();

            OnStateChanged?.Invoke(previousStateId, newStateId);
        }

        /// <summary>
        /// Updates the current state. Call from MonoBehaviour.Update().
        /// </summary>
        public void Update()
        {
            _currentState?.Update();
        }

        /// <summary>
        /// Fixed updates the current state. Call from MonoBehaviour.FixedUpdate().
        /// </summary>
        public void FixedUpdate()
        {
            _currentState?.FixedUpdate();
        }

        /// <summary>
        /// Checks if currently in the specified state.
        /// </summary>
        public bool IsInState(TStateId stateId)
        {
            return EqualityComparer<TStateId>.Default.Equals(_currentStateId, stateId);
        }
    }
}
