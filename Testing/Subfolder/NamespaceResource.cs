using Godot;
using MonoCustomResourceRegistry;

namespace Testing.Subfolder
{
    [RegisteredType(nameof(NamespaceResource), "res://Testing/icon.png")]
    public partial class NamespaceResource : Resource
    {
        [Export]
        public int Number { get; set; }
    }
}
