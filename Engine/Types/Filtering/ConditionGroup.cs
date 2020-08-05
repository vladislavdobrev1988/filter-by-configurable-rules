using Engine.Enums.Filtering;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Engine.Types.Filtering
{
    public class ConditionGroup
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ConditionGroupType? ConditionType { get; set; }
        public ConditionGroup[] Conditions { get; set; }
        public Condition Condition { get; set; }
    }
}
