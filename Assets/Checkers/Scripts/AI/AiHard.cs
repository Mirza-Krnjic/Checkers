using Checkers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Checkers
{
    public class AiHard : AiDifficulty
    {
        public override AiMove GetMove(Dictionary<Checker, List<Square>> boardData, bool isBeatProcess, bool isNeedRepeatMove, int currentCheckerWhichBeatId)
        {
            AiMove move = null;
            List<AiMove> possibleMoves = new List<AiMove>();

            if (!isBeatProcess)
            {
                foreach (var item in boardData)
                {
                    if (item.Key.IsBeat)
                    {
                        continue;
                    }
                    foreach (var square in item.Value)
                    {
                        if (!GameController.Instance.CoreInstance.CheckCheckerForBeatInSquare(item.Key, square.Id))
                        {
                            possibleMoves.Add(new AiMove(item.Key, square));
                        }
                    }
                }
            }

            if (isNeedRepeatMove)
            {
                move = GameController.Instance.CoreInstance.LastAiMoveData;
            }
            else
            {
                if (possibleMoves.Count > 0 && !isBeatProcess)
                {
                    System.Random r = new System.Random();
                    int randMove = r.Next(0, possibleMoves.Count);
                    move = possibleMoves[randMove];
                }
                else
                {
                    System.Random r = new System.Random();
                    int randCheckerId = r.Next(0, boardData.Count);
                    Checker randChecker = boardData.Keys.ElementAt(randCheckerId);

                    if (currentCheckerWhichBeatId != 0)
                    {
                        while (randChecker.Id != currentCheckerWhichBeatId)
                        {
                            randCheckerId = r.Next(0, boardData.Count);
                            randChecker = boardData.Keys.ElementAt(randCheckerId);
                        }
                    }

                    int randSquare = r.Next(0, boardData[randChecker].Count);
                    move = new AiMove(randChecker, boardData[randChecker][randSquare]);
                }
                Core core = GameController.Instance.CoreInstance;
                if (!core.IsCheckerBeatOtherCheckers)
                {
                    core.LastAiMoveData = move;
                }
            }

            return move;
        }
    }
}

[System.Serializable]
public class MoveData
{
    public Checker Checker;
    public Square MiddleSquare;

    public MoveData(Checker checker, Square middleSquare)
    {
        Checker = checker;
        MiddleSquare = middleSquare;
    }

    public override bool Equals(object obj)
    {
        MoveData comparer = obj as MoveData;
        bool result = comparer != null && Checker.Id == comparer.Checker.Id && MiddleSquare.Id == comparer.MiddleSquare.Id;
        return result;
    }
}