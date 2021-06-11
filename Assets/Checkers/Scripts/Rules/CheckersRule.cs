using UnityEngine;

namespace Checkers
{
    [CreateAssetMenu(fileName = "CheckersRule", menuName = "Checkers Rules/Checkers Rule")]
    public class CheckersRule : ScriptableObject
    {
        public CheckersRuleType Type;
        public string Label;

        [TextArea(3,10)]
        public string Info;
        public Sprite Preview;

        public CheckersRuleOptions Options;
    }
}