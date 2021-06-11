using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Checkers
{
    public class ChooseColorPopup : GamePopup
    {
        [Header("Choose side components:")]
        public Toggle WhiteToggle;
        public Toggle BlackToggle;
        public ToggleGroup Group;

        [Space(10)]
        public Image BlockRaycastImage;

        private IEnumerator _randColorCoroutine;
        private bool _isRandomColor;

        /// <summary>
        /// Called after click on random color button.
        /// </summary>
        public void RandomizeColor()
        {
            if (_randColorCoroutine != null)
            {
                StopCoroutine(_randColorCoroutine);
            }
            _randColorCoroutine = RandColor();
            StartCoroutine(_randColorCoroutine);
        }

        public void OnColorToggleSet(UserColor userColor)
        {
            if (_isRandomColor)
            {
                return;
            }

            BoardController.Instance.SetColorPlayer(userColor);

            //BoardController.Instance.GameControllerComponent.StartGame();

            //Close();
        }

        /// <summary>
        /// User color Randomization action.
        /// </summary>
        private IEnumerator RandColor()
        {
            _isRandomColor = true;

            BlockRaycastImage.enabled = true;

            yield return new WaitForSeconds(.5f);

            int count = Random.Range(5, 10);

            for (int i = 1; i <= count; i++)
            {
                AudioController.Instance.PlayOneShotAudio(AudioController.AudioType.Random);
                WhiteToggle.isOn = true;
                yield return new WaitForSeconds(.5f / i);
                AudioController.Instance.PlayOneShotAudio(AudioController.AudioType.Random);
                BlackToggle.isOn = true;
                yield return new WaitForSeconds(.5f / i);
            }

            yield return new WaitForSeconds(.5f / count);

            _isRandomColor = false;
            Group.SetAllTogglesOff();
            AudioController.Instance.PlayOneShotAudio(AudioController.AudioType.Random);
            if (count % 2 == 0)
            {
                WhiteToggle.isOn = true;
            }
            else
            {
                BlackToggle.isOn = true;
            }

            BlockRaycastImage.enabled = false;

            yield return new WaitForSeconds(.5f);

            BoardController.Instance.GameControllerComponent.StartGame();

            Close();
        }

        public override void Close()
        {
            if (_isRandomColor)
            {
                return;
            }

            Group.SetAllTogglesOff();

            base.Close();
        }
    }
}