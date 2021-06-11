using UnityEngine;
using Text =  UnityEngine.UI.Text;

namespace Checkers
{
	public class AlertPopUpController : MonoBehaviour
	{
		public Text AlertPopUpMessage;

		/// <summary>
		/// Show alert poup with message.
		/// </summary>
		public static void ShowAlertPopUp(string msg)
		{
			AlertPopUpController alert = Instantiate(Resources.Load<AlertPopUpController>("Prefabs/AlertPopUp"));
			alert.InitAlertPopUp(msg);
			alert.transform.parent = FindObjectOfType<Canvas>().transform;
			alert.transform.localScale = Vector3.one;
			RectTransform alertRect = alert.GetComponent<RectTransform>();
			alertRect.sizeDelta = new Vector2(0, 0);
			alertRect.localPosition = new Vector2(0, 0);
		}

		/// <summary>
		/// Initialize alert popup.
		/// </summary>
		public void InitAlertPopUp(string message)
		{
			AlertPopUpMessage.text = message;
		}
	}
}
