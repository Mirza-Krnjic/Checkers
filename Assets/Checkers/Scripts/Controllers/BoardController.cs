using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace Checkers
{
    public class BoardController : Singleton<BoardController>
    {
        [Header("Components:")]
        public GameController GameControllerComponent;
        public CameraController CameraControllerComponent;

        [Space(5f)]
        [Header("Board visual:")]
        public BoardVisual Visual;
        public CheckersContainerController CheckersContainer;

        [Space(5f)]
        [Header("Current turn components:")]
        public GameObject TurnObjectParentPerspective;
        public GameObject TurnObjectParentTopDown;

        [Space(5f)]
        public Sprite CurrentColorBlackSprite;
        public Sprite CurrentColorWhiteSprite;
        public Image CurrentTurnColor;
        public Image CurrentTurnBlackColorImageForTopDownCamera;
        public Image CurrentTurnWhiteColorImageForTopDownCamera;

        [Header("Square/Checkers arrays:")]
        public List<SquareVisual> BlackSquares = new List<SquareVisual>();
        public List<SquareVisual> WhiteSquares = new List<SquareVisual>();
        public List<CheckerVisual> Checkers = new List<CheckerVisual>();

        [Space(10)]
        public Image BlockRaycastImage;

        [Header("Checker materials:")]
        [SerializeField]
        private Material _whiteCheckerMaterial;

        [SerializeField]
        private Material _blackCheckerMaterial;
        [SerializeField]
        private Material _shadowCheckerMaterial;

        [Header("Checker sprites")]
        [SerializeField]
        private Sprite _blackCheckerSprite;
        [SerializeField]
        private Sprite _whiteCheckerSprite;

        [Header("Checker shadow colors:")]
        [SerializeField]
        private Color _shadowWhiteColor;
        [SerializeField]
        private Color _shadowBlackColor;

        [Header("Checker start position:")]
        [SerializeField]
        private Transform _startPositionForBlack;
        [SerializeField]
        private Transform _startPositionForWhite;

        private CheckerColor _choosenCheckerColor;
        private readonly IDictionary<int, CheckerVisual> _checkersViews = new Dictionary<int, CheckerVisual>();
        private readonly IDictionary<int, SquareVisual> _squaresViews = new Dictionary<int, SquareVisual>();
        private int _squareSize;
        private float _blackRemoveStackPosition;
        private float _whiteRemoveStackPosition;
        private bool _isBoardPreparing;
        private Color _blackSquareColor;

        public bool IsBoardPrepared;

        /// <summary>
        /// Initialize of checkers stacks.
        /// </summary>
        private void InitCheckersStacks()
        {
            _blackRemoveStackPosition = 0f;
            _whiteRemoveStackPosition = 0f;
        }

        /// <summary>
        /// Initialize color of all checkers on start game.
        /// </summary>
        public void InitColor()
        {
            foreach (var item in Checkers)
            {
                item.CheckerRenderer.material = new Material(_whiteCheckerMaterial);
            }
        }

        /// <summary>
        /// Called after click on black/white color buttons.
        /// </summary>
        /// <param name="userColor"></param>
        public void SetColorPlayer(UserColor userColor)
        {
            GameControllerComponent.CurrentUserColor = userColor;

            if (userColor == UserColor.White)
            {
                _choosenCheckerColor = CheckerColor.White;
            }
            else
            {
                _choosenCheckerColor = CheckerColor.Black;
            }
            SetMaterialToShadowChecker(userColor);
        }

        /// <summary>
        /// Board initialization.
        /// </summary>
        public void OnBoardPrepared(List<Square> squaresInitialPositions, List<Checker> checkersInitialPositions, bool undo = false)
        {
            _isBoardPreparing = true;

            Visual.UpdateView();

            InitCheckersStacks();

            _squareSize = squaresInitialPositions.Max(square => square.Position.Y) + 1;

            int blackCounter = 0, whiteCounter = 0;
            foreach (var square in squaresInitialPositions)
            {
                SquareVisual squareView;

                if (square.Color == SquareColor.White)
                {
                    squareView = WhiteSquares[whiteCounter];
                    whiteCounter++;
                }
                else
                {
                    squareView = BlackSquares[blackCounter];
                    squareView.ShadowRenderer.enabled = false;
                    blackCounter++;
                }

                squareView.transform.localPosition = SquareScreenPosition(square.Position);

                squareView.Init(square.Id);
                _squaresViews[square.Id] = squareView;

                if (square.Color == SquareColor.Black && _blackSquareColor == Color.clear)
                    _blackSquareColor = squareView.SquareMaterial.color;
            }

            float intervalWhite = 0.15f, intervalBlack = 0.15f;

            int CheckerCounter = 0;

            for (int i = 0; i < checkersInitialPositions.Count; i++)
            {
                Checker checker = checkersInitialPositions[i];
                if (checker.IsBeat)
                {
                    continue;
                }

                CheckerVisual checkerView = undo && !GameController.Instance.IsContinue ? Checkers.First(ch => ch.Id == checker.Id) : Checkers[CheckerCounter];

                checkerView.SuperCheckerRenderer.enabled = false;

                if (checker.Color == CheckerColor.White)
                {
                    checkerView.CheckerRenderer.material = (_choosenCheckerColor == CheckerColor.White) ? new Material(_whiteCheckerMaterial) : new Material(_blackCheckerMaterial);
                    if (!undo)
                        checkerView.transform.localPosition = new Vector3(_startPositionForWhite.position.x, _startPositionForWhite.position.y + intervalWhite, _startPositionForWhite.position.z);
                    intervalWhite += 0.15f;
                }
                else
                {
                    checkerView.CheckerRenderer.material = (_choosenCheckerColor == CheckerColor.Black) ? new Material(_whiteCheckerMaterial) : new Material(_blackCheckerMaterial);
                    if (!undo)
                        checkerView.transform.localPosition = new Vector3(_startPositionForBlack.position.x, _startPositionForBlack.position.y + intervalBlack, _startPositionForBlack.position.z);
                    intervalBlack += 0.15f;
                }
                CheckerCounter++;
                checkerView.Init(checker.Id);
                checkerView.SetEnableCollider(!checker.IsBeat);
                checkerView.ChoosenCheckerRenderer.enabled = false;
                _checkersViews[checker.Id] = checkerView;
            }
            if (!undo)
            {
                CheckersContainer.Show(0f);
            }
            else
            {
                CheckersContainer.SetContainerActive();
            }

            StartCoroutine(undo ? UndoAnimation(checkersInitialPositions) : StartAnimation(checkersInitialPositions));
        }

        /// <summary>
        /// Called when user click on checker.
        /// </summary>
        public void OnCheckerClicked(int id)
        {
            if (_isBoardPreparing)
            {
                return;
            }

            GameControllerComponent.CoreInstance.FindAvailableSquares(id);
        }

        /// <summary>
        /// Called when user click on square.
        /// </summary>
        /// <param name="squareId"></param>
        public void OnSquareClicked(int squareId)
        {
            if (_isBoardPreparing)
            {
                return;
            }

            GameControllerComponent.CoreInstance.TryToMoveChecker(squareId);
        }

        /// <summary>
        /// Dealing checkers animation on game start.
        /// </summary>
        private IEnumerator StartAnimation(List<Checker> checker)
        {
            for (int i = Mathf.CeilToInt(checker.Count / 2f); i > 0; i--)
            {
                var viewEnemy = _checkersViews[checker[Mathf.CeilToInt(checker.Count / 2) + i - 1].Id];
                var enemyPosition = checker[Mathf.CeilToInt(checker.Count / 2) + i - 1].Square.Position;
                viewEnemy.transform.DOKill();
                viewEnemy.transform.DOLocalMove(CheckerScreenPosition(enemyPosition), 0.2f).SetEase(Ease.OutSine);
                AudioController.Instance.PlayOneShotAudio(AudioController.AudioType.Move);

                var viewPlayer = _checkersViews[checker[i - 1].Id];
                var playerPosition = checker[i - 1].Square.Position;
                viewPlayer.transform.DOKill();
                viewPlayer.transform.DOLocalMove(CheckerScreenPosition(playerPosition), 0.2f).SetEase(Ease.OutSine);
                AudioController.Instance.PlayOneShotAudio(AudioController.AudioType.Move);

                yield return new WaitForSeconds(0.1f);
            }

            _isBoardPreparing = false;
            GameController.Instance.CoreInstance.FindAvailableSquaresForColor();
            ShowAvailableForMoveCheckers();
        }

        /// <summary>
        /// Dealing checkers animation on game start when undo process.
        /// </summary>
        private IEnumerator UndoAnimation(List<Checker> checker)
        {
            yield return new WaitForEndOfFrame();
            List<Checker> whiteCheckers = checker.FindAll(x => x.Color == CheckerColor.White);
            List<Checker> blackCheckers = checker.FindAll(x => x.Color == CheckerColor.Black);

            int count = whiteCheckers.Count > blackCheckers.Count ? whiteCheckers.Count : blackCheckers.Count;
            var undoTime = DataConfig.Instance.UndoAnimationTime;

            for (int i = 0; i < count; i++)
            {
                if (i < blackCheckers.Count)
                {
                    Checker blackChecker = blackCheckers[i];
                    if (blackChecker.Color == CheckerColor.Black && !blackChecker.IsBeat)
                    {
                        var viewEnemy = _checkersViews[blackChecker.Id];
                        viewEnemy.transform.DOKill();
                        viewEnemy.transform.DOLocalMove(CheckerScreenPosition(blackChecker.Square.Position), undoTime).SetEase(Ease.Linear);
                        AudioController.Instance.PlayOneShotAudio(AudioController.AudioType.Move);
                    }
                }

                if (i < whiteCheckers.Count)
                {
                    Checker whiteChecker = whiteCheckers[i];
                    if (whiteChecker.Color == CheckerColor.White && !whiteChecker.IsBeat)
                    {
                        var viewPlayer = _checkersViews[whiteChecker.Id];
                        viewPlayer.transform.DOKill();
                        viewPlayer.transform.DOLocalMove(CheckerScreenPosition(whiteChecker.Square.Position), undoTime).SetEase(Ease.Linear);
                        AudioController.Instance.PlayOneShotAudio(AudioController.AudioType.Move);
                    }
                }
            }
            _isBoardPreparing = false;
            GameController.Instance.CoreInstance.FindAvailableSquaresForColor();
            ShowAvailableForMoveCheckers();
        }

        /// <summary>
        /// Position of square at screen.
        /// </summary>
        private Vector3 SquareScreenPosition(Position position)
        {
            return new Vector3(0.5f - _squareSize / 2f + position.X, 0, 0.5f - _squareSize / 2f + position.Y);
        }

        /// <summary>
        /// Position of checker at screen.
        /// </summary>
        private Vector3 CheckerScreenPosition(Position position)
        {
            return new Vector3(0.5f - _squareSize / 2f + position.X, .28f, 0.5f - _squareSize / 2f + position.Y);
        }

        /// <summary>
        /// Change shadow material depending on color of user checker.
        /// </summary>
        private void SetMaterialToShadowChecker(UserColor userColor)
        {
            _shadowCheckerMaterial.color = userColor == UserColor.White ? _shadowWhiteColor : _shadowBlackColor;
        }

        /// <summary>
        /// Called when need change current turn.
        /// </summary>
        public void ChangeCurrentTurnImage()
        {
            if (CameraControllerComponent.IsPerspectiveCamera)
            {
                InitTopDownCurrentTurnObectsValues();
            }
            else
            {
                InitPerspectiveCurrentTurnObectsValues();
            }
        }

        /// <summary>
        /// Called when need change current turn.
        /// </summary>
        public void InitCurrentTurnObjects()
        {
            TurnObjectParentPerspective.SetActive((CameraControllerComponent.IsPerspectiveCamera) ? true : false);
            TurnObjectParentTopDown.SetActive((CameraControllerComponent.IsPerspectiveCamera) ? false : true);

            ChangeCurrentTurnImage();
        }

        /// <summary>
        /// Change cuurent turn player sprite of top/down objects.
        /// </summary>
        private void InitTopDownCurrentTurnObectsValues()
        {
            if (GameControllerComponent.CoreInstance.CurrentMoveColor == CheckerColor.White)
            {
                CurrentTurnColor.sprite = (_choosenCheckerColor == CheckerColor.White) ? CurrentColorWhiteSprite : CurrentColorBlackSprite;
            }
            else
            {
                CurrentTurnColor.sprite = (_choosenCheckerColor == CheckerColor.Black) ? CurrentColorWhiteSprite : CurrentColorBlackSprite;
            }
        }

        /// <summary>
        /// Switch visual elements of current turn player of perspective objects.
        /// </summary>
        private void InitPerspectiveCurrentTurnObectsValues()
        {
            if (GameControllerComponent.CoreInstance.CurrentMoveColor == CheckerColor.White)
            {
                CurrentTurnWhiteColorImageForTopDownCamera.enabled = (GameControllerComponent.CoreInstance.CurrentMoveColor == CheckerColor.White);
                CurrentTurnBlackColorImageForTopDownCamera.enabled = !(GameControllerComponent.CoreInstance.CurrentMoveColor == CheckerColor.White);
                CurrentTurnWhiteColorImageForTopDownCamera.sprite = (_choosenCheckerColor == CheckerColor.White) ? CurrentColorWhiteSprite : CurrentColorBlackSprite;
            }
            else
            {
                CurrentTurnWhiteColorImageForTopDownCamera.enabled = !(GameControllerComponent.CoreInstance.CurrentMoveColor == CheckerColor.Black);
                CurrentTurnBlackColorImageForTopDownCamera.enabled = (GameControllerComponent.CoreInstance.CurrentMoveColor == CheckerColor.Black);
                CurrentTurnBlackColorImageForTopDownCamera.sprite = (_choosenCheckerColor == CheckerColor.Black) ? CurrentColorWhiteSprite : CurrentColorBlackSprite;
            }
        }

        /// <summary>
        /// Move checker animation.
        /// </summary>
        /// <param name="checker"></param>
        public void OnMoveChecker(Checker checker)
        {
            var view = _checkersViews[checker.Id];
            int lastCheckerOrder = view.CheckerRenderer.sortingOrder;
            AudioController.Instance.PlayOneShotAudio(AudioController.AudioType.Move);
            view.transform.DOKill();
            view.transform.DOLocalMove(CheckerScreenPosition(checker.Square.Position), 0.4f).SetEase(Ease.OutSine).OnComplete(() =>
            {
                GameController.Instance.CoreInstance.SetUserMoveState(false);
            });
        }

        /// <summary>
        /// Remove checker action.
        /// </summary>
        /// <param name="checker"></param>
        public void OnRemoveChecker(Checker checker)
        {
            var view = _checkersViews[checker.Id];

            Vector3 stackPosition;
            if (checker.Color == CheckerColor.White)
            {
                _blackRemoveStackPosition += 0.15f;
                stackPosition = new Vector3(_startPositionForBlack.position.x,
                                            _startPositionForBlack.position.y + _blackRemoveStackPosition,
                                            _startPositionForBlack.position.z);

            }
            else
            {
                _whiteRemoveStackPosition += 0.15f;
                stackPosition = new Vector3(_startPositionForWhite.position.x,
                                            _startPositionForWhite.position.y + _whiteRemoveStackPosition,
                                            _startPositionForWhite.position.z);
            }

            Vector3 upPosition = CheckerScreenPosition(checker.Square.Position);
            view.transform.DOKill(true);
            view.transform.DOLocalMove(new Vector3(upPosition.x, upPosition.y + 0.2f, upPosition.z), 1f).SetEase(Ease.OutSine).OnComplete(delegate { CompleteRemoveAnimation(view, stackPosition, checker.Id); });
            view.CheckerCollider.enabled = false;
        }

        /// <summary>
        /// Remove animation of checker.
        /// </summary>
        private void CompleteRemoveAnimation(CheckerVisual view, Vector3 stackPosition, int Id)
        {
            view.transform.DOKill();
            view.transform.DOLocalMove(stackPosition, 0.2f).SetEase(Ease.InExpo);
            _checkersViews.Remove(Id);
        }

        /// <summary>
        /// Shake animation of checker.
        /// </summary>
        /// <param name="checker"></param>
        public void ShakeChecker(Checker checker)
        {
            var view = _checkersViews[checker.Id];
            view.transform.DOKill();
            view.transform.DOShakePosition(0.2f, new Vector3(0.05f, 0, 0.05f), vibrato: 15);
        }

        /// <summary>
        /// Turn on shadows of available moves for checker.
        /// </summary>
        public void MarkSquares(List<Square> availableSquares)
        {
            foreach (var square in availableSquares)
            {
                var view = _squaresViews[square.Id];
                view.ShadowRenderer.enabled = true;
            }
        }

        /// <summary>
        /// Turn off shadows of available moves for checker.
        /// </summary>
        public void UnmarkSquares(List<Square> availableSquares)
        {
            foreach (var square in availableSquares)
            {
                var view = _squaresViews[square.Id];
                view.ShadowRenderer.enabled = false;
            }
        }

        /// <summary>
        /// Change state of checker to Super.
        /// </summary>
        public void SetCrownChecker(Checker checker)
        {
            if (!_checkersViews.ContainsKey(checker.Id))
            {
                return;
            }
            var view = _checkersViews[checker.Id].SuperCheckerRenderer;
            view.enabled = true;

            if (checker.Color == CheckerColor.White)
            {
                view.sprite = (_choosenCheckerColor == CheckerColor.White) ? _blackCheckerSprite : _whiteCheckerSprite;
            }
            else
            {
                view.sprite = (_choosenCheckerColor == CheckerColor.Black) ? _blackCheckerSprite : _whiteCheckerSprite;
            }
        }

        /// <summary>
        /// Reser ChoosenCheckerRenderer to default state. Decline choosing of checker
        /// </summary>
        public void ResetCheckersChoosing()
        {
            foreach (var ch in Checkers)
            {
                ch.ChoosenCheckerRenderer.enabled = false;
                ch.ChoosenCheckerRenderer.color = Color.white;
            }
        }

        /// <summary>
        /// Show checkers which can move.
        /// </summary>
        public void ShowAvailableForMoveCheckers()
        {
            GameController gameCtrl = GameController.Instance;
            Core core = gameCtrl.CoreInstance;

            if (gameCtrl.Mode == GameMode.PlayerVsAI || gameCtrl.Mode == GameMode.Puzzle)
            {
                if (core.CurrentMoveColor == CheckerColor.Black || core.IsBeatProcessActive)
                {
                    return;
                }
            }
            List<int> beatCheckers = core.FindBeatCheckersIds(core.CurrentMoveColor);
            List<int> checkersId = beatCheckers.Count > 0 && !GameController.Instance.CoreInstance.GameRule.Options.IsNotRequiredBeating ? beatCheckers : core.CheckersWhichCanMove;

            foreach (var id in checkersId)
            {
                var view = _checkersViews[id].ChoosenCheckerRenderer;
                Checker ch = core.GetChecker(id);
                if (ch.Color == core.CurrentMoveColor)
                {
                    var color = DataConfig.Instance.AvailableForMoveCheckerColor;
                    view.enabled = true;
                    view.color = new Color32(color.r, color.g, color.b, (byte)(view.color.a * 255));
                }
            }
        }

        /// <summary>
        /// Choose checker action by id.
        /// </summary>
        public void ChooseChecker(int id)
        {
            ResetCheckersChoosing();

            CheckerVisual checker = _checkersViews[id];

            if (checker != null && checker.Id != 0)
            {
                var color = DataConfig.Instance.ChoosenCheckerColor;
                checker.ChoosenCheckerRenderer.enabled = true;
                checker.ChoosenCheckerRenderer.color = new Color32(color.r, color.g, color.b, (byte)(checker.ChoosenCheckerRenderer.color.a * 255));
            }
        }

        /// <summary>
        /// Get checker visual component.
        /// </summary>
        public CheckerVisual GetVisualFromChecker(int checkerId)
        {
            return Checkers.First(x => x.Id == checkerId);
        }

        public void Reset()
        {
            InitCheckersStacks();
        }
    }
}
