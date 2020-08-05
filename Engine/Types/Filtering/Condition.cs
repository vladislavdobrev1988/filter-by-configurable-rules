using Engine.Enums.Filtering;

namespace Engine.Types.Filtering
{
    public class Condition
    {
        public string Property { get; set; }
        public object Value { get; set; }
        public ComparisonType ComparisonType { get; set; }
    }
}
