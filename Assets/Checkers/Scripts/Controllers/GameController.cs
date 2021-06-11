using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Checkers
{
    public class GameController : Singleton<GameController>
    {
        public UnityEvent ChangeCamera;

        public UnityEvent StartNewGameEvent;
        

        [Header("Components: ")]
        public BoardController BoardViewCompoennt;
        public UiViewController UiViewComponent;
        public Toggle DifficultyToggleEasy;
        public Toggle DifficultyToggleHard;

        public bool IsGameStart;
        public UserColor CurrentUserColor;

        [Space]
        public Core CoreInstance;

        public IEnumerator _moveCoroutine;
        public GameMode Mode;
        public GameResult Result { get; private set; }
        public bool IsContinue { get; set; }
        public bool IsRestart { get; set; }

        public void Update()
        {
            MovesAction();
        }

        /// <summary>
        /// Bot Move actions.
        /// </summary>
        private void MovesAction()
        {
            if (IsGameStart)
            {
                if (CoreInstance.CurrentMoveColor == CheckerColor.Black)
                {
                    switch (Mode)
                    {
                        case GameMode.PlayerVsAI:
                        case GameMode.Puzzle:
                            if (!CoreInstance.IsAiMove)
                            {
                                if (_moveCoroutine != null)
                                {
                                    StopCoroutine(_moveCoroutine);
                                }
                                _moveCoroutine = CoreInstance.BotMove();
                                StartCoroutine(_moveCoroutine);
                            }
                            break;
                        case GameMode.PlayerVsPlayer:
                            if (!CoreInstance.IsAiMove)
                            {
                                if (_moveCoroutine != null)
                                {
                                    StopCoroutine(_moveCoroutine);
                                }
                                _moveCoroutine = CoreInstance.SecondPlayerMove();
                                StartCoroutine(_moveCoroutine);
                            }
                            break;
                    }
                }
            }
        }
        private void Awake()
        {
            if (PlayerPrefs.HasKey("ishard"))
            {
                if (PlayerPrefs.GetInt("ishard") == 0)
                {
                    DifficultyToggleEasy.isOn = true;
                    DifficultyToggleHard.isOn = false;
                    setEasy();
                }
                else
                {
                    DifficultyToggleEasy.isOn = false;
                    DifficultyToggleHard.isOn = true;
                    setHard();
                }
            }
        }
        public void setEasy()
        {
            PlayerPrefs.SetInt("ishard", 0);
            CoreInstance.setDifficultyEasy();
        }
        public void setHard()
        {
            PlayerPrefs.SetInt("ishard", 1);
            CoreInstance.setDifficultyHard();
        }
        /// <summary>
        /// Start game action.
        /// </summary>
        [ContextMenu("Start")]
        public void StartGame()
        {
            
            UiViewComponent.ResetUIView();
            BoardViewCompoennt.Reset();
            ChangeGameResult(GameResult.None);

            CoreInstance = new Core
            {
                GameRule = GameRulesController.Instance.GetRule()
            };

            IsGameStart = true;
            BoardViewCompoennt.InitCurrentTurnObjects();
            if (IsContinue)
            {
                UndoPerformer undoCtrl = UndoPerformer.Instance;
                CoreInstance.FirstTurnPlayer(CheckerColor.White);
                undoCtrl.LoadGame();

                GameMode undoMode = undoCtrl.StatesData.Type;
                ChooseMode(undoMode);
                PuzzleController.Instance.SetPuzzleState(undoMode == GameMode.Puzzle);

                if (undoMode == GameMode.Puzzle)
                {
                    PuzzleController.Instance.SetPuzzle(undoCtrl.StatesData.PuzzleRec.PuzzleId);
                    CoreInstance.InitPuzzle(undoCtrl.StatesData.PuzzleRec.PassedMoves);
                    CoreInstance.InitPassedMoveText();
                }

                UndoPerformer.Instance.Undo();
            }
            else if (Mode == GameMode.Puzzle)
            {
                UndoPerformer.Instance.DeleteLastGame();
                UndoPerformer.Instance.ResetUndoStates();

                Puzzle puzzle = PuzzleController.Instance.CurrentPuzzle;

                CurrentUserColor = UserColor.White;
                CoreInstance.FirstTurnPlayer(CheckerColor.White);
                CoreInstance.PrepareBoard(CurrentUserColor, PuzzleController.Instance.IsPuzzleGameActive, puzzle);
            }
            else
            {
                UndoPerformer.Instance.DeleteLastGame();
                UndoPerformer.Instance.ResetUndoStates();

                CoreInstance.FirstTurnPlayer((CurrentUserColor == UserColor.White) ? CheckerColor.White : CheckerColor.Black);
                CoreInstance.PrepareBoard(CurrentUserColor);
            }
            StartNewGameEvent?.Invoke();
            IsRestart = false;

            if (DifficultyToggleEasy.isOn)
                setEasy();
            else
                setHard();
        }

        /// <summary>
        /// Restart game action.
        /// </summary>
        public void Restart()
        {
            AdsController.Instance.ShowInterstitial();
            IsGameStart = false;
            IsRestart = true;
            if (_moveCoroutine != null)
            {
                StopCoroutine(_moveCoroutine);
            }

            BoardViewCompoennt.Reset();
            UndoPerformer.Instance.ResetUndoStates();
            StartGame();
        }

        /// <summary>
        /// Change game mode.
        /// </summary>
        public void ChooseMode(GameMode mode)
        {
            Mode = mode;
        }

        /// <summary>
        /// Update result state.
        /// </summary>
        public void ChangeGameResult(GameResult result)
        {
            Result = result;
        }
        
        /// <summary>
        /// Save state when application Pause.
        /// </summary>
        public void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                if (CoreInstance != null && CoreInstance.IsUserMadeFirstMove && !CoreInstance.GameEnd)
                {
                    CoreInstance.WriteUndoStates();
                    UndoPerformer.Instance.SaveGame();
                }
            }
            else
            {
                if (CoreInstance != null && CoreInstance.IsUserMadeFirstMove && !CoreInstance.GameEnd)
                {
                    UndoPerformer.Instance.Undo();
                }
            }
        }

        /// <summary>
        /// Save state when application focus.
        /// </summary>
        public void OnApplicationFocus(bool focus)
        {
            if (!focus)
            {
                if (CoreInstance != null && CoreInstance.IsUserMadeFirstMove && !CoreInstance.GameEnd)
                {
                    CoreInstance.WriteUndoStates();
                    UndoPerformer.Instance.SaveGame();
                }
            }
            else
            {
                if (CoreInstance != null && CoreInstance.IsUserMadeFirstMove && !CoreInstance.GameEnd)
                {
                    UndoPerformer.Instance.Undo();
                }
            }
        }

        /// <summary>
        /// Set state of continue game to TRUE.
        /// </summary>
        public void Continue()
        {
            IsContinue = true;
            StartGame();
        }
    }
}