using System.Collections.Generic;

namespace Checkers
{
    [System.Serializable]
    public class PuzzleFormat
    {
        public int PuzzleId;
        public int OptimalMoves;
        public List<string> WhitePositions = new List<string>();
        public List<string> BlackPositions = new List<string>();
    }
}