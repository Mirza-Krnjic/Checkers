using UnityEditor;
using UnityEngine;

namespace Checkers
{
    [CustomEditor(typeof(MetricsController), true)]
    public class MetricControllerEditor : Editor
    {
        private MetricsController _target;

        public void OnEnable()
        {
            _target = target as MetricsController;
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Activate"))
            {
                _target.ActivateMetrics(true);
            }

            if (GUILayout.Button("Deactivate"))
            {
                _target.ActivateMetrics(false);
            }

            base.OnInspectorGUI();
        }
    }
}