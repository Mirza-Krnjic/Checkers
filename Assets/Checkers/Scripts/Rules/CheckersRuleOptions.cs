using UnityEngine;

namespace Checkers
{
    [System.Serializable]
    public class CheckersRuleOptions
    {
        [Header("Board options:")]
        public bool IsReverseBoard;

        [Header("Game options:")]
        public DiagonalMoves DiagonalMoveRule;
        public Beating BeatingRule;
        public SimpleCheckersBackBeating BackBeatingRule;
        public ContinueBeatAfterLastRowAchieve ContinueBeatingLastRowRule;

        public bool IsLongMoveAllow
        {
            get
            {
                return DiagonalMoveRule == DiagonalMoves.DiagonalMoves
                    || DiagonalMoveRule == DiagonalMoves.DiagonalMovesStopAfterBeating;
            }
        }

        /// <summary>
        /// Could simple checker beat backward additionaly.
        /// </summary>
        public bool IsCheckersCanBeatBack
        {
            get
            {
                return BackBeatingRule == SimpleCheckersBackBeating.CanBeatBack;
            }
        }

        public bool IsNotRequiredBeating
        {
            get
            {
                return BeatingRule == Beating.NotRequired;
            }
        }

        public bool IsRequiredBeatMaximum
        {
            get
            {
                return BeatingRule == Beating.RequiredBeatMaximum;
            }
        }

        public bool IsShouldStopAfterDiagonalMove
        {
            get
            {
                return DiagonalMoveRule == DiagonalMoves.DiagonalMovesStopAfterBeating;
            }
        }

        public bool IsCanBeatAfterLastRowAchieve
        {
            get
            {
                return ContinueBeatingLastRowRule == ContinueBeatAfterLastRowAchieve.CanBeatLikeSimpleChecker 
                    || ContinueBeatingLastRowRule == ContinueBeatAfterLastRowAchieve.CanBeatLikeCrownedChecker;
            }
        }

        public bool IsStillSimpleCheckerWhenBeatAfterLastRowAchieved
        {
            get
            {
                return ContinueBeatingLastRowRule == ContinueBeatAfterLastRowAchieve.CanBeatLikeSimpleChecker;
            }
        }
    }

    [System.Serializable]
    public enum DiagonalMoves
    {
        Unknown = -1,
        NoDiagonalMoves = 0,
        DiagonalMoves,
        DiagonalMovesStopAfterBeating
    }

    [System.Serializable]
    public enum Beating
    {
        Unknown = -1,
        NotRequired = 0,
        RequiredBeatMaximum, // Will be used in next release.
        RequiredBeatAnything
    }
    [System.Serializable]
    public enum SimpleCheckersBackBeating
    {
        Unknown = -1,
        CanBeatBack = 0,
        CanNotBeatBack
    }

    [System.Serializable]
    public enum ContinueBeatAfterLastRowAchieve
    {
        Unknown = -1,
        CanNotBeat = 0,
        CanBeatLikeSimpleChecker,
        CanBeatLikeCrownedChecker
    }
}