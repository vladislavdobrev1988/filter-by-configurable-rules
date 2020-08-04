using Engine.Enums;

namespace Engine.Types
{
    public class Condition
    {
        public string Property { get; set; }
        public object Value { get; set; }
        public ComparisonType ComparisonType { get; set; }
    }
}
