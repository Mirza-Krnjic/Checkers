using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Checkers
{
    public class CheckersContainerController : MonoBehaviour
    {
        [Header("Main options:")]
        [SerializeField]
        private float _showStepTime;
        [SerializeField]
        private float _hideStepTime;
        [SerializeField]
        private List<CheckerVisual> _checkers;

        [Space, Header("Test options:")]
        [SerializeField]
        private float _testShowStepTime = 1f;
        [SerializeField]
        private float _testHideStepTime = 1f;
        [SerializeField]
        private KeyCode _showKeycode = KeyCode.I;
        [SerializeField]
        private KeyCode _hideKeycode = KeyCode.O;

        private IEnumerator _showRoutine;
        private IEnumerator _hideRoutine;

        private bool _isContainerActive;
        private bool _isAnimationActive;

        #region Test

        private void Update()
        {
            if (Input.GetKeyDown(_showKeycode))
            {
                Show();
            }

            if (Input.GetKeyDown(_hideKeycode))
            {
                Hide();
            }
        }

        private void Show()
        {
            Show(_testShowStepTime);
        }

        private void Hide()
        {
            Hide(_testHideStepTime);
        }

        #endregion

        /// <summary>
        /// Show checkers container.
        /// </summary>
        public void Show(float time = 0f)
        {
            if (_isContainerActive || _isAnimationActive)
            {
                return;
            }

            if (_showRoutine != null)
            {
                StopCoroutine(_showRoutine);
            }

            _showRoutine = ShowRoutine(time);
            StartCoroutine(_showRoutine);
        }

        public IEnumerator ShowRoutine(float time = 0f)
        {
            if (_checkers.Count < 1)
            {
                yield break;
            }
            _isAnimationActive = true;

            var checkers = _checkers.Select(ch => GameController.Instance.CoreInstance.GetChecker(ch.Id)).ToList();
            var activeCheckers = checkers.Where(ch => ch != null && !ch.IsBeat).ToList();
            var order = activeCheckers.OrderByDescending(ch => ch.Square.Position.X + ch.Square.Position.Y).ToList();

            var checkersVisualsOrder = order.Select(ch => BoardController.Instance.GetVisualFromChecker(ch.Id)).ToList();

            if (time > 0f)
                yield return new WaitForEndOfFrame();

            var currentWeight = order[0].Square.Position.X + order[0].Square.Position.Y;

            for (int i = 0; i < checkersVisualsOrder.Count; i++)
            {
                var lastWeight = order[i].Square.Position.X + order[i].Square.Position.Y;
                if (lastWeight < currentWeight)
                {
                    currentWeight = lastWeight;
                    if (time > 0f)
                        yield return new WaitForSeconds(_showStepTime);
                }
                checkersVisualsOrder[i].Show(time);
            }

            _isContainerActive = true;
            _isAnimationActive = false;
        }

        /// <summary>
        /// Hide checkers container.
        /// </summary>
        public void Hide(float time = 0f)
        {
            if (!_isContainerActive || _isAnimationActive)
            {
                return;
            }

            if (_hideRoutine != null)
            {
                StopCoroutine(_hideRoutine);
            }

            _hideRoutine = HideRoutine(time);
            StartCoroutine(_hideRoutine);
        }

        public IEnumerator HideRoutine(float time = 0f)
        {
            if (_checkers.Count < 1)
            {
                yield break;
            }

            _isAnimationActive = true;

            var checkers = _checkers.Select(ch => GameController.Instance.CoreInstance.GetChecker(ch.Id)).ToList();
            var activeCheckers = checkers.Where(ch => ch != null && !ch.IsBeat).ToList();
            var order = activeCheckers.OrderBy(ch => ch.Square.Position.X + ch.Square.Position.Y).ToList();

            var checkersVisualsOrder = order.Select(ch => BoardController.Instance.GetVisualFromChecker(ch.Id)).ToList();

            if (time > 0f)
                yield return new WaitForEndOfFrame();

            var currentWeight = order[0].Square.Position.X + order[0].Square.Position.Y;
            for (int i = 0; i < checkersVisualsOrder.Count; i++)
            {
                var lastWeight = order[i].Square.Position.X + order[i].Square.Position.Y;
                if (lastWeight > currentWeight)
                {
                    currentWeight = lastWeight;
                    if (time > 0f)
                        yield return new WaitForSeconds(_hideStepTime);
                }
                checkersVisualsOrder[i].Hide(time);
            }

            _isContainerActive = false;
            _isAnimationActive = false;
        }

        /// <summary>
        /// Set container active state.
        /// Use for undo.
        /// </summary>
        public void SetContainerActive()
        {
            _isContainerActive = true;
        }
    }
}