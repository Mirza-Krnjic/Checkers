using System;
using System.Collections.Generic;
using System.Linq;

namespace Checkers
{
    public class AiEasy : AiDifficulty
    {
        public override AiMove GetMove(Dictionary<Checker, List<Square>> boardData, bool isBeatProcess, bool isNeedRepeatMove, int currentCheckerWhichBeat)
        {
            AiMove move = null;

            if (isNeedRepeatMove)
            {
                move = GameController.Instance.CoreInstance.LastAiMoveData;
            }
            else
            {
                Random r = new Random();
                int randCheckerId = r.Next(0, boardData.Count);
                Checker randChecker = boardData.Keys.ElementAt(randCheckerId);
                int randSquare = r.Next(0, boardData[randChecker].Count);

                move = new AiMove(randChecker, boardData[randChecker][randSquare]);
                GameController.Instance.CoreInstance.LastAiMoveData = move;
            }
            return move;
        }
    }
}