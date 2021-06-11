using UnityEngine;

namespace Checkers
{
	public class AudioEvent : MonoBehaviour
	{
		public AudioController.AudioType AudioType;

		[Space(10)]
		public AudioEventByParameter Event;

		/// <summary>
		/// Call <see cref="Event"/> action.
		/// </summary>
		public void OnEvent()
		{
			if (Event == null)
				return;

			Event.Invoke(AudioType);
		}
	}
}