using System;
using System.Collections;
using UnityEngine;
namespace Checkers
{
    public class CoroutineHelper : Singleton<CoroutineHelper>
    {
        public void StartIenumerator(IEnumerator coroutine)
        {
            StartCoroutine(coroutine);
        }

        public void DelayedAction(Action action, int frames)
        {
            StartCoroutine(LaunchDelayedAction(action, frames));
        }

        protected IEnumerator LaunchDelayedAction(Action action, float delay)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }

        protected IEnumerator LaunchDelayedAction(Action action, int frames)
        {
            for (int i = 0; i < frames; i++)
                yield return new WaitForEndOfFrame();
            action?.Invoke();
        }
    }
}