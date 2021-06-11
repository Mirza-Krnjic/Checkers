using System;

namespace Checkers
{
    public class GameRulesController : Singleton<GameRulesController>
    {
        public event Action<CheckersRule> SetRuleEvent;
        public event Action ChangeRuleEvent;

        public CheckersRule CurrentRule;
        public CheckersRulesContainer RulesContainer;

        public DiagonalMoves DiagonalMovesRule;
        public Beating BeatingRule;
        public SimpleCheckersBackBeating BackBeatingRule;
        public ContinueBeatAfterLastRowAchieve ContinueBeatingLastRowRule;

        private int _ruleIndexInContainer = 0;

        /// <summary>
        /// Update parameters by game Rules.
        /// </summary>
        public void UpdateRulesParameters(CheckersRuleOptions options)
        {
            DiagonalMovesRule = options.DiagonalMoveRule;
            BeatingRule = options.BeatingRule;
            BackBeatingRule = options.BackBeatingRule;
            ContinueBeatingLastRowRule = options.ContinueBeatingLastRowRule;
        }

        /// <summary>
        /// Get current rule.
        /// </summary>
        public CheckersRule GetRule()
        {
            CheckersRule rule = null;

            rule = RulesContainer.Rules.Find(x => x.Options.DiagonalMoveRule == DiagonalMovesRule &&
                                                  x.Options.BeatingRule == BeatingRule &&
                                                  x.Options.BackBeatingRule == BackBeatingRule &&
                                                  x.Options.ContinueBeatingLastRowRule == ContinueBeatingLastRowRule);
            if (!rule)
            {
                rule = RulesContainer.CustomRule;
                rule.Options.DiagonalMoveRule = DiagonalMovesRule;
                rule.Options.BackBeatingRule = BackBeatingRule;
                rule.Options.BeatingRule = BeatingRule;
                rule.Options.ContinueBeatingLastRowRule = ContinueBeatingLastRowRule;
                SetRule(rule);
            }

            return rule;
        }

        /// <summary>
        /// Change rule.
        /// </summary>
        public void SetNextRule()
        {
            int currentIndex = _ruleIndexInContainer;

            CheckersRule rule = currentIndex >= RulesContainer.Rules.Count - 1 ? RulesContainer.Rules[0] : RulesContainer.Rules[currentIndex + 1];
            SetRule(rule);

            UpdateRulesParameters(rule.Options);

            ChangeRuleEvent?.Invoke();
        }

        /// <summary>
        /// Change rule.
        /// </summary>
        public void SetPrevRule()
        {
            int currentIndex = _ruleIndexInContainer;

            CheckersRule rule = currentIndex == 0 ? RulesContainer.Rules[RulesContainer.Rules.Count - 1] : RulesContainer.Rules[currentIndex - 1];

            SetRule(rule);

            UpdateRulesParameters(rule.Options);

            ChangeRuleEvent?.Invoke();
        }

        /// <summary>
        /// Set rule data.
        /// </summary>
        /// <param name="rule"></param>
        public void SetRule(CheckersRule rule)
        {
            CurrentRule = rule;

            int index = RulesContainer.Rules.IndexOf(CurrentRule);
            _ruleIndexInContainer = index == -1 ? 0 : index;

            UpdateRulesParameters(CurrentRule.Options);
            SetRuleEvent?.Invoke(rule);
        }

        /// <summary>
        /// Set rule by default.
        /// </summary>
        public void SetDefaultRule()
        {
            SetRule(RulesContainer.DefaultRule);
        }

        /// <summary>
        /// Set rule from last game from progress.
        /// </summary>
        public void SetRuleFromProgress()
        {
            CheckersRule rule = null;
            UndoData data = UndoPerformer.Instance.GetLastGame();
            RuleRecord record = data?.RuleRec;

            if (record != null)
            {
                rule = RulesContainer.Rules.Find(x => x.Type == record.Type);

                if (record.Type == CheckersRuleType.Unknown || string.IsNullOrEmpty(record.RuleLabel))
                {
                    SetDefaultRule();
                    return;
                }
                else if (record.Type == CheckersRuleType.Custom)
                {
                    rule = RulesContainer.CustomRule;
                    rule.Options.DiagonalMoveRule = record.DiagonalMoveRule;
                    rule.Options.BackBeatingRule = record.BackBeatingRule;
                    rule.Options.BeatingRule = record.BeatingRule;
                    rule.Options.ContinueBeatingLastRowRule = record.ContinueBeatingLastRowRule;
                }
                SetRule(rule);
            }
            else
            {
                SetDefaultRule();
            }
        }

        /// <summary>
        /// Will be used in next release.
        /// </summary>
        public void ChangeBackBeatAvailability()
        {
            switch (BackBeatingRule)
            {
                case SimpleCheckersBackBeating.CanBeatBack:
                    BackBeatingRule = SimpleCheckersBackBeating.CanNotBeatBack;
                    break;
                case SimpleCheckersBackBeating.CanNotBeatBack:
                    BackBeatingRule = SimpleCheckersBackBeating.CanBeatBack;
                    break;
            }
            UpdateRulesParameters(GetRule().Options);
        }
    }
}