using Godot;

#if TOOLS
namespace ComputeShaderTest.addons.computeshader
{
  [Tool]
  public class ComputeShaderPlugin : EditorPlugin
  {
    public const string AddonPath = "res://addons/computeshader";

    public override void _EnterTree()
    {
      AddCustomType(
        nameof(ComputeShader),
        nameof(Node),
        GD.Load<Script>("ComputeShader.cs".ComputePath()),
        GD.Load<Texture>("icon.png".ComputePath())
      );
    }

    public override void _ExitTree()
    {
      RemoveCustomType(nameof(ComputeShader));
    }
  }

  public static class ComputeShaderPluginExtensions
  {
    public static string ComputePath(this string name) => ComputeShaderPlugin.AddonPath.PlusFile(name);
  }
}
#endif