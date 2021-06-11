using System.Collections.Generic;

namespace Checkers
{
    [System.Serializable]
    public class Puzzle
    {
        public int PuzzleId;
        public int OptimalMoves;
        public List<Position> WhitePositions = new List<Position>();
        public List<Position> BlackPositions = new List<Position>();
    }
}