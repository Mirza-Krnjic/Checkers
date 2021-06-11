using UnityEngine;

namespace Checkers
{
    public class MenuPopup : GamePopup
    {
        [SerializeField]
        private GameObject _continueButton;

        private bool _isOpenedFromMainGame = false;

        public void OnEnable()
        {
            UpdateView();
        }

        private void UpdateView()
        {
            UpdateContinueButtonState();
        }

        /// <summary>
        /// Should called from active game.
        /// </summary>
        public void OpenWhenGameActive()
        {
            BoardController.Instance.ResetCheckersChoosing();
            GameController.Instance.CoreInstance.ClearAvailableSquares();

            BoardController.Instance.CheckersContainer.Hide(0.5f);

            _isOpenedFromMainGame = true;
            base.Open();
        }

        private void UpdateContinueButtonState()
        {
            var state = UndoPerformer.Instance.IsHasGame() && !_isOpenedFromMainGame;
            _continueButton.SetActive(state);
        }

        public void CloseWhenGameActive()
        {
            BoardController.Instance.ShowAvailableForMoveCheckers();
            BoardController.Instance.CheckersContainer.Show(0.5f);

            _isOpenedFromMainGame = false;
            base.Close();
        }
    }
}