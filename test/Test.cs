using Godot;
using ComputeShaderTest.addons.computeshader;

namespace ComputeShaderTest.test
{
  internal class Test : Node2D
  {
    public override async void _Input(InputEvent @event)
    {
      if (!@event.IsActionPressed("debug")) return;

      var compute = GetNode<ComputeShader>("ComputeShader");
      compute.OutputSize = Vector2.One * 2; // 2x2 output

      var image = new Image();
      image.Create(2, 2, false, Image.Format.Rgba8);
      image.Lock(); // locks data to allow reading / writing

      // Texture made (with red color channel only:
      // 01 69
      // 42 02
      image.SetPixel(0, 0, Color.Color8(01, 0, 0, 0));
      image.SetPixel(1, 0, Color.Color8(69, 0, 0, 0));
      image.SetPixel(0, 1, Color.Color8(42, 0, 0, 0));
      image.SetPixel(1, 1, Color.Color8(02, 0, 0, 0));

      var texture = new ImageTexture();
      texture.CreateFromImage(image);

      // send data to shader and wait for result
      Image data = await compute.Compute(
        ("input", (Texture) texture),
        ("scale", .5f)
      );

      // print output
      GD.Print("Compute:");
      GD.Print($"\t{data.GetPixel(0, 0).r8} {data.GetPixel(1, 0).r8}");
      GD.Print($"\t{data.GetPixel(0, 1).r8} {data.GetPixel(1, 1).r8}");
    }
  }
}