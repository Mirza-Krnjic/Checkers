using System.Collections.Generic;
using UnityEngine;

namespace Checkers
{
    [CreateAssetMenu(fileName = "CheckersRulesContainer", menuName = "Checkers Rules/Checkers Rules Container")]
    public class CheckersRulesContainer : ScriptableObject
    {
        public List<CheckersRule> Rules;
        public CheckersRule DefaultRule;
        public CheckersRule CustomRule;
    }
}