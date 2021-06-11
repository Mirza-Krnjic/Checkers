using Checkers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Checkers
{
    [Serializable]
    public class Core
    {
        public MoveData LastPlayerMoveData;
        public AiMove LastAiMoveData;

        public CheckersRule GameRule;

        public bool IsAiMove { get; private set; }
        public bool IsUserMove { get; set; }
        public bool IsBeatProcessActive { get; set; }
        public bool IsCheckerBeatOtherCheckers { get; private set; }
        public bool GameEnd { get; private set; }
        public bool IsUserMadeFirstMove { get; private set; }
        public bool IsNeedRepeatMoveByAI { get; private set; }

        [Header("Data arrays: ")]
        public List<Square> AvailableSquaresForWhite = new List<Square>();
        public List<Square> AvailableSquaresForBlack = new List<Square>();
        public List<int> CheckersWhichCanMove = new List<int>();

        private const int _boardSize = 8;
        private const int _idIncrement = 1;

        private const int _checkerCount = 12;

        private Square[,] _boardSquares;

        private List<Square> _allAvailableSquares = new List<Square>();

        private Checker _currentChecker;

        private Dictionary<int, Square> _squaresData = new Dictionary<int, Square>();

        private Dictionary<int, Checker> _checkersData = new Dictionary<int, Checker>();

        private CheckerColor _currentMoveCollor;
        public CheckerColor CurrentMoveColor
        {
            get => _currentMoveCollor;
            set
            {
                _currentMoveCollor = value;
                BoardController.Instance.ChangeCurrentTurnImage();
            }
        }

        public Difficulty CurrentDifficult = Difficulty.Easy;



        public int PassedMoves = 0;

        /// <summary>
        /// Create new Game instance
        /// </summary>
        public Core()
        {
            _boardSquares = new Square[_boardSize, _boardSize];
            _allAvailableSquares = new List<Square>();
            GameEnd = false;
            IsUserMadeFirstMove = false;
            CurrentDifficult = Difficulty.Hard;
        }

        /// <summary>
        /// Setup first turn player.
        /// </summary>
        public void FirstTurnPlayer(CheckerColor color)
        {
            CurrentMoveColor = color;
        }

        public void setDifficultyEasy()
        {
            CurrentDifficult = Difficulty.Easy;
        }
        public void setDifficultyHard()
        {
            CurrentDifficult = Difficulty.Hard;
        }



        /// <summary>
        /// Create board(squares and checkers).
        /// </summary>
        /// <param name="userColor"></param>
        public void PrepareBoard(UserColor userColor, bool isPuzzle = false, Puzzle puzzle = null)
        {
            _squaresData = new Dictionary<int, Square>();
            _checkersData = new Dictionary<int, Checker>();

            if (isPuzzle)
            {
                PreparePuzzle(puzzle);
                InitPuzzle(0);
                int optimalMoves = PuzzleController.Instance.CurrentPuzzle.OptimalMoves;
                bool isHasMovesYet = PassedMoves <= optimalMoves;
                //UiViewController.Instance.SetPuzzlePassedMovesText(PassedMoves.ToString(), optimalMoves.ToString(), isHasMovesYet);
                BoardController.Instance.OnBoardPrepared(_squaresData.Values.ToList(), _checkersData.Values.ToList());
                foreach (var value in _checkersData.Values)
                {
                    CheckForCrownChecker(value.Id);
                }
            }
            else
            {
                //UiViewController.Instance.ClearPuzzlePassedMovesText();
                PrepareMainGame();
                BoardController.Instance.OnBoardPrepared(_squaresData.Values.ToList(), _checkersData.Values.ToList());
            }
            CheckGameEnd();
        }

        /// <summary>
        /// Prepare board for undo.
        /// </summary>
        public void PrepareBoardAction(Square[,] boardSquares, List<CheckerPositions> chPoses)
        {
            BoardController board = BoardController.Instance;

            //if (!PuzzleController.Instance.IsPuzzleGameActive) { UiViewController.Instance.ClearPuzzlePassedMovesText(); }
            PrepareUndoGame(boardSquares, chPoses);

            int whiteBeatCount = _checkersData.Where(x => x.Value.Color == CheckerColor.Black && x.Value.IsBeat).Count() - 1;
            int blackBeatCount = _checkersData.Where(x => x.Value.Color == CheckerColor.White && x.Value.IsBeat).Count() - 1;

            //board.InitCheckersStacks(0, 0);

            board.OnBoardPrepared(_squaresData.Values.ToList(), _checkersData.Values.ToList(), true);

            foreach (var value in _checkersData.Values)
            {
                if (GameController.Instance.IsContinue && value.IsBeat)
                {
                    continue;
                }

                CheckForCrownChecker(value.Id);
            }

            if (GameController.Instance.IsContinue)
            {
                GameController.Instance.IsContinue = false;
            }

            board.ResetCheckersChoosing();

            CheckGameEnd();
        }

        /// <summary>
        /// Prepare board when puzzle game.
        /// </summary>
        /// <param name="puzzle"></param>
        public void PreparePuzzle(Puzzle puzzle)
        {
            if (puzzle == null)
            {
                Debug.LogError("Puzzle equals null!");
                return;
            }

            for (int i = 0; i < _boardSize; i++)
            {
                for (int j = 0; j < _boardSize; j++)
                {
                    var position = new Position(i, j, PosDirection.None);
                    var id = j * _boardSize + i;
                    SquareColor squareColor = (i + j) % 2 == 0 ? (!GameRule.Options.IsReverseBoard) ? SquareColor.Black : SquareColor.White
                                                              : (!GameRule.Options.IsReverseBoard) ? SquareColor.White : SquareColor.Black;

                    Square square = new Square(id, position, squareColor);
                    _boardSquares[i, j] = square;
                    _squaresData[square.Id] = square;
                }
            }

            int index = 0;

            for (int i = 0; i < puzzle.WhitePositions.Count; i++)
            {
                index = (!GameRule.Options.IsReverseBoard ? _boardSize - 1 - puzzle.WhitePositions[i].X : puzzle.WhitePositions[i].X) + puzzle.WhitePositions[i].Y * 8;

                int id = index + _idIncrement;
                Checker checker = new Checker(id, _squaresData[index], CheckerColor.White);
                _checkersData[id] = checker;
                _squaresData[index].SetChecker(checker);
            }

            for (int i = 0; i < puzzle.BlackPositions.Count; i++)
            {
                index = (!GameRule.Options.IsReverseBoard ? _boardSize - 1 - puzzle.BlackPositions[i].X : puzzle.BlackPositions[i].X) + puzzle.BlackPositions[i].Y * 8;

                int id = index + _idIncrement;
                Checker checker = new Checker(id, _squaresData[index], CheckerColor.Black);
                _checkersData[id] = checker;
                _squaresData[index].SetChecker(checker);
            }
        }

        /// <summary>
        /// Prepare board when PVC game.
        /// </summary>
        public void PrepareMainGame()
        {
            for (int i = 0; i < _boardSize; i++)
            {
                for (int j = 0; j < _boardSize; j++)
                {
                    var position = new Position(i, j, PosDirection.None);
                    var id = j * _boardSize + i;

                    SquareColor squareColor = (i + j) % 2 == 0 ? (!GameRule.Options.IsReverseBoard) ? SquareColor.Black : SquareColor.White
                                                               : (!GameRule.Options.IsReverseBoard) ? SquareColor.White : SquareColor.Black;

                    Square square = new Square(id, position, squareColor);
                    _boardSquares[i, j] = square;
                    _squaresData[square.Id] = square;
                }
            }

            int c = 0, index = _boardSize * _boardSize - 1;
            while (c < _checkerCount)
            {
                if (_boardSquares[index / _boardSize, index % _boardSize].Color == SquareColor.Black)
                {
                    int id = index + _idIncrement;
                    Checker checker = new Checker(id, _squaresData[index], CheckerColor.Black);
                    _checkersData[id] = checker;
                    _squaresData[index].SetChecker(checker);
                    c++;
                }
                index--;
            }

            c = 0;
            index = 0;
            while (c < _checkerCount)
            {
                if (_boardSquares[index / _boardSize, index % _boardSize].Color == SquareColor.Black)
                {
                    int id = index + _idIncrement;
                    Checker checker = new Checker(id, _squaresData[index], CheckerColor.White);
                    _checkersData[id] = checker;
                    _squaresData[index].SetChecker(checker);
                    c++;
                }
                index++;

            }
        }

        /// <summary>
        /// Prepare board when undo.
        /// </summary>
        public void PrepareUndoGame(Square[,] boardSquares, List<CheckerPositions> chPoses)
        {
            for (int i = 0; i < _boardSize; i++)
            {
                for (int j = 0; j < _boardSize; j++)
                {
                    if (GameController.Instance.IsContinue)
                    {
                        var position = new Position(i, j, PosDirection.None);
                        var id = j * _boardSize + i;

                        SquareColor squareColor = (i + j) % 2 == 0 ? (!GameRule.Options.IsReverseBoard) ? SquareColor.Black : SquareColor.White
                                                               : (!GameRule.Options.IsReverseBoard) ? SquareColor.White : SquareColor.Black;

                        Square square = new Square(id, position, squareColor);
                        _boardSquares[i, j] = square;
                        _squaresData[square.Id] = square;
                    }
                    else
                    {
                        Square square = boardSquares[i, j];

                        _boardSquares[i, j] = square;
                        _squaresData[square.Id] = square;
                    }
                }
            }

            foreach (var ch in chPoses)
            {
                Square possibleSquare = _squaresData.First(x => x.Value.Position.X == ch.Pos.X && ch.Pos.Y == x.Value.Position.Y).Value;

                Checker checker = new Checker()
                {
                    Id = ch.Id,
                    Color = ch.Color,
                    IsBeat = ch.IsBeat,
                    Square = possibleSquare,
                    IsSuperChecker = ch.IsSuperChecker
                };

                if (!ch.IsBeat)
                {
                    possibleSquare.SetChecker(ch.IsBeat ? null : checker);
                }
                _boardSquares[possibleSquare.Position.X, possibleSquare.Position.Y] = possibleSquare;
                _squaresData[possibleSquare.Id] = possibleSquare;
                _checkersData[ch.Id] = checker;
            }
        }

        /// <summary>
        /// Get squares for move in all directions.
        /// </summary>
        public List<Square> GetAvailableSquaresInAllDirections(Checker checker, CheckerColor colorForCheck)
        {
            bool isShouldBeatChecker = CheckBeatCheckerInAllDirections(checker.Id, checker.IsSuperChecker && GameRule.Options.IsLongMoveAllow, colorForCheck);
            List<Square> availableSquares = new List<Square>();
            List<PosDirection> directions = new List<PosDirection>() { PosDirection.UpRight, PosDirection.UpLeft, PosDirection.BottomRight, PosDirection.BottomLeft };
            foreach (var direction in directions)
            {
                availableSquares.AddRange(GetAvailableSquaresInDirectionByDiagonal(checker,
                                                            direction,
                                                            GameRule.Options.IsLongMoveAllow,
                                                            checker.IsSuperChecker,
                                                            CheckBeatCheckerInDirection(checker.Id, checker.IsSuperChecker && GameRule.Options.IsLongMoveAllow, direction, colorForCheck).Value,
                                                            isShouldBeatChecker,
                                                            GameRule.Options.IsStillSimpleCheckerWhenBeatAfterLastRowAchieved,
                                                            GameRule.Options.IsNotRequiredBeating));
            }

            return availableSquares;
        }

        /// <summary>
        /// Get square for move in direction.
        /// </summary>
        public List<Square> GetAvailableSquaresInDirection(Checker checker, PosDirection dir, CheckerColor colorForCheck)
        {
            bool isShouldBeatChecker = CheckBeatCheckerInAllDirections(checker.Id, checker.IsSuperChecker && GameRule.Options.IsLongMoveAllow, colorForCheck);

            return GetAvailableSquaresInDirectionByDiagonal(checker,
                                                            dir,
                                                            GameRule.Options.IsLongMoveAllow,
                                                            checker.IsSuperChecker,
                                                            CheckBeatCheckerInDirection(checker.Id, checker.IsSuperChecker && GameRule.Options.IsLongMoveAllow, dir, colorForCheck).Value,
                                                            isShouldBeatChecker,
                                                            GameRule.Options.IsStillSimpleCheckerWhenBeatAfterLastRowAchieved,
                                                            GameRule.Options.IsNotRequiredBeating);
        }

        /// <summary>
        /// Get squares for diagonal move in direction.
        /// </summary>
        public List<Square> GetAvailableSquaresInDirectionByDiagonal(Checker checker, PosDirection dir, bool isDiagonalMove, bool isSuperChecker, bool isHasBeatCheckers, bool onlySquaresAfterBeating, bool isCanBeatBackLikeSimpleChecker, bool isNotRequiredBeating)
        {
            List<Square> allAvailableSquares = new List<Square>();
            bool isFoundSquareWithEnemyChecker = false;
            bool isFoundSquareWithPlayerCheker = false;

            foreach (var neighbourSquarePosition in checker.Square.NeighboursInDirection(isSuperChecker && isDiagonalMove, dir))
            {
                if (IsOnBoard(neighbourSquarePosition))
                {
                    if ((((dir == PosDirection.UpRight || dir == PosDirection.UpLeft) && checker.Color == CheckerColor.Black) ||
                                                           ((dir == PosDirection.BottomRight || dir == PosDirection.BottomLeft) && checker.Color == CheckerColor.White)))
                    {
                        if (!checker.IsSuperChecker && ((isCanBeatBackLikeSimpleChecker && !isHasBeatCheckers) || !isCanBeatBackLikeSimpleChecker && !isHasBeatCheckers))
                        {
                            continue;
                        }
                    }

                    Square neighbourSquare = _boardSquares[neighbourSquarePosition.X, neighbourSquarePosition.Y];
                    if (neighbourSquare != null)
                    {
                        if (!isFoundSquareWithPlayerCheker)
                        {
                            isFoundSquareWithPlayerCheker = neighbourSquare.HasChecker()
                            && neighbourSquare.Checker.Color == CurrentMoveColor;
                        }
                        else
                        {
                            break;
                        }

                        if (!isFoundSquareWithEnemyChecker)
                        {
                            isFoundSquareWithEnemyChecker = neighbourSquare.HasChecker()
                                && neighbourSquare.Checker.Color != CurrentMoveColor;
                        }

                        if (isNotRequiredBeating && !IsBeatProcessActive)
                        {
                            if (onlySquaresAfterBeating && isFoundSquareWithEnemyChecker)
                            {
                                Square availableInDir = GetSecondNextSquareInDirection(neighbourSquare, dir);

                                if (availableInDir != null && !availableInDir.HasChecker())
                                {
                                    allAvailableSquares.Add(availableInDir);
                                }
                                else
                                {
                                    break;
                                }
                            }

                            if (!neighbourSquare.HasChecker() && !isFoundSquareWithEnemyChecker)
                            {
                                allAvailableSquares.Add(neighbourSquare);
                            }
                        }
                        else
                        {
                            if (onlySquaresAfterBeating && !isFoundSquareWithEnemyChecker)
                            {
                                continue;
                            }
                            else if (onlySquaresAfterBeating && isFoundSquareWithEnemyChecker)
                            {
                                Square availableInDir = GetSecondNextSquareInDirection(neighbourSquare, dir);

                                if (availableInDir != null && !availableInDir.HasChecker())
                                {
                                    allAvailableSquares.Add(availableInDir);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else if (!onlySquaresAfterBeating && !isHasBeatCheckers && !neighbourSquare.HasChecker() && !isFoundSquareWithEnemyChecker)
                            {
                                allAvailableSquares.Add(neighbourSquare);
                            }
                        }
                    }
                }
            }

            return allAvailableSquares;
        }

        /// <summary>
        /// When checker is chosen we need to find empty squares to move on.
        /// </summary>
        public void FindAvailableSquares(int checkerId, bool isNotFirstFind = false)
        {
            if (GameEnd)
            {
                return;
            }

            if ((IsAiMove && (GameController.Instance.Mode == GameMode.PlayerVsAI || GameController.Instance.Mode == GameMode.Puzzle)))
            {
                ClearAvailableSquares();
            }
            //If user do not beat checkers yet 
            else if (!IsBeatProcessActive && !isNotFirstFind)
            {
                _currentChecker = null;
                ClearAvailableSquares();
            }

            Checker choosenChecker = _checkersData[checkerId];
            List<Checker> beatCheckers = FindBeatCheckers(CurrentMoveColor);

            if (choosenChecker.Color == CurrentMoveColor && GameRule.Options.IsRequiredBeatMaximum && beatCheckers.Count != 0)
            {
                CoreWhenCheckersIsRequiredBeatMaximum(choosenChecker);
            }
            else if (choosenChecker.Color == CurrentMoveColor && GameRule.Options.IsNotRequiredBeating)
            {
                CoreWhenCheckersIsNotRequiredForBeating(choosenChecker);
            }
            else if (choosenChecker.Color == CurrentMoveColor && beatCheckers.Count == 0)
            {
                CoreWhenHaveNoBeatCheckers(choosenChecker);
            }
            else if (choosenChecker.Color == CurrentMoveColor && beatCheckers.Count != 0 && beatCheckers.Contains(choosenChecker))
            {
                CoreWhenCheckerHaveOtherCheckersForBeating(choosenChecker);
            }
            else
            {
                if (!IsCheckerBeatOtherCheckers)
                {
                    BoardController.Instance.ResetCheckersChoosing();
                    BoardController.Instance.ShowAvailableForMoveCheckers();
                }
            }

            if (((GameController.Instance.Mode == GameMode.PlayerVsAI || GameController.Instance.Mode == GameMode.Puzzle) &&
                choosenChecker.Color == CheckerColor.White) || GameController.Instance.Mode == GameMode.PlayerVsPlayer)
            {
                if (_allAvailableSquares.Count == 0)
                {
                    if (!IsUserMove)
                    {
                        BoardController.Instance.ShakeChecker(choosenChecker);
                        BoardController.Instance.ResetCheckersChoosing();
                        BoardController.Instance.ShowAvailableForMoveCheckers();
                    }
                }
                else
                {
                    if (!IsBeatProcessActive)
                    {
                        _currentChecker = choosenChecker;
                        BoardController.Instance.ChooseChecker(_currentChecker.Id);
                    }

                    BoardController.Instance.MarkSquares(_allAvailableSquares);
                }
            }
        }

        /// <summary>
        /// Logic for checker when it have no checkers for beating.
        /// </summary>
        public void CoreWhenHaveNoBeatCheckers(Checker choosenChecker)
        {
            if (choosenChecker.IsSuperChecker && GameRule.Options.IsLongMoveAllow)
            {
                _allAvailableSquares.AddRange(GetAvailableSquaresInAllDirections(choosenChecker, CurrentMoveColor));
                ///UNCOMMENT FOR TRACKING FOUNDED AVAILABLE SQUARES
                //foreach (var item in _allAvailableSquares)
                //{
                //    Debug.LogError(item.Id);
                //}
            }
            else
            {
                foreach (var neighbourSquarePosition in choosenChecker.Square.Neighbours(false, GameRule.Options.IsLongMoveAllow, false))
                {
                    if (IsOnBoard(neighbourSquarePosition))
                    {
                        Square neighbourSquare = _boardSquares[neighbourSquarePosition.X, neighbourSquarePosition.Y];

                        if (!neighbourSquare.HasChecker())
                            _allAvailableSquares.Add(neighbourSquare);
                    }
                }
            }
        }

        /// <summary>
        /// Logic for checker when it has no required beating option.
        /// </summary>
        private void CoreWhenCheckersIsNotRequiredForBeating(Checker choosenChecker)
        {
            _allAvailableSquares.AddRange(GetAvailableSquaresInAllDirections(choosenChecker, CurrentMoveColor));
        }

        /// <summary>
        /// Logic for checker when it can beat other checkers
        /// </summary>
        private void CoreWhenCheckerHaveOtherCheckersForBeating(Checker choosenChecker)
        {
            _allAvailableSquares.AddRange(GetAvailableSquaresInAllDirections(choosenChecker, CurrentMoveColor));
        }

        /// <summary>
        /// Logic for checker when it has required beat maximum checkers.
        /// </summary>
        private void CoreWhenCheckersIsRequiredBeatMaximum(Checker choosenChecker)
        {
            _allAvailableSquares.AddRange(GetAvailableSquaresInAllDirections(choosenChecker, CurrentMoveColor));
            CoroutineHelper.Instance.StartCoroutine(SimulateCheckerMaximumBeatProcess(choosenChecker.Id, _allAvailableSquares));
        }

        /// <summary>
        /// Update lists with checkers by color.
        /// </summary>
        [ContextMenu("FindAvailableSquaresForColor")]
        public void FindAvailableSquaresForColor()
        {
            if (GameEnd)
                return;

            AvailableSquaresForBlack.Clear();
            AvailableSquaresForWhite.Clear();
            CheckersWhichCanMove.Clear();

            foreach (var item in _checkersData.Values)
            {
                if (item.IsBeat)
                {
                    continue;
                }

                List<Square> squares = GetAvailableSquaresInAllDirections(item, item.Color);
                if (squares.Count > 0)
                {
                    CheckersWhichCanMove.Add(item.Id);
                    if (item.Color == CheckerColor.White)
                    {
                        AvailableSquaresForWhite.AddRange(squares);
                    }
                    else
                    {
                        AvailableSquaresForBlack.AddRange(squares);
                    }
                }
            }

            if (AvailableSquaresForWhite.Count == 0)
            {
                int checkersForBeat = FindBeatCheckers(CheckerColor.White).Count;
                if (checkersForBeat == 0)
                {
                    OccurGameEndActions(GameResult.Lose);
                }
            }
            else if (AvailableSquaresForBlack.Count == 0)
            {
                int checkersForBeat = FindBeatCheckers(CheckerColor.Black).Count;
                if (checkersForBeat == 0)
                {
                    OccurGameEndActions(GameResult.Won);

                    if (PuzzleController.Instance.IsPuzzleGameActive)
                    {
                        PuzzleController.Instance.OpenNextPuzzle();
                        PuzzleController.Instance.UpdatePuzzleStatus(PassedMoves, PuzzleController.Instance.CurrentPuzzle.OptimalMoves);
                    }
                }
            }
        }

        /// <summary>
        /// Occur game end actions.(Win, Lose, No moves)
        /// </summary>
        /// <param name="result"></param>
        private void OccurGameEndActions(GameResult result)
        {
            GameController.Instance.ChangeGameResult(result);
            UiViewController.Instance.ShowGameResultAction(result);

            UndoPerformer.Instance.DeleteLastGame();
            UndoPerformer.Instance.ResetUndoStates();

            AudioController.Instance.PlayOneShotAudio(result == GameResult.Won ? AudioController.AudioType.Win : AudioController.AudioType.Lose);

            GameEnd = true;
        }

        /// <summary>
        /// Move checker to square that available.
        /// </summary>
        public void TryToMoveChecker(int squareId, int? checkerId = null)
        {
            Square square = _squaresData[squareId];
            Checker checker = null;
            if (checkerId.HasValue && checkerId.Value != 0)
            {
                checker = _checkersData[checkerId.Value];
            }
            else
            {
                checker = _currentChecker;
            }

            if (_allAvailableSquares.Find(x => x.Id == square.Id) != null)
            {
                //If choose checker is exist
                if (checker != null)
                {
                    if (checker.IsBeat)
                    {
                        checker = null;
                        return;
                    }

                    SetUserMoveState(true);

                    if (!IsUserMadeFirstMove)
                    {
                        IsUserMadeFirstMove = true;
                        UndoPerformer.Instance.ResetUndoStates();
                        UndoPerformer.Instance.DeleteLastGame();
                    }

                    //Undo write
                    if (CurrentMoveColor == CheckerColor.White)
                    {
                        WriteUndoStates();

                        if (IsUserMadeFirstMove)
                        {
                            UndoPerformer.Instance.SaveGame();
                        }
                        UndoPerformer.Instance.ActivateUndoButton();
                    }

                    if (PuzzleController.Instance.IsPuzzleGameActive && CurrentMoveColor == CheckerColor.White)
                    {
                        //ONLY FOR PUZZLES LOGIC
                        PassedMoves++;
                        int optimalMoves = PuzzleController.Instance.CurrentPuzzle.OptimalMoves;
                        bool isHasMovesYet = PassedMoves <= optimalMoves;
                        //UiViewController.Instance.SetPuzzlePassedMovesText(PassedMoves.ToString(), optimalMoves.ToString(), isHasMovesYet);
                    }

                    //If between square where you want to place your checker exist enemy checker
                    Square intermediateSquare = (checker.IsSuperChecker && GameRule.Options.IsLongMoveAllow) ?
                        HasIntermediateSquareWithCheckerForCrownChecker(checker.Square, square) :
                        HasIntermediateSquareWithChecker(checker.Square, square);
                    if (intermediateSquare != null)
                    {
                        int removeId = _squaresData[intermediateSquare.Id].Checker.Id;
                        _checkersData[removeId].WasBeat();
                        BoardController.Instance.OnRemoveChecker(_squaresData[intermediateSquare.Id].Checker);
                        _boardSquares[intermediateSquare.Position.X, intermediateSquare.Position.Y].SetChecker(null);
                        _squaresData[intermediateSquare.Id].SetChecker(null);
                        IsBeatProcessActive = true;
                        IsCheckerBeatOtherCheckers = true;
                        CheckGameEnd();
                    }
                    _boardSquares[checker.Square.Position.X, checker.Square.Position.Y].SetChecker(null);
                    _squaresData[checker.Square.Id].SetChecker(null);
                    checker.MoveTo(square);
                    square.SetChecker(checker);
                    if (CurrentMoveColor == CheckerColor.White)
                    {
                        RememberLastPlayerMove(new MoveData(checker, square));
                    }
                    BoardController.Instance.OnMoveChecker(checker);
                }

                ///need check for undo when cjecker equals null
                bool isAchievedLastRow = IsLastRowAchieved(checker.Id);

                if (!GameRule.Options.IsStillSimpleCheckerWhenBeatAfterLastRowAchieved && GameRule.Options.IsCanBeatAfterLastRowAchieve)
                {
                    CheckForCrownChecker(checker.Id);
                }
                // Move checker to new square.
                bool canBeatYet = false;

                canBeatYet = IsCanCheckerBeatOtherCheckers(checker.Id);
                if (canBeatYet && IsBeatProcessActive)
                {
                    ClearAvailableSquares();
                    FindAvailableSquares(checker.Id, IsCheckerBeatOtherCheckers);
                }

                if (!GameRule.Options.IsCanBeatAfterLastRowAchieve
                    || isAchievedLastRow && GameRule.Options.IsCanBeatAfterLastRowAchieve && !canBeatYet && !IsBeatProcessActive && !GameRule.Options.IsStillSimpleCheckerWhenBeatAfterLastRowAchieved
                    || isAchievedLastRow && GameRule.Options.IsCanBeatAfterLastRowAchieve && !canBeatYet && IsBeatProcessActive && GameRule.Options.IsStillSimpleCheckerWhenBeatAfterLastRowAchieved
                    || isAchievedLastRow && GameRule.Options.IsCanBeatAfterLastRowAchieve && IsBeatProcessActive && !GameRule.Options.IsStillSimpleCheckerWhenBeatAfterLastRowAchieved
                    || isAchievedLastRow && GameRule.Options.IsCanBeatAfterLastRowAchieve && !IsBeatProcessActive)
                {
                    CheckForCrownChecker(checker.Id);
                }

                if (!IsBeatProcessActive || !canBeatYet || isAchievedLastRow && !GameRule.Options.IsCanBeatAfterLastRowAchieve || IsBeatProcessActive && _currentChecker.IsSuperChecker && GameRule.Options.IsShouldStopAfterDiagonalMove)
                {
                    _currentChecker = null;
                    // Swich move to another player.
                    BoardController.Instance.ResetCheckersChoosing();

                    ClearAvailableSquares();
                    ChangeMoveColor();
                    IsCheckerBeatOtherCheckers = false;
                    IsBeatProcessActive = false;
                }

                FindAvailableSquaresForColor();
                BoardController.Instance.ShowAvailableForMoveCheckers();
            }
        }

        /// <summary>
        /// Is Checkers on board now.
        /// </summary>
        private bool IsOnBoard(Position position)
        {
            return position.X >= 0 && position.X < _boardSize && position.Y >= 0 && position.Y < _boardSize;
        }

        /// <summary>
        /// Change color of current player that move. So if White checkers has moved on the next movement for black checkers
        /// </summary>
        private void ChangeMoveColor()
        {
            CurrentMoveColor = (CurrentMoveColor == CheckerColor.White) ? CheckerColor.Black : CheckerColor.White;
        }

        /// <summary>
        /// Clear all available squares.
        /// </summary>
        public void ClearAvailableSquares()
        {
            BoardController.Instance.UnmarkSquares(_allAvailableSquares);
            _allAvailableSquares.Clear();
        }

        /// <summary>
        /// Find checkers for beat.
        /// </summary>
        public List<Checker> FindBeatCheckers(CheckerColor colorForFindBeatChecker)
        {
            List<Checker> beatCheckers = new List<Checker>();
            foreach (var checker in _checkersData.Values)
            {
                if (checker.Color == colorForFindBeatChecker && !checker.IsBeat)
                {
                    if (CheckBeatCheckerInAllDirections(checker.Id, checker.IsSuperChecker && GameRule.Options.IsLongMoveAllow, colorForFindBeatChecker))
                    {
                        beatCheckers.Add(checker);
                    }
                }
            }
            return beatCheckers;
        }


        /// <summary>
        /// Find checkers which can  beat other.
        /// </summary>
        public List<int> FindBeatCheckersIds(CheckerColor colorForFindBeatChecker)
        {
            List<int> beatCheckers = new List<int>();
            foreach (var checker in _checkersData.Values)
            {
                if (checker.Color == colorForFindBeatChecker && !checker.IsBeat)
                {
                    if (CheckBeatCheckerInAllDirections(checker.Id, checker.IsSuperChecker && GameRule.Options.IsLongMoveAllow, colorForFindBeatChecker))
                    {
                        beatCheckers.Add(checker.Id);
                    }
                }
            }
            return beatCheckers;
        }

        /// <summary>
        /// Is can checker beat other in direction
        /// </summary>
        private CheckerBooleanResult CheckBeatCheckerInDirection(int checkerId, bool diagonal, PosDirection direction, CheckerColor colorForCheck)
        {
            Checker checker = _checkersData[checkerId];

            if (checker.Color == colorForCheck)
            {
                foreach (var neighbourSquarePosition in checker.Square.NeighboursInDirection(diagonal, direction))
                {
                    if (IsOnBoard(neighbourSquarePosition))
                    {
                        Square neighbourSquare = _boardSquares[neighbourSquarePosition.X, neighbourSquarePosition.Y];
                        if (neighbourSquare.HasChecker() && neighbourSquare.Checker.Color != colorForCheck)
                        {
                            Square secondNextSquare = GetSecondNextSquareInDirection(neighbourSquare, direction);
                            if (secondNextSquare != null && !secondNextSquare.HasChecker())
                            {
                                if (!checker.IsSuperChecker && !GameRule.Options.IsCheckersCanBeatBack)
                                {
                                    if (IsLastRowAchieved(checkerId) && GameRule.Options.IsStillSimpleCheckerWhenBeatAfterLastRowAchieved)
                                    {
                                        if (checker.Color == CheckerColor.White)
                                        {
                                            if (checker.Square.Position.Y == _boardSize - 1)
                                            {
                                                return new CheckerBooleanResult() { Value = true, CapturedChecker = neighbourSquare.Checker };
                                            }
                                        }
                                        else
                                        {
                                            if (checker.Square.Position.Y == 0)
                                            {
                                                return new CheckerBooleanResult() { Value = true, CapturedChecker = neighbourSquare.Checker };
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (colorForCheck == CheckerColor.White)
                                        {
                                            if (neighbourSquare.Position.Y > checker.Square.Position.Y)
                                            {
                                                return new CheckerBooleanResult() { Value = true, CapturedChecker = neighbourSquare.Checker };
                                            }
                                        }
                                        else
                                        {
                                            if (neighbourSquare.Position.Y < checker.Square.Position.Y)
                                            {
                                                return new CheckerBooleanResult() { Value = true, CapturedChecker = neighbourSquare.Checker };
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    return new CheckerBooleanResult() { Value = true, CapturedChecker = neighbourSquare.Checker };
                                }
                            }
                            else
                            {
                                return new CheckerBooleanResult() { Value = false, CapturedChecker = null };
                            }
                        }
                        else if (neighbourSquare.HasChecker() && neighbourSquare.Checker.Color == colorForCheck)
                        {
                            return new CheckerBooleanResult() { Value = false, CapturedChecker = null };
                        }
                    }
                }
            }
            return new CheckerBooleanResult() { Value = false, CapturedChecker = null };
        }

        /// <summary>
        /// IS can checker beat other checker in all directions.
        /// </summary>
        private bool CheckBeatCheckerInAllDirections(int checkerId, bool diagonal, CheckerColor colorForCheck)
        {
            Checker checker = _checkersData[checkerId];

            if (checker.Color == colorForCheck)
            {
                List<PosDirection> directions = new List<PosDirection>() { PosDirection.UpRight, PosDirection.UpLeft, PosDirection.BottomRight, PosDirection.BottomLeft };
                foreach (var direction in directions)
                {
                    bool result = CheckBeatCheckerInDirection(checker.Id, diagonal, direction, colorForCheck).Value;
                    if (result)
                    {
                        return result;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Will be checkers captured in all directions.
        /// </summary>
        private List<Checker> GetCapturedCheckerInAllDirections(CheckerColor colorForCheck)
        {
            List<Checker> capturedCheckers = new List<Checker>();
            List<PosDirection> directions = new List<PosDirection>() { PosDirection.UpRight, PosDirection.UpLeft, PosDirection.BottomRight, PosDirection.BottomLeft };

            foreach (var checker in _checkersData.Values)
            {
                if (_currentChecker == null || checker == _currentChecker)
                {
                    if (checker.Color == colorForCheck && !checker.IsBeat)
                    {
                        foreach (var direction in directions)
                        {
                            CheckerBooleanResult result = CheckBeatCheckerInDirection(checker.Id, checker.IsSuperChecker && GameRule.Options.IsLongMoveAllow, direction, colorForCheck);
                            if (result.Value)
                            {
                                capturedCheckers.Add(result.CapturedChecker);
                            }
                        }
                    }
                }
            }

            return capturedCheckers;
        }

        /// <summary>
        /// If checker can beat others checkers.
        /// </summary>
        private bool IsCanCheckerBeatOtherCheckers(int checkerId)
        {
            Checker checker = _checkersData[checkerId];

            return CheckBeatCheckerInAllDirections(checkerId, checker.IsSuperChecker && GameRule.Options.IsLongMoveAllow, checker.Color);
        }

        /// <summary>
        /// Is next square empty?
        /// </summary>
        public bool IsSecondNextSquareEmpty(Square checkerSquare, Square neighbourSquare)
        {
            Square secondNextSquare = GetSecondNextSquare(checkerSquare, neighbourSquare);
            return (secondNextSquare != null && !secondNextSquare.HasChecker());
        }

        /// <summary>
        /// Get next square for move through the square.
        /// </summary>
        private Square GetSecondNextSquare(Square currSquare, Square nextSquare)
        {
            Position secondNextPosition = new Position(currSquare.Position - 2 * (currSquare.Position - nextSquare.Position));
            return IsOnBoard(secondNextPosition) ? _boardSquares[secondNextPosition.X, secondNextPosition.Y] : null;
        }

        /// <summary>
        /// Get second square in direction.
        /// </summary>
        private Square GetSecondNextSquareInDirection(Square currSquare, PosDirection dir)
        {
            Position secondNextPosition = currSquare.NeighbourInDirection(dir);
            return IsOnBoard(secondNextPosition) ? _boardSquares[secondNextPosition.X, secondNextPosition.Y] : null;
        }

        /// <summary>
        /// Is has square between currSquare and nextSquare.
        /// </summary>
        private Square HasIntermediateSquareWithChecker(Square currSquare, Square nextSquare)
        {
            Position pos = nextSquare.Position - currSquare.Position;
            if (pos.Abs() != new Position(1, 1, PosDirection.None))
            {
                Position intermediatePosition = (currSquare.Position + nextSquare.Position) / 2;
                return _boardSquares[intermediatePosition.X, intermediatePosition.Y];
            }
            return null;
        }

        /// <summary>
        /// Is has checker on the way of crown checker
        /// </summary>
        private Square HasIntermediateSquareWithCheckerForCrownChecker(Square currSquare, Square nextSquare)
        {
            int yIncrement = currSquare.Position.Y > nextSquare.Position.Y ? -1 : 1;
            int xIncrement = currSquare.Position.X > nextSquare.Position.X ? -1 : 1;

            for (int x = currSquare.Position.X + xIncrement, y = currSquare.Position.Y + yIncrement;
                     x != nextSquare.Position.X + xIncrement && y != nextSquare.Position.Y + yIncrement; x += xIncrement, y += yIncrement)
            {
                if (_boardSquares[x, y].HasChecker())
                {
                    return _boardSquares[x, y];
                }
            }

            return null;
        }

        /// <summary>
        /// Check for changing to crown checker.
        /// </summary>
        private void CheckForCrownChecker(int id)
        {
            Checker checkerForCheck = _checkersData[id];
            if (checkerForCheck != null)
            {
                if (!checkerForCheck.IsSuperChecker)
                {
                    if ((checkerForCheck.Color == CheckerColor.Black && checkerForCheck.Square.Position.Y == 0) ||
                        (checkerForCheck.Color == CheckerColor.White && checkerForCheck.Square.Position.Y == 7))
                    {
                        AudioController.Instance.PlayOneShotAudio(AudioController.AudioType.SuperChecker);
                        checkerForCheck.BecomeCrownChecker();
                        BoardController.Instance.SetCrownChecker(checkerForCheck);
                    }
                }
                else
                {
                    checkerForCheck.BecomeCrownChecker();
                    BoardController.Instance.SetCrownChecker(checkerForCheck);
                }
            }
        }

        /// <summary>
        /// Check if checker achieved last row first time.
        /// </summary>
        public bool IsLastRowAchieved(int id)
        {
            bool lastRowAchieved = false;

            Checker checkerForCheck = _checkersData[id];
            if (checkerForCheck != null)
            {
                if (!checkerForCheck.IsSuperChecker)
                {
                    if ((checkerForCheck.Color == CheckerColor.Black && checkerForCheck.Square.Position.Y == 0) ||
                        (checkerForCheck.Color == CheckerColor.White && checkerForCheck.Square.Position.Y == 7))
                    {
                        lastRowAchieved = true;
                    }
                }
            }

            return lastRowAchieved;
        }

        /// <summary>
        /// Check for game ending.
        /// </summary>
        private void CheckGameEnd()
        {
            List<Checker> whiteCheckers = new List<Checker>();
            List<Checker> blackCheckers = new List<Checker>();

            foreach (var checker in _checkersData.Values)
            {
                if (checker.Color == CheckerColor.White && !checker.IsBeat)
                    whiteCheckers.Add(checker);
                else if (checker.Color == CheckerColor.Black && !checker.IsBeat)
                    blackCheckers.Add(checker);
            }

            UiViewController.Instance.UpdateGameScore(whiteCheckers.Count, blackCheckers.Count);

            if (whiteCheckers.Count == 0)
            {
                AdsController.Instance.ShowRewardBasedVideo();
                OccurGameEndActions(GameResult.Lose);
                return;
            }
            else if (blackCheckers.Count == 0)
            {
                OccurGameEndActions(GameResult.Won);

                if (PuzzleController.Instance.IsPuzzleGameActive)
                {
                    PuzzleController.Instance.OpenNextPuzzle();
                    PuzzleController.Instance.UpdatePuzzleStatus(PassedMoves, PuzzleController.Instance.CurrentPuzzle.OptimalMoves);
                }
                return;
            }
        }

        /// <summary>
        /// Get square instance by id.
        /// </summary>
        public Square GetSquare(int squareID)
        {
            return (_squaresData.ContainsKey(squareID)) ? _squaresData[squareID] : null;
        }

        /// <summary>
        /// Get square instance by id.
        /// </summary>
        public Square GetSquareByChecker(int id)
        {
            return _squaresData.Where(x => x.Value.Checker.Id == id) as Square;
        }


        /// <summary>
        /// Get checker by id.
        /// </summary>
        public Checker GetChecker(int checkerID)
        {
            return (_checkersData.ContainsKey(checkerID)) ? _checkersData[checkerID] : null;
        }

        /// <summary>
        /// Bot move action in coroutine.
        /// </summary>
        public IEnumerator BotMove()
        {
            if (GameEnd) yield break;
            IsAiMove = true;
            UndoPerformer.Instance.ActivateUndoButton();
            yield return new WaitForSeconds(DataConfig.Instance.AIMoveTime);

            AiDifficulty botDifficulty = null;
            switch (CurrentDifficult)
            {
                case Difficulty.Easy:
                    botDifficulty = new AiEasy();
                    break;
                case Difficulty.Hard:
                    botDifficulty = new AiHard();
                    break;
            }

            AudioController.Instance.PlayOneShotAudio(AudioController.AudioType.Move);
            if (CurrentMoveColor == CheckerColor.Black)
            {
                Dictionary<Checker, List<Square>> availableCheckers = new Dictionary<Checker, List<Square>>();
                //find all available AI checkers.
                foreach (var checker in _checkersData.Values)
                {
                    if (checker.Color == CheckerColor.Black && !checker.IsBeat)
                    {
                        FindAvailableSquares(checker.Id);

                        if (_allAvailableSquares.Count != 0)
                            availableCheckers.Add(checker, new List<Square>(_allAvailableSquares));
                    }
                }

                if (availableCheckers.Count != 0)
                {
                    AiMove move = botDifficulty.GetMove(availableCheckers, FindBeatCheckers(CheckerColor.Black).Count > 0, IsNeedRepeatMoveByAI && LastAiMoveData != null, _currentChecker != null ? _currentChecker.Id : 0);
                    if (move != null)
                    {
                        if (IsNeedRepeatMoveByAI) { IsNeedRepeatMoveByAI = false; }
                        _currentChecker = move.Checker;
                        FindAvailableSquares(move.Checker.Id);
                        TryToMoveChecker(move.Square.Id, move.Checker.Id);
                    }
                    else
                    {
                        Debug.LogError("Has no move! for difficulty");
                    }
                }
            }
            IsAiMove = false;

            UndoPerformer.Instance.ActivateUndoButton();
            yield break;
        }

        /// <summary>
        /// Second player move action in coroutine.
        /// </summary>
        public IEnumerator SecondPlayerMove()
        {
            if (GameEnd) yield break;
            IsAiMove = true;
            var moveColor = CurrentMoveColor;
            IsBeatProcessActive = false;
            yield return new WaitUntil(() => moveColor != CurrentMoveColor);

            IsBeatProcessActive = false;
            IsAiMove = false;
            yield break;
        }

        /// <summary>
        /// Get checker from square by square coordinates.
        /// </summary>
        public Checker GetCheckerInSquareByPos(int x, int y)
        {
            Square square = GetSquareByPos(x, y);

            return square != null ? square.Checker : null;
        }

        /// <summary>
        /// Get square by coordinates
        /// </summary>
        public Square GetSquareByPos(int x, int y)
        {
            Square square = null;
            int i = 0;
            foreach (var s in _boardSquares)
            {
                i++;
                if (s.Position.X == x && s.Position.Y == y)
                {
                    square = s;
                    break;
                }
            }

            return square ?? null;
        }

        /// <summary>
        /// Check if checker will be beat in square.
        /// </summary>
        public bool CheckCheckerForBeatInSquare(Checker checkerForCheck, int squareId)
        {
            bool result = false;
            Checker checker = checkerForCheck;
            Square lastCheckerSquare = checker.Square;

            Square squareSimulation = _squaresData[squareId];
            Checker squareSimulationLastChecker = squareSimulation.Checker;
            checker.MoveTo(squareSimulation);
            lastCheckerSquare.SetChecker(null);
            squareSimulation.SetChecker(checker);

            List<Checker> capturedCheckers = GetCapturedCheckerInAllDirections(CheckerColor.White);

            result = capturedCheckers.Contains(checker);

            squareSimulation.SetChecker(squareSimulationLastChecker);
            checker.MoveTo(lastCheckerSquare);
            checker.Square = lastCheckerSquare;
            lastCheckerSquare.SetChecker(checker);
            return result;
        }

        /// <summary>
        /// Write board state for undo logic.
        /// </summary>
        public void WriteUndoStates()
        {
            GameController gameCtrl = GameController.Instance;
            Core core = gameCtrl.CoreInstance;
            if (!core.IsUserMadeFirstMove && UndoPerformer.Instance.StatesData.States.Count == 0)
            {
                return;
            }
            UndoPerformer.Instance.AddUndoState(_squaresData as Dictionary<int, Square>, _checkersData as Dictionary<int, Checker>, _boardSquares);
        }

        /// <summary>
        /// Remember last move which player has done.
        /// </summary>
        /// <param name="data"></param>
        public void RememberLastPlayerMove(MoveData data)
        {
            ///Check undo logic when moving some times
            if (data.Equals(LastPlayerMoveData))
            {
                IsNeedRepeatMoveByAI = true;
            }

            LastPlayerMoveData = data;
        }

        /// <summary>
        /// Init puzzle moves text on start.
        /// </summary>
        public void InitPuzzle(int passedMovesOnStart)
        {
            PassedMoves = passedMovesOnStart;
        }

        /// <summary>
        /// Init passed moves text and show on UI.
        /// </summary>
        public void InitPassedMoveText()
        {
            //int optimalMoves = PuzzleController.Instance.CurrentPuzzle.OptimalMoves;
            //bool isHasMovesYet = PassedMoves <= optimalMoves;
            //UiViewController.Instance.SetPuzzlePassedMovesText(PassedMoves.ToString(), optimalMoves.ToString(), isHasMovesYet);
        }

        /// <summary>
        /// Set user move activity.
        /// </summary>
        public void SetUserMoveState(bool state)
        {
            IsUserMove = state;
        }

        /// <summary>
        /// Set user move activity.
        /// </summary>
        public void SetCurrentChecker(int checkerId)
        {
            Checker curChecker = checkerId == 0 ? null : _checkersData[checkerId];

            if (curChecker != null)
            {
                _currentChecker = curChecker;
            }
        }

        /// <summary>
        /// Get current checker
        /// </summary>
        public Checker GetCurrentChecker()
        {
            return _currentChecker;
        }

        /// <summary>
        /// Get square by coordinates.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Square GetSquareByPos(Position pos)
        {
            Square square = null;

            if (IsOnBoard(pos))
            {
                square = _squaresData.First(x => x.Value.Position.X == pos.X && x.Value.Position.Y == pos.Y).Value;
            }

            return square;
        }

        /// <summary>
        /// Simulation.
        /// </summary>
        public IEnumerator SimulateCheckerMaximumBeatProcess(int checkerId, List<Square> availableSquares)
        {
            bool result = false;
            Checker checker = _checkersData[checkerId];
            Square initCheckerSquare = checker.Square;

            Square lastCheckerSquare = null;
            Square squareSimulation = null;
            List<BeatPoses> possibleMovePoses = new List<BeatPoses>();
            BeatPoses poses = new BeatPoses();
            int simulationId = 0;
            int numberOfMoveInSequence = 0;

            List<Square> possibleSquaresForBeatFromStart = new List<Square>(availableSquares);
            poses.AddSimulations(GetSimulationPoses(ref poses, ref simulationId, ref numberOfMoveInSequence, checker, availableSquares));
            yield return new WaitForSeconds(1f);
            poses.SimulatedPoses.Reverse();
            poses.SimulatedPoses = poses.SimulatedPoses.OrderBy(x => x.Id).ToList();

            /****** Test actions. DO NOT USE!! ******
             
            foreach (var square in possibleSquaresForBeatFromStart)
            {
                simulationId++;
                do
                {
                    lastCheckerSquare = checker.Square;

                    squareSimulation = _squaresData[square.Id];

                    Square intermediateSquare = (checker.IsSuperChecker && GameRule.Options.IsLongMoveAllow) ?
                           HasIntermediateSquareForSuperChecker(checker.Square, squareSimulation) :
                           HasIntermediateSquare(checker.Square, squareSimulation);

                    checker.MoveTo(squareSimulation);
                    lastCheckerSquare.SetChecker(null);
                    squareSimulation.SetChecker(checker);
                    poses.AddSimulations(GetSimulationPoses(ref simulationId, checker, GetAvailableSquaresInAllDirections(checker, CurrentMoveColor)));

                }
                while (IsCanCheckerBeatOtherCheckers(checker.Id));
                possibleMovePoses.Add(poses);
            }

            squareSimulation.SetChecker(null);

            ****** Test actions. DO NOT USE!! ******/

            checker.MoveTo(initCheckerSquare);
            checker.Square = initCheckerSquare;
            initCheckerSquare.SetChecker(checker);
        }

        /****** Test actions. DO NOT USE!! ******

        //public List<BeatSimulation> GetSimulationPoses(ref BeatPoses poses, ref int simulationId, Checker checker, List<Square> availableSquares)
        //{
        //    bool firstSimulation = true;
        //    Position intiialCheckerPos = checker.Square.Position;
        //    Square lastCheckerSquare = null;
        //    Square squareSimulation = null;
        //    List<BeatSimulation> simuls = new List<BeatSimulation>();
        //    List<Square> possibleSquaresForBeatFromStart = new List<Square>(availableSquares);

        //    foreach (var square in possibleSquaresForBeatFromStart)
        //    {
        //        bool condition = false;

        //        do
        //        {
        //            lastCheckerSquare = checker.Square;
        //            squareSimulation = _squaresData[square.Id];
        //            checker.MoveTo(squareSimulation);
        //            condition = IsCanCheckerBeatOtherCheckers(checker.Id);
        //            Debug.LogError($"Square ID: {square.Id} condition {condition} checker square {checker.Square.Id}");


        //            if()
        //            Square intermediateSquare = (checker.IsSuperChecker && GameRule.Options.IsLongMoveAllow) ?
        //                   HasIntermediateSquareForSuperChecker(checker.Square, squareSimulation) :
        //                   HasIntermediateSquare(checker.Square, squareSimulation);
        //            checker.MoveTo(squareSimulation);
        //            lastCheckerSquare.SetChecker(null);
        //            squareSimulation.SetChecker(checker);
        //            List<Square> availableSqrs = GetAvailableSquaresInAllDirections(checker, CurrentMoveColor);

        //            string sqrs = string.Empty;

        //            foreach (var item in availableSqrs)
        //            {
        //                sqrs += $"Square id: {item.Id} Poses X:{item.Position.X} Y:{item.Position.Y}\n";
        //            }

        //            Debug.LogWarning(sqrs);

        //            if (intermediateSquare != null)
        //            {
        //                simuls.Add(new BeatSimulation() { Id = simulationId, CapturedChecker = intermediateSquare.Checker, Pos = squareSimulation.Position });
        //                poses.AddSimulations(GetSimulationPoses(ref poses, ref simulationId, checker, availableSqrs));
        //            }
        //            if (firstSimulation)
        //            {
        //                firstSimulation = false;
        //            }
        //            else
        //            {
        //                simulationId++;

        //                List<BeatSimulation> prevMoveSimulations = poses.GetAlreadyExistingSimulationsById(simulationId, intiialCheckerPos);
        //                poses.AddSimulations(prevMoveSimulations);
        //            }
        //            condition = false;
        //        } while (condition);
        //    }
        //    return simuls;
        //}
        ****** Test actions. DO NOT USE!! ******/

        public List<BeatSimulation> GetSimulationPoses(ref BeatPoses poses, ref int simulationId, ref int numberOfMoveInSequence, Checker checker, List<Square> availableSquares)
        {
            bool firstSimulation = true;
            Position initialCheckerPos = checker.Square.Position;
            Position startCheckerPos = checker.Square.Position;
            Square lastCheckerSquare = null;
            Square squareSimulation = null;
            List<BeatSimulation> simuls = new List<BeatSimulation>();
            List<Square> possibleSquaresForBeatFromStart = new List<Square>(availableSquares);


            for (int i = 0; i < possibleSquaresForBeatFromStart.Count; i++)
            {
                Square square = possibleSquaresForBeatFromStart[i];

                bool condition = false;

                numberOfMoveInSequence = 0;
                lastCheckerSquare = GetSquareByPos(startCheckerPos);
                squareSimulation = _squaresData[square.Id];
                Debug.LogError($"squareSimulation ID: {squareSimulation.Id} ");
                checker.MoveTo(squareSimulation);

                if (i != 0)
                {
                    simulationId++;
                }

                do
                {
                    condition = IsCanCheckerBeatOtherCheckers(checker.Id);
                    Square intermediateSquare = (checker.IsSuperChecker && GameRule.Options.IsLongMoveAllow) ?
                           HasIntermediateSquareWithCheckerForCrownChecker(lastCheckerSquare, checker.Square) :
                           HasIntermediateSquareWithChecker(lastCheckerSquare, checker.Square);
                    int intermedCheckerID = intermediateSquare.HasChecker() ? intermediateSquare.Checker.Id : 0;



                    List<Square> availableSqrs = GetAvailableSquaresInAllDirections(checker, CurrentMoveColor);
                    Debug.LogWarning(intermedCheckerID);

                    if (intermediateSquare != null)
                    {
                        numberOfMoveInSequence++;

                        if (intermedCheckerID != 0) Debug.LogError($"Prev: {lastCheckerSquare.Id} Square ID: {square.Id} condition {condition} checker square {checker.Square.Id} intermediate square {intermedCheckerID}");
                        else Debug.LogWarning(null);
                        simuls.Add(new BeatSimulation() { Id = simulationId, CapturedCheckerId = intermedCheckerID, Pos = checker.Square.Position, MoveNumber = numberOfMoveInSequence });
                        poses.AddSimulations(condition ? GetSimulationPoses(ref poses, ref simulationId, ref numberOfMoveInSequence, checker, availableSqrs) : simuls);
                        if (!condition)
                        {
                            break;
                        }
                    }

                    if (i != 0 && simulationId != 0)
                    {
                        //List<BeatSimulation> prevMoveSimulations = poses.GetAlreadyExistingSimulationsById(ref simulationId, initialCheckerPos);
                        //poses.AddSimulations(prevMoveSimulations);
                    }
                    condition = IsCanCheckerBeatOtherCheckers(checker.Id);
                }
                while (condition);
                lastCheckerSquare.SetChecker(null);
                squareSimulation.SetChecker(null);
            }

            foreach (var item in simuls)
            {
                Debug.LogWarning(item.ToString());
            }

            return simuls;
        }
    }
}

public class BeatSimulation
{
    public int Id;
    public int MoveNumber;
    public Position Pos;
    public int CapturedCheckerId;

    public override string ToString()
    {
        return $"Simulation id: {Id} Poses X:{Pos.X} Y:{Pos.Y} Move number: {Id}  Captured checker id: {(CapturedCheckerId)} \n"; ;
    }
}

public class BeatPoses
{
    public List<BeatSimulation> SimulatedPoses = new List<BeatSimulation>();

    public List<BeatSimulation> GetAlreadyExistingSimulationsById(ref int simulationId, Position pos)
    {
        List<BeatSimulation> simulations = new List<BeatSimulation>(SimulatedPoses);
        List<BeatSimulation> existingSimulations = new List<BeatSimulation>(SimulatedPoses);
        simulationId++;

        simulations.Reverse();
        bool foundStartPos = false;
        foreach (var sim in simulations)
        {
            if (pos == sim.Pos)
            {
                foundStartPos = true;
            }

            if (foundStartPos)
            {
                existingSimulations.Add(new BeatSimulation() { Id = simulationId, CapturedCheckerId = sim.CapturedCheckerId, Pos = sim.Pos, MoveNumber = sim.MoveNumber });
            }
        }

        existingSimulations.Reverse();
        return existingSimulations;
    }

    public void AddSimulation(int simulationId, Position pos, int checkerID)
    {
        SimulatedPoses.Add(new BeatSimulation() { Id = simulationId, Pos = pos, CapturedCheckerId = checkerID });
    }

    public void AddSimulations(List<BeatSimulation> simulations)
    {
        foreach (var simulation in simulations)
        {
            SimulatedPoses.Add(new BeatSimulation() { Id = simulation.Id, Pos = simulation.Pos, CapturedCheckerId = simulation.CapturedCheckerId });
        }
    }

    public override string ToString()
    {
        string poses = string.Empty;

        foreach (var item in SimulatedPoses)
        {
            poses += $"Simulation id: {item.Id} Poses X:{item.Pos.X} Y:{item.Pos.Y} Move number: {item.Id}  Captured checker id: {(item.CapturedCheckerId)} \n";
        }

        poses += $"Poses count: {SimulatedPoses.Count}";
        return poses;
    }
}