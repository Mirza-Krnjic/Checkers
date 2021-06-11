using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Checkers
{
    public class UndoPerformer : Singleton<UndoPerformer>
    {
        [Header("Components:")]
        [SerializeField]
        private Button _undoButton;

        [Tooltip("Array with all states.")]
        public UndoData StatesData;

        private readonly string _lastGameKey = "LastGameKey";

        private void OnEnable()
        {
            GameRulesController.Instance.ChangeRuleEvent -= ResetSavedGameWhenRuleChanage;
        }

        private void OnDisable()
        {
            GameRulesController.Instance.ChangeRuleEvent -= ResetSavedGameWhenRuleChanage;
        }

        public void ResetSavedGameWhenRuleChanage()
        {
            if (IsHasGame())
            {
                ClearUndoRecord();

                string game = JsonUtility.ToJson(StatesData);
                PlayerPrefs.SetString(_lastGameKey, game);
            }
        }

        /// <summary>
        /// Action of Undo all decks/cards states.
        /// </summary>
        public void Undo()
        {
            GameController gameCtrl = GameController.Instance;
            Core core = gameCtrl.CoreInstance;

            if (gameCtrl.Mode == GameMode.PlayerVsPlayer || core.IsAiMove)
            {
                return;
            }

            if (StatesData.States.Count > 0)
            {
                if (PuzzleController.Instance.IsPuzzleGameActive && core.IsUserMadeFirstMove)
                {
                    core.PassedMoves--;
                    core.InitPassedMoveText();
                }
                UndoStates state = GetLastState();
                StatesData.States.RemoveAt(StatesData.States.Count - 1);
                core.PrepareBoardAction(state.BoardSquares, state.CheckersDataPos);
                core.IsBeatProcessActive = state.BeatProcessActivity;
                core.SetCurrentChecker(state.CurrentCheckerId);
                core.IsUserMove = true;
                if (core.IsBeatProcessActive)
                {
                    core.ClearAvailableSquares();
                    core.FindAvailableSquares(state.CurrentCheckerId, true);
                    BoardController.Instance.ChooseChecker(state.CurrentCheckerId);
                }
                ActivateUndoButton();
            }
        }

        /// <summary>
        /// Get last undo state from list.
        /// </summary>
        /// <returns></returns>
        public UndoStates GetLastState()
        {
            return StatesData.States[StatesData.States.Count - 1];
        }

        /// <summary>
        /// Collect new state.
        /// </summary>
        public void AddUndoState(Dictionary<int, Square> squareStates, Dictionary<int, Checker> checkerStates, Square[,] boardSquares)
        {
            GameController gameCtrl = GameController.Instance;
            Core core = gameCtrl.CoreInstance;

            if (gameCtrl.Mode == GameMode.PlayerVsPlayer)
            {
                return;
            }

            StatesData.Type = gameCtrl.Mode;
            StatesData.RuleRec = new RuleRecord(GameRulesController.Instance.CurrentRule);

            if (PuzzleController.Instance.IsPuzzleGameActive)
            {
                StatesData.PuzzleRec = PuzzleController.Instance.IsPuzzleGameActive ?
                                       new PuzzleRecord(PuzzleController.Instance.CurrentPuzzle.PuzzleId, core.PassedMoves) :
                                       null;
            }

            Checker ch = core.GetCurrentChecker();
            StatesData.States.Add(new UndoStates(squareStates, checkerStates, boardSquares, core.IsBeatProcessActive,
                ch != null ? ch.Id: 0));
        }

        /// <summary>
        /// Activate for user undo button on bottom panel.
        /// </summary>
        public void ActivateUndoButton()
        {
            GameController gameCtrl = GameController.Instance;
            Core core = gameCtrl.CoreInstance;

            if (gameCtrl.Mode == GameMode.PlayerVsPlayer)
            {
                return;
            }

            bool isHasUndoState = IsHasUndoState() && core.IsUserMadeFirstMove;

            _undoButton.interactable = isHasUndoState;
        }

        /// <summary>
        /// Check for existing undo states.
        /// </summary>
        private bool IsHasUndoState()
        {
            return StatesData.States.Count > 0;
        }

        /// <summary>
        /// Clear array with states
        /// </summary>
        public void ResetUndoStates()
        {
            GameController gameCtrl = GameController.Instance;

            if (gameCtrl.Mode == GameMode.PlayerVsPlayer)
            {
                return;
            }

            ClearUndoRecord();
            ActivateUndoButton();
        }

        /// <summary>
        /// Clear undo data.
        /// </summary>
        public void ClearUndoRecord()
        {
            StatesData.RuleRec = null;
            StatesData.PuzzleRec = null;
            StatesData.States.Clear();
        }

        /// <summary>
        /// Save game with current game state.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="steps"></param>
        /// <param name="score"></param>
        public void SaveGame()
        {
            GameController gameCtrl = GameController.Instance;

            if (gameCtrl.Mode == GameMode.PlayerVsPlayer || !gameCtrl.CoreInstance.IsUserMadeFirstMove)
            {
                return;
            }

            string game = JsonUtility.ToJson(StatesData);
            PlayerPrefs.SetString(_lastGameKey, game);
        }

        /// <summary>
        /// Load game if it exist.
        /// </summary>
        public void LoadGame()
        {
            if (PlayerPrefs.HasKey(_lastGameKey))
            {
                string game = PlayerPrefs.GetString(_lastGameKey);
                StatesData = JsonUtility.FromJson<UndoData>(game);
            }
        }

        /// <summary>
        /// Is has saved game process.
        /// </summary>
        public bool IsHasGame()
        {
            bool isHasGame = false;

            if (PlayerPrefs.HasKey(_lastGameKey))
            {
                string lastGameData = PlayerPrefs.GetString(_lastGameKey);
                UndoData data = JsonUtility.FromJson<UndoData>(lastGameData);

                if (data != null && data.States.Count > 0)
                {
                    isHasGame = true;
                }
            }

            return isHasGame;
        }

        /// <summary>
        /// Get last game data.
        /// </summary>
        /// <returns></returns>
        public UndoData GetLastGame()
        {
            if (PlayerPrefs.HasKey(_lastGameKey))
            {
                string lastGameData = PlayerPrefs.GetString(_lastGameKey);
                UndoData data = JsonUtility.FromJson<UndoData>(lastGameData);

                if (data != null && data.States.Count > 0)
                {

                    return data;
                }
            }
            return null;
        }


        /// <summary>
        /// Delete last game.
        /// </summary>
        public void DeleteLastGame()
        {
            GameController gameCtrl = GameController.Instance;

            if (gameCtrl.Mode == GameMode.PlayerVsPlayer)
            {
                return;
            }

            PlayerPrefs.DeleteKey(_lastGameKey);
        }
    }

    [System.Serializable]
    public class UndoData
    {
        public GameMode Type;
        public RuleRecord RuleRec;
        public PuzzleRecord PuzzleRec;
        public List<UndoStates> States = new List<UndoStates>();
    }

    [System.Serializable]
    public class UndoStates
    {
        public int CurrentCheckerId;
        public bool BeatProcessActivity;
        public Square[,] BoardSquares;

        public List<CheckerPositions> CheckersDataPos = new List<CheckerPositions>();

        public UndoStates(Dictionary<int, Square> squareStates, Dictionary<int, Checker> checkerStates, Square[,] boardSquares, bool beatProcess, int currentCheckerId)
        {
            BoardSquares = new Square[8, 8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    BoardSquares[i, j] = new Square(boardSquares[i, j]);
                }
            }

            var newListCheckersPoses = new List<CheckerPositions>();
            foreach (var kvp in checkerStates)
            {
                Checker ch = kvp.Value;
                newListCheckersPoses.Add(new CheckerPositions(ch.Id, ch.Square.Position, ch.IsBeat, ch.IsSuperChecker, ch.Color));
            }

            CheckersDataPos = newListCheckersPoses;
            CurrentCheckerId = currentCheckerId;
            BeatProcessActivity = beatProcess;
        }
    }

    [System.Serializable]
    public class SquareRecord
    {
        public int Id;
        public Square Square;

        public SquareRecord(int id, Square square)
        {
            Id = id;
            Square = new Square(square.Id, square.Position, square.Color);
        }
    }

    [System.Serializable]
    public class CheckerRecord
    {
        public int Id;
        public Checker Checker;

        public CheckerRecord(int id, Checker checker)
        {
            Id = id;
            Checker = new Checker(checker.Id, checker.Square, checker.Color);
        }
    }

    [System.Serializable]
    public class PuzzleRecord
    {
        public int PuzzleId;
        public int PassedMoves;

        public PuzzleRecord(int puzzleId, int passedMoves)
        {
            PuzzleId = puzzleId;
            PassedMoves = passedMoves;
        }
    }

    [System.Serializable]
    public class CheckerPositions
    {
        public int Id;
        public Position Pos;
        public bool IsBeat;
        public bool IsSuperChecker;
        public CheckerColor Color;

        public CheckerPositions()
        {

        }

        public CheckerPositions(int id, Position pos, bool isBeat, bool isSuperChecker, CheckerColor color)
        {
            Id = id;
            Pos = pos;
            IsBeat = isBeat;
            IsSuperChecker = isSuperChecker;
            Color = color;
        }
    }

    [System.Serializable]
    public class RuleRecord
    {
        public string RuleLabel;
        public CheckersRuleType Type;
        public DiagonalMoves DiagonalMoveRule;
        public Beating BeatingRule;
        public SimpleCheckersBackBeating BackBeatingRule;
        public ContinueBeatAfterLastRowAchieve ContinueBeatingLastRowRule;

        public RuleRecord()
        {

        }

        public RuleRecord(CheckersRule rule)
        {
            RuleLabel = rule.Label;
            Type = rule.Type;
            DiagonalMoveRule = rule.Options.DiagonalMoveRule;
            BeatingRule = rule.Options.BeatingRule;
            BackBeatingRule = rule.Options.BackBeatingRule;
            ContinueBeatingLastRowRule = rule.Options.ContinueBeatingLastRowRule;
        }
    }
}