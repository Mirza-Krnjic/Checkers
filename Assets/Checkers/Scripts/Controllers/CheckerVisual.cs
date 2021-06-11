#if UNITY_EDITOR
using UnityEditor;
#endif
using DG.Tweening;
using UnityEngine;

namespace Checkers
{
	public class CheckerVisual : MonoBehaviour
	{
		public int Id;
		public Renderer CheckerRenderer;
		public Collider CheckerCollider;
		public SpriteRenderer ChoosenCheckerRenderer;
		public SpriteRenderer SuperCheckerRenderer;

		private Sequence _show;
		private Sequence _hide;

		/// <summary>
		/// Initialize checker with id.
		/// </summary>
		public void Init(int id)
		{
			Id = id;
            gameObject.name = "Checker: " + id;
		}

		public void OnMouseDown()
		{
			OnMouseDownClick();
		}

		public void SetEnableCollider(bool enable)
		{
			CheckerCollider.enabled = enable;
		}

		/// <summary>
		/// Show animation.
		/// </summary>
		public void Show(float time = 0)
		{
			_show.Kill();

			SetEnableCollider(true);

			_show.Append(transform.DOLocalMoveY(transform.localPosition.y-0.2f, time));
			_show.Join(CheckerRenderer.material.DOFade(1, time));
			_show.Join(ChoosenCheckerRenderer.DOFade(1, time));
			_show.Join(SuperCheckerRenderer.DOFade(1, time));
		}

		/// <summary>
		/// Hide animation.
		/// </summary>
		public void Hide(float time = 0)
		{
			_hide.Kill();

			SetEnableCollider(false);

			_hide.Append(transform.DOLocalMoveY(transform.localPosition.y + 0.2f, time));
			_hide.Join(CheckerRenderer.material.DOFade(0, time));
			_hide.Join(ChoosenCheckerRenderer.DOFade(0, time));
			_hide.Join(SuperCheckerRenderer.DOFade(0, time));
		}

		/// <summary>
		/// Called whe player click on checker.
		/// </summary>
		private void OnMouseDownClick()
		{
			BoardController.Instance.OnCheckerClicked(Id);
		}

		[ContextMenu("Become Crown Checker Immediately")]
		public void BecomeCrownCheckerImmediately()
		{
			Checker ch = GameController.Instance.CoreInstance.GetChecker(Id);
			ch.BecomeCrownChecker();
			BoardController.Instance.SetCrownChecker(ch);
		}

		public void OnDrawGizmos()
		{
#if UNITY_EDITOR
			//Show positions in scene using Handles
			if (!Application.isMobilePlatform && Application.isPlaying)
			{
				Checker ch = GameController.Instance.CoreInstance.GetChecker(Id);
				if (ch != null)
				{
					GUIStyle style = new GUIStyle(EditorStyles.textField) { normal = new GUIStyleState() { textColor = Color.blue }, fontStyle = FontStyle.Bold };

					Handles.Label(transform.position + Vector3.up / 2, string.Format("{0}", ch.Id), style);
				}
			}
#endif
		}
	}
}
