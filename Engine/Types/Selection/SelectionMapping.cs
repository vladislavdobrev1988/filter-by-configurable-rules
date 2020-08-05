using Engine.Enums.Selection;

namespace Engine.Types.Selection
{
    public class SelectionMapping
    {
        public string InputProperty { get; set; }
        public string OutputProperty { get; set; }
        public StringConversionType? StringConversionType { get; set; }
    }
}
