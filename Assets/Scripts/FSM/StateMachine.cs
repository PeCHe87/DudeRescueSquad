using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using Object = System.Object;

// From Jason's video: https://www.youtube.com/watch?v=V75hgcsCGOM

// Notes
// 1. What a finite state machine is
// 2. Examples where you'd use one
//     AI, Animation, Game State
// 3. Parts of a State Machine
//     States & Transitions
// 4. States - 3 Parts
//     Tick - Why it's not Update()
//     OnEnter / OnExit (setup & cleanup)
// 5. Transitions
//     Separated from states so they can be re-used
//     Easy transitions from any state

namespace DudeResqueSquad
{
    public class StateMachine : INotifyPropertyChanged
    {
        #region Public properties

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Private properties

        private IState _currentState;
        private IState _lastState;
        private bool _isEnable = true;

        private Dictionary<Type, List<Transition>> _transitions = new Dictionary<Type, List<Transition>>();
        private List<Transition> _currentTransitions = new List<Transition>();
        private List<Transition> _anyTransitions = new List<Transition>();

        private static List<Transition> EmptyTransitions = new List<Transition>(0);

        #endregion

        #region Public methods

        public void Tick()
        {
            if (!_isEnable)
                return;

            var transition = GetTransition();
            if (transition != null)
                SetState(transition.To);

            _currentState?.Tick();
        }

        public void SetState(IState state)
        {
            if (state == _currentState)
                return;

            _currentState?.OnExit();
            _currentState = state;

            _transitions.TryGetValue(_currentState.GetType(), out _currentTransitions);
            if (_currentTransitions == null)
                _currentTransitions = EmptyTransitions;

            _currentState.OnEnter();

            // Check if FSM reaches the last state and should be stopped
            if (_currentState == _lastState)
            {
                _isEnable = false;

                Debug.Log("<color=magenta>FSM</color> - <b>STOP</b>");
            }

            NotifyPropertyChanged();
        }

        public void SetLastState(IState state)
        {
            _lastState = state;
        }

        public void AddTransition(IState from, IState to, Func<bool> predicate)
        {
            if (_transitions.TryGetValue(from.GetType(), out var transitions) == false)
            {
                transitions = new List<Transition>();
                _transitions[from.GetType()] = transitions;
            }

            transitions.Add(new Transition(to, predicate));
        }

        public void AddAnyTransition(IState state, Func<bool> predicate)
        {
            _anyTransitions.Add(new Transition(state, predicate));
        }

        public Enums.EnemyStates GetCurrentState()
        {
            if (_currentState == null)
                return Enums.EnemyStates.NONE;

            return _currentState.State();
        }

        #endregion

        #region Private methods

        private class Transition
        {
            public Func<bool> Condition { get; }
            public IState To { get; }

            public Transition(IState to, Func<bool> condition)
            {
                To = to;
                Condition = condition;
            }
        }

        private Transition GetTransition()
        {
            foreach (var transition in _anyTransitions)
                if (transition.Condition())
                    return transition;

            foreach (var transition in _currentTransitions)
                if (transition.Condition())
                    return transition;

            return null;
        }

        #endregion

        #region INotifyPropertyChanged implementation

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}