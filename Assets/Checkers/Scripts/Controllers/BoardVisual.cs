using UnityEngine;

namespace Checkers
{
    public class BoardVisual : MonoBehaviour
    {
        [SerializeField]
        private Renderer _boardRenderer;

        [Header("Board textures:")]
        [SerializeField]
        private Texture _nonReverseTexture;
        [SerializeField]
        private Texture _reverseTexture;

        private const string _albedoFieldName = "_MainTex";

        /// <summary>
        /// Update board visual.
        /// </summary>
        public void UpdateView()
        {
            var rule = GameRulesController.Instance.GetRule();
            var isReverse = rule != null && rule.Options.IsReverseBoard;
            var tex = isReverse ? _reverseTexture : _nonReverseTexture;

            //_boardRenderer.material.SetTexture(_albedoFieldName, tex);
        }
    }
}