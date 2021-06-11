using UnityEngine;
using UnityEngine.UI;

namespace Checkers
{
    public class InfoPopup : GamePopup
    {
        [SerializeField]
        private Text _infoField;

        public void OnEnable()
        {
            UpdateView();
        }

        /// <summary>
        /// Update popup visual.
        /// </summary>
        private void UpdateView()
        {
            var rule = GameRulesController.Instance.GetRule();
            var info = rule != null && !string.IsNullOrEmpty(rule.Info) ? rule.Info : string.Empty;

            _infoField.text = info;
        }
    }
}