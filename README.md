# godot3-compute-shader

## TODO: Write README

Temporary work around around the lack of Compute Shaders in Godot 3.

Uses shaders rendering in viewports to get data back as a texture, this approach is very limited but for many cases it
gets the job done.

### This plugin currently only works for C#

# Installing

To install simply download this repository and put only the `addons/computeshader`
inside your addons folder.

# Usage

1. After adding the compute shader node into your scene, you need to create the actual shader that the node will run.
   and assign it to the node <br>
   For testing purposes, the shader below will just make each pixel in the texture the color red with a uniform for
   controlling how red.
    ```glsl
    shader_type canvas_item;
    uniform float red = 1.0;
    
    void fragment() {
       COLOR = vec4(red, 0.0, 0.0, 1.0);
   }
   ```
2. Run using C# inside of an asynchronous method
   ```c#
   // Retrieve the compute shader node
   var compute = GetNode<ComputeShader>("ComputeShader");
  
   // Retrieves texture that the the shader produced
   Image data = await compute.Compute();
   
   // Should print out each pixel as 
   for (int x = 0; x < data.GetWidth(); x++)
   for (int y = 0; y < data.GetHeight(); y++)
     GD.Print(data.GetPixel(x, y).r);
 
 
   
   ```

# Full Example

```c#
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

```
