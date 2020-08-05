using Engine.Enums.Filtering;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Engine.Types.Filtering
{
    public class Condition
    {
        public string Property { get; set; }
        public object Value { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ComparisonType ComparisonType { get; set; }
    }
}
