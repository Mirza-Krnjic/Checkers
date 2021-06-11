using System;

namespace Checkers
{
	[System.Serializable]
	public class Position
	{
		public int X;

		public int Y;

		public PosDirection Direction;

		public Position()
		{
		}

		public Position(int x, int y, PosDirection dir)
		{
			X = x;
			Y = y;
			Direction = dir;
		}

		public Position(Position other)
		{
			X = other.X;
			Y = other.Y;
		}

		public Position Abs()
		{
			return new Position(Math.Abs(X), Math.Abs(Y), PosDirection.None);
		}

		public static Position operator -(Position leftPos, Position rightPos)
		{
			return new Position(leftPos.X - rightPos.X, leftPos.Y - rightPos.Y, PosDirection.None);
		}

		public static Position operator +(Position leftPos, Position rightPos)
		{
			return new Position(leftPos.X + rightPos.X, leftPos.Y + rightPos.Y, PosDirection.None);
		}

		public static Position operator *(int multiplier, Position rightPos)
		{
			return new Position(multiplier * rightPos.X, multiplier * rightPos.Y, PosDirection.None);
		}

		public static Position operator /(Position leftPos, int value)
		{
			return new Position(leftPos.X / value, leftPos.Y / value, PosDirection.None);
		}

		public static bool operator ==(Position leftPos, Position rightPos)
		{
			return leftPos.X == rightPos.X && leftPos.Y == rightPos.Y;
		}

		public static bool operator !=(Position leftPos, Position rightPos)
		{
			return !(leftPos == rightPos);
		}
	}

	public enum PosDirection
	{
		None = 0,
		UpLeft,
		UpRight,
		BottomLeft,
		BottomRight
	}
}