namespace Checkers
{
	[System.Serializable]
    public class Checker
    {
		/// <summary>
		/// Checker id.
		/// </summary>
		public int Id;

		/// <summary>
		/// Reference to square.
		/// </summary>
		public Square Square;

		/// <summary>
		/// Color of checker.
		/// </summary>
		public CheckerColor Color;

		/// <summary>
		/// True when checker is super.
		/// </summary>
		public bool IsSuperChecker;

		/// <summary>
		/// True when checker was beat.
		/// </summary>
		public bool IsBeat;

		public Checker()
		{

		}

		/// <summary>
		/// Create new checker instance.
		/// </summary>
		public Checker(int id, Square square, CheckerColor color)
        {
            Id = id;
            Square = square;
            Color = color;
            IsSuperChecker = false;
            IsBeat = false;
        }

		public Checker(Checker ch)
		{
			Id = ch.Id;
			Square = ch.Square;
			Color = ch.Color;
			IsSuperChecker = ch.IsSuperChecker;
			IsBeat = ch.IsBeat;
		}


		/// <summary>
		/// Change square ref of checker
		/// </summary>
		/// <param name="square"></param>
		public void MoveTo(Square square)
        {
            Square = square;
        }

		/// <summary>
		/// Set super checker state.
		/// </summary>
        public void BecomeCrownChecker()
        {
            IsSuperChecker = true;
        }

		/// <summary>
		/// Set beat state.
		/// </summary>
        public void WasBeat()
        {
            IsBeat = true;
        }
    }
}
