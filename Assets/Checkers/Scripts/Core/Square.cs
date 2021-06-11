using System.Collections.Generic;

namespace Checkers
{
    [System.Serializable]
    public class Square
    {
        /// <summary>
        /// Id of square.
        /// </summary>
        public int Id;

        /// <summary>
        /// Position of square.
        /// </summary>
        public Position Position;

        /// <summary>
        /// Color of square.
        /// </summary>
        public SquareColor Color;

        /// <summary>
        /// Checker which placed on square.
        /// </summary>
        public Checker Checker;

        public Square()
        {

        }
        /// <summary>
        /// Create new Square instance.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="position"></param>
        /// <param name="color"></param>
        public Square(int id, Position position, SquareColor color)
        {
            Id = id;
            Position = position;
            Color = color;
        }

        public Square(Square square)
        {
            Id = square.Id;
            Position = square.Position;
            Color = square.Color;
            Checker = square.Checker;
        }

        /// <summary>
        /// Setup checker on square.
        /// </summary>
        public void SetChecker(Checker checker)
        {
            Checker = checker;
        }

        /// <summary>
        /// Is square contains checker.
        /// </summary>
        public bool HasChecker()
        {
            return Checker != null && Checker.Id != 0;
        }

        /// <summary>
        /// Return neibours of square.
        /// </summary>
        /// <param name="all"></param>
        public IEnumerable<Position> Neighbours(bool all, bool diagonal, bool isCanBeatBack)
        {
            if (!HasChecker()) yield break;

            if (Checker.IsSuperChecker)
                all = true;
            if (all && diagonal)
            {
                int upLeftX, upRightX, bottomLeftX, bottomRightX;
                int upLeftY, upRightY, bottomLeftY, bottomRightY;

                upLeftX = upRightX = bottomLeftX = bottomRightX = Position.X;
                upLeftY = upRightY = bottomLeftY = bottomRightY = Position.Y;
                for (int i = 0; i < 8; i++)
                {
                    yield return new Position(--upLeftX, ++upLeftY, PosDirection.UpLeft);

                    yield return new Position(++upRightX, ++upRightY, PosDirection.UpRight);

                    yield return new Position(--bottomLeftX, --bottomLeftY, PosDirection.BottomLeft);

                    yield return new Position(++bottomRightX, --bottomRightY, PosDirection.BottomRight);
                }

                yield break;
            }

            if (all || Checker.Color == CheckerColor.White)
            {

                //return top left
                yield return new Position(Position.X - 1, Position.Y + 1, PosDirection.UpLeft);
                //return top right
                yield return new Position(Position.X + 1, Position.Y + 1, PosDirection.UpRight);

                if (GameController.Instance.CoreInstance.GameRule.Options.IsCheckersCanBeatBack && isCanBeatBack)
                {
                    //return bottom left
                    yield return new Position(Position.X - 1, Position.Y - 1, PosDirection.BottomLeft);
                    //return bottom right
                    yield return new Position(Position.X + 1, Position.Y - 1, PosDirection.BottomRight);
                }
            }
            if (all || Checker.Color == CheckerColor.Black)
            {
                //return bottom left
                yield return new Position(Position.X - 1, Position.Y - 1, PosDirection.BottomLeft);
                //return bottom right
                yield return new Position(Position.X + 1, Position.Y - 1, PosDirection.BottomRight);

                if (GameController.Instance.CoreInstance.GameRule.Options.IsCheckersCanBeatBack && isCanBeatBack)
                {
                    //return top left
                    yield return new Position(Position.X - 1, Position.Y + 1, PosDirection.UpLeft);
                    //return top right
                    yield return new Position(Position.X + 1, Position.Y + 1, PosDirection.UpRight);
                }
            }
        }

        /// <summary>
        /// Return neibours of square.
        /// </summary>
        /// <param name="all"></param>
        public IEnumerable<Position> NeighboursInDirection(bool diagonal, PosDirection dir)
        {
            if (diagonal)
            {
                int upLeftX, upRightX, bottomLeftX, bottomRightX;
                int upLeftY, upRightY, bottomLeftY, bottomRightY;

                upLeftX = upRightX = bottomLeftX = bottomRightX = Position.X;
                upLeftY = upRightY = bottomLeftY = bottomRightY = Position.Y;

                for (int i = 0; i < 8; i++)
                {
                    switch (dir)
                    {
                        case PosDirection.UpLeft:
                            yield return new Position(--upLeftX, ++upLeftY, PosDirection.UpLeft);
                            break;
                        case PosDirection.UpRight:
                            yield return new Position(++upRightX, ++upRightY, PosDirection.UpRight);
                            break;
                        case PosDirection.BottomLeft:
                            yield return new Position(--bottomLeftX, --bottomLeftY, PosDirection.BottomLeft);
                            break;
                        case PosDirection.BottomRight:
                            yield return new Position(++bottomRightX, --bottomRightY, PosDirection.BottomRight);
                            break;
                    }
                }
            }
            else
            {
                switch (dir)
                {
                    case PosDirection.UpLeft:
                        yield return new Position(Position.X - 1, Position.Y + 1, PosDirection.UpLeft);
                        break;
                    case PosDirection.UpRight:
                        yield return new Position(Position.X + 1, Position.Y + 1, PosDirection.UpRight);
                        break;
                    case PosDirection.BottomLeft:
                        yield return new Position(Position.X - 1, Position.Y - 1, PosDirection.BottomLeft);
                        break;
                    case PosDirection.BottomRight:
                        yield return new Position(Position.X + 1, Position.Y - 1, PosDirection.BottomRight);
                        break;
                }
            }
        }

        public Position NeighbourInDirection(PosDirection dir)
        {
            switch (dir)
            {
                case PosDirection.UpLeft:
                    return new Position(Position.X - 1, Position.Y + 1, PosDirection.UpLeft);
                case PosDirection.UpRight:
                    return new Position(Position.X + 1, Position.Y + 1, PosDirection.UpRight);
                case PosDirection.BottomLeft:
                    return new Position(Position.X - 1, Position.Y - 1, PosDirection.BottomLeft);
                case PosDirection.BottomRight:
                    return new Position(Position.X + 1, Position.Y - 1, PosDirection.BottomRight);
                default: return null;
            }
        }
    }
}