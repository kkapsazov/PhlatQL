using System.Collections.Generic;

namespace PhlatQL.Core.Validation
{
    public class ValidationRuleBuilder
    {
        private readonly List<ValidationRule> rules = new();

        public ValidationRuleBuilder MaxValue(int val)
        {
            this.AddRule(RuleType.MaxValue, val.ToString());
            return this;
        }

        public ValidationRuleBuilder MinValue(int val)
        {
            this.AddRule(RuleType.MinValue, val.ToString());
            return this;
        }

        public ValidationRuleBuilder StrictMaxValue(int val)
        {
            this.AddRule(RuleType.StrictMaxValue, val.ToString());
            return this;
        }

        public ValidationRuleBuilder StrictMinValue(int val)
        {
            this.AddRule(RuleType.StrictMinValue, val.ToString());
            return this;
        }

        public ValidationRuleBuilder MaxLength(int val)
        {
            this.AddRule(RuleType.MaxLength, val.ToString());
            return this;
        }

        public ValidationRuleBuilder MinLength(int val)
        {
            this.AddRule(RuleType.MinLength, val.ToString());
            return this;
        }

        public ValidationRuleBuilder Regex(string val)
        {
            this.AddRule(RuleType.Regex, val);
            return this;
        }

        public ValidationRuleBuilder Required()
        {
            this.AddRule(RuleType.Required, null);
            return this;
        }

        private void AddRule(RuleType type, string val)
        {
            this.rules.Add(new ValidationRule
            {
                Type = type,
                Value = val
            });
        }

        public List<ValidationRule> Build()
        {
            return this.rules;
        }
    }
}
