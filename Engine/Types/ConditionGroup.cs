using Engine.Enums;

namespace Engine.Types
{
    public class ConditionGroup
    {
        public ConditionGroupType? ConditionType { get; set; }
        public ConditionGroup[] Conditions { get; set; }
        public Condition Condition { get; set; }
    }
}
