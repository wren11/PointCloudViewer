using Silk.NET.OpenGL;

namespace ConsoleApp2;

public class Shader
{
    private readonly GL _gl;
    public uint Handle { get; private set; }

    public Shader(GL gl, string vertexSource, string fragmentSource)
    {
        _gl = gl;

        var vertex = _gl.CreateShader(ShaderType.VertexShader);
        _gl.ShaderSource(vertex, vertexSource);
        CompileShader(vertex);

        var fragment = _gl.CreateShader(ShaderType.FragmentShader);
        _gl.ShaderSource(fragment, fragmentSource);
        CompileShader(fragment);

        Handle = _gl.CreateProgram();
        _gl.AttachShader(Handle, vertex);
        _gl.AttachShader(Handle, fragment);
        _gl.LinkProgram(Handle);

        _gl.DeleteShader(vertex);
        _gl.DeleteShader(fragment);
    }

    private void CompileShader(uint shader)
    {
        _gl.CompileShader(shader);
        _gl.GetShader(shader, ShaderParameterName.CompileStatus, out var success);
        if (success == 0)
        {
            var infoLog = _gl.GetShaderInfoLog(shader);
            Console.WriteLine($"Shader compilation failed: {infoLog}");
        }
    }

    public void Use() => _gl.UseProgram(Handle);
    public void Dispose() => _gl.DeleteProgram(Handle);
}