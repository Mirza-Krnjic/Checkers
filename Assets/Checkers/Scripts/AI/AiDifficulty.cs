using System.Collections.Generic;

namespace Checkers
{
	public abstract class AiDifficulty
	{
        public abstract AiMove GetMove(Dictionary<Checker, List<Square>> boardData, bool isBeatProcess, bool isNeedRepeatLastMove, int currentCheckerWhichBeatId);
	}
}