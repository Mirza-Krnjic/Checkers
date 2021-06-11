using UnityEngine;
using UnityEngine.UI;

namespace Checkers
{
	[System.Serializable]
	public class PuzzleItem : MonoBehaviour
	{
        public int ElementId;
		public Text IdText;
		public Button PuzzleActionButton;
		public Image StatusImage;
	}
}