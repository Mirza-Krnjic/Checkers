using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Checkers
{
    [CustomEditor(typeof(GamePopup),true)]
    public class GamePopupEditor : Editor
    {
        private GamePopup _target;

        public void OnEnable()
        {
            _target = target as GamePopup;
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open"))
            {
                _target.Open();
            }

            if (GUILayout.Button("Close"))
            {
                _target.Close();
            }

            base.OnInspectorGUI();
        }
    }
}