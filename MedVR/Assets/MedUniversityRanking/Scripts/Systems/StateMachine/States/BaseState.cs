using System.Collections;
using UnityEngine;

namespace Nato.StateMachine
{
    public interface IState<T>
    {
        void OnStart(T manager);
        void OnTick();
        void OnFixedTick();
        void OnEnd();
        void OnDestroyTick();
        bool IsInitialized();
    }

    public abstract class BaseState<T> : IState<T>
    {
        public T Manager;
        public bool IsStarted;

        private float startTime;
        private float delayTime = 0.1f;

        public virtual void OnStart(T manager)
        {
            Manager = manager;
            StartStateDelay();
        }

        public virtual void OnTick()
        {
            if (!IsStarted)
            {
                if (Time.unscaledTime >= startTime + delayTime)
                {
                    IsStarted = true;
                }
            }
        }

        public virtual void OnFixedTick()
        {
        }

        public virtual void OnEnd()
        {
            IsStarted = false;
        }

        public virtual void OnDestroyTick()
        {
        }

        private void StartStateDelay()
        {
            startTime = Time.unscaledTime;
            IsStarted = false;
        }

        public bool IsInitialized()
        {
            return IsStarted;
        }
    }
}