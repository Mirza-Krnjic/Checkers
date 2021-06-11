using UnityEngine;
using UnityEditor;

namespace Checkers
{
	[CustomEditor(typeof(PuzzleController))]
	public class PuzzleControllerEditor : Editor
	{
		[SerializeField] private PuzzleController _ctrl;

		private void OnEnable()
		{
			_ctrl = target as PuzzleController;
		}

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			DrawCustomInspector();
		}

		private void DrawCustomInspector()
		{
			GUILayout.Space(20f);
			GUILayout.Label("Test options:");

			if (GUILayout.Button("Clear all progress"))
			{
				PuzzleController.Instance.ClearProgress();
			}
			SetEditorDirty();
		}

		private void SetEditorDirty()
		{
			if (GUI.changed)
			{
				GUI.changed = false;
				EditorUtility.SetDirty(_ctrl);
			}
		}
	}
}