using UnityEngine;

namespace Checkers
{
	public class ButtonModeEvent : MonoBehaviour
	{
		public GameMode ModeType;

		[Space(10)]
		public ButtonModeEventByParameter Event;

		/// <summary>
		/// Call <see cref="Event"/> action.
		/// </summary>
		public void OnEvent()
		{
			if (Event == null)
				return;

			Event.Invoke(ModeType);
		}
	}
}