namespace Checkers
{
	[System.Serializable]
	public class AiMove
	{
		public Checker Checker;
		public Square Square;

		public float Quality;

		public AiMove(Checker checker, Square square)
		{
			Checker = checker;
			Square = square;
		}

		public override string ToString()
		{
            string squareId = (Square != null) ? Square.Id.ToString() : "null";
            string checkerId = (Checker != null) ? Checker.Id.ToString() : "null";

            return $"Checker ID: {checkerId} \n" +
				   $"Square ID: {squareId} \n" ;
		}
	}
}