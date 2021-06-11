using System.Collections;
using UnityEngine;

namespace Checkers
{
	public class EnableDisableObject : MonoBehaviour
	{
		[Header("Object Parameters:")]
		public bool NeedToDisableImmediatly;
		public float TimeToDisable;
		public float TimeToEnable;

		private void OnEnable()
		{
			InitObject();
		}

		/// <summary>
		/// Initialize object whe it enable.
		/// </summary>
		private void InitObject()
		{
			if (NeedToDisableImmediatly)
				DestroyObjectImmediately(TimeToDisable);
		}

		/// <summary>
		/// Destroy object from 0 sec.
		/// </summary>
		private void DestroyObjectImmediately(float time = 0f)
		{
			Destroy(gameObject, time);
		}

		/// <summary>
		/// Disable object.
		/// </summary>
		public void DisableAction(GameObject go)
		{
			StartCoroutine(DisableObjectAction(go));
		}

		/// <summary>
		/// Disable coroutine.
		/// </summary>
		public IEnumerator DisableObjectAction(GameObject go)
		{
			yield return new WaitForSeconds(TimeToDisable);
			go.SetActive(false);
		}

		/// <summary>
		/// Activate object.
		/// </summary>
		/// <param name="go"></param>
		public void ActiveAction(GameObject go)
		{
			StartCoroutine(EnableObjectAction(go));
		}

		/// <summary>
		/// Enable coroutine.
		/// </summary>
		public IEnumerator EnableObjectAction(GameObject go)
		{
			yield return new WaitForSeconds(TimeToEnable);
			go.SetActive(true);
		}
	}
}