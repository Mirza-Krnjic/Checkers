using System.Collections.Generic;

namespace Checkers
{
    [System.Serializable]
    public class PuzzleProgress
    {
        public int LastPuzzle;

        public List<PuzzleResult> Results = new List<PuzzleResult>();

        public PuzzleProgress(int lastPuzzle)
        {
            LastPuzzle = lastPuzzle;
        }
    }

    [System.Serializable]
    public class PuzzleResult
    {
        public int Id;
        public PuzzleStatus Status;

        public PuzzleResult(int id, PuzzleStatus status)
        {
            Id = id;
            Status = status;
        }
    }
}