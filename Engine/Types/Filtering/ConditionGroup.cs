using Engine.Enums.Filtering;

namespace Engine.Types.Filtering
{
    public class ConditionGroup
    {
        public ConditionGroupType? ConditionType { get; set; }
        public ConditionGroup[] Conditions { get; set; }
        public Condition Condition { get; set; }
    }
}
