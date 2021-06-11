using UnityEngine;

namespace Checkers
{
	public class ToggleColorEvent : MonoBehaviour
	{
		public UserColor ColorType;

		[Space(10)]
		public ToggleColorEventByParameter Event;

		/// <summary>
		/// Call <see cref="Event"/> action.
		/// </summary>
		public void OnEvent(bool _event)
		{
			if (!_event)
				return;
			if (Event == null)
				return;

			Event.Invoke(ColorType);
		}
	}
}