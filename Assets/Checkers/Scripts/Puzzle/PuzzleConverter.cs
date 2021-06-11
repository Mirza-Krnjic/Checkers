using System;
using System.Collections.Generic;
using UnityEngine;

namespace Checkers
{
	[System.Serializable]
	public static class PuzzleConverter
	{
        /// <summary>
        /// Convert list of puzzle formats to puzzles.
        /// </summary>
		public static List<Puzzle> ConvertPuzzles(List<PuzzleFormat> puzzleFormats)
		{
			List<Puzzle> puzzles = new List<Puzzle>();

			foreach (var puzzle in puzzleFormats)
			{
				puzzles.Add(ConvertPuzzle(puzzle));
			}

			return puzzles;
		}

        /// <summary>
        /// Convert puzzle fortmat to puzzle.
        /// </summary>
		public static Puzzle ConvertPuzzle(PuzzleFormat format)
		{
			Puzzle puzzleData = new Puzzle();
			if (format != null)
			{
                puzzleData.OptimalMoves = format.OptimalMoves;
                puzzleData.PuzzleId = format.PuzzleId;
                puzzleData.WhitePositions = ConvertPuzzlePositions(format.WhitePositions, CheckerColor.White);
				puzzleData.BlackPositions = ConvertPuzzlePositions(format.BlackPositions, CheckerColor.White);
			}
			else
			{
				Debug.LogError("Puzzle equals null");
			}
			return puzzleData;
		}

		/// <summary>
		/// Convert string array into Position format
		/// </summary>
		/// <param name="positionsInBoardFormat"></param>
		/// <param name="color"></param>
		/// <returns></returns>
		private static List<Position> ConvertPuzzlePositions(List<string> positionsInBoardFormat, CheckerColor color)
		{
			List<Position> gamePositons = new List<Position>();
			foreach (var pos in positionsInBoardFormat)
			{
				char[] letters = pos.ToCharArray();
				gamePositons.Add(color == CheckerColor.White ?
					PlayerPosConverter(char.ToLower(letters[0]), char.ToLower(letters[1])) :
					EnemyPosConverter(char.ToLower(letters[0]), char.ToLower(letters[1])));
			}
			return gamePositons;
		}

		/// <summary>
		/// Parse board data from bottom side.
		/// </summary>
		private static Position PlayerPosConverter(char letter, char number)
		{
			char firstAlphabetLetter = 'a';

			return new Position(letter - firstAlphabetLetter, 8 - Convert.ToInt32(number.ToString()), PosDirection.None);
		}

		/// <summary>
		/// Parse board data from up side.
		/// </summary>
		private static Position EnemyPosConverter(char letter, char number)
		{
			char firstAlphabetLetter = 'a';

			return new Position(8 - (letter - firstAlphabetLetter) - 1, Convert.ToInt32(number.ToString()) - 1, PosDirection.None);
		}
	}
}