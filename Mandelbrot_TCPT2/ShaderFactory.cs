using OpenTK.Graphics.OpenGL;

namespace Mandelbrot_TCPT2
{
    /// <summary>
    /// Read shader files to use them in OpenGL
    /// </summary>
    internal class ShaderFactory
    {
        public static int CreateProgram(string vertexShaderFile, string fragmentShaderFile)
        {
            int shaderProgram = GL.CreateProgram();

            int vertexShader = CreateShader(ShaderType.VertexShader, ReadShaderCode(vertexShaderFile));
            int fragmentShader = CreateShader(ShaderType.FragmentShader, ReadShaderCode(fragmentShaderFile));

            GL.AttachShader(shaderProgram, vertexShader);
            GL.AttachShader(shaderProgram, fragmentShader);

            GL.LinkProgram(shaderProgram);

            GL.DetachShader(shaderProgram, vertexShader);
            GL.DetachShader(shaderProgram, fragmentShader);

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            return shaderProgram;
        }

        private static int CreateShader(ShaderType type, string shaderCode)
        {
            int shaderId = GL.CreateShader(type);

            GL.ShaderSource(shaderId, shaderCode);
            GL.CompileShader(shaderId);

            return shaderId;
        }

        private static string ReadShaderCode(string shaderFile)
        {
            using (var reader = new StreamReader(shaderFile))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
