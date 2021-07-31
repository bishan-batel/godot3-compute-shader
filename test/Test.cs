using Godot;
using ComputeShaderTest.addons.computeshader;

namespace ComputeShaderTest.test
{
  internal class Test : Node2D
  {
    [Export] Shader _shader;

    public override async void _Input(InputEvent @event)
    {
      if (!@event.IsActionPressed("debug")) return;

      
    }
  }
}