#if TOOLS
using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ComputeShaderTest.addons.computeshader
{
  [Tool]
  public class ComputeShader : Node
  {
    /// <summary>
    /// Shader to be run
    /// </summary>
    [Export]
    public Shader Shader { set; get; }

    /// <summary>
    /// Output Size for computation, must be a minimum of 4x4
    /// </summary>
    [Export]
    public Vector2 OutputSize
    {
      set
      {
        _outputSize = value.Round();
        if (_outputSize.x < 2 || _outputSize.y < 2)
          _outputSize = Vector2.One * 2;
      }
      get => _outputSize;
    }

    public Viewport Port { private set; get; }

    Vector2 _outputSize;

    public override void _EnterTree()
    {
      AddChild(Port = new Viewport
      {
        RenderTargetUpdateMode = Viewport.UpdateMode.Disabled,
        Disable3d = true,
        Usage = Viewport.UsageEnum.Usage2d,
        OwnWorld = true
      });
    }


    /// <summary>
    /// Asynchronously applies shader with inputted parameters and returns result
    /// </summary>
    /// <param name="shaderParams">Array of key-value pairs which are set as the shader params</param>
    public async Task<Image> Compute(params ShaderParam[] shaderParams)
    {
      Port.GetChildren().Cast<Node>().ToList().ForEach(node => node.Free());

      Port.Size = OutputSize;

      // Create sprite as a container for the shader
      var texture = GD.Load<Texture>("icon.png");

      Port.AddChild(new Sprite
      {
        Texture = texture,
        Scale = new Vector2(
          OutputSize.x / texture.GetWidth(),
          OutputSize.y / texture.GetHeight()
        ),
        Position = OutputSize * .5f,
        Material = CreateShaderMaterial(shaderParams)
      });


      // Wait for output to render and return result
      _updateViewport();
      await ToSignal(VisualServer.Singleton, "frame_post_draw");
      return GetOutput();
    }

    public ViewportTexture GetTexture() => Port.GetTexture();

    /// <summary>
    /// Gets output of viewport (acts as result for any computation done)
    /// </summary>
    public Image GetOutput()
    {
      Image data = Port.GetTexture().GetData();
      data.FlipY();
      data.Lock();
      return data;
    }

    /// <summary>
    /// Creates a material with the shader set along with the specified parameters 
    /// </summary>
    /// <param name="shaderParams">Array of key-value pairs which are set as the shader params</param>
    public ShaderMaterial CreateShaderMaterial(params ShaderParam[] shaderParams)
    {
      var shaderMaterial = new ShaderMaterial
      {
        Shader = Shader
      };
      shaderParams?
          .ToList()
          .ForEach(
            shaderParam => shaderMaterial.SetShaderParam(shaderParam.Param, shaderParam.Value)
          );
      return shaderMaterial;
    }

    /// <summary>
    /// Forces viewport to re render
    /// </summary>
    protected virtual void _updateViewport()
    {
      Port.RenderTargetUpdateMode = Viewport.UpdateMode.Disabled;
      Port.UpdateWorlds();
      Port.RenderTargetUpdateMode = Viewport.UpdateMode.Once;
    }

    [Obsolete]
    public static async Task<Image> Compute(Node parent, int width, int height, Shader shader, params ShaderParam[] shaderParams)
    {
      if (shader is null) throw new NullReferenceException($"{nameof(shader)} cannot be null");
      if (width < 2) throw new ArgumentOutOfRangeException(nameof(width), "Minimum size for compute shader to work is 2x2");
      if (height < 2) throw new ArgumentOutOfRangeException(nameof(height), "Minimum size for compute shader to work is 2x2");

      var shaderNode = new ComputeShader
      {
        OutputSize = new Vector2(width, height),
        Shader = shader
      };
      parent.AddChild(shaderNode);
      await parent.ToSignal(shaderNode, "ready");

      return await shaderNode.Compute(shaderParams);
    }
  }

  public struct ShaderParam
  {
    public string Param;
    public object Value;

    public static implicit operator ShaderParam((string param, object value) tuple) =>
        new()
        {
          Param = tuple.param,
          Value = tuple.value
        };
  }
}
#endif