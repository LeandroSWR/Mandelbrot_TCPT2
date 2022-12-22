using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Drawing;
using System.Drawing.Imaging;

namespace Mandelbrot_TCPT2
{
    internal class Window : GameWindow
    {
        private int vBufferH;
        private int vArrayH;

        private int shaderProgram;

        private float scale = 2.0f;
        private double centerX = -3.0d / 4.0d;
        private double centerY = 0.0d;
        private int maxIterations = 100;

        private double prevTime = 0.0;
        private double currentTime = 0.0;
        private double timeDiff;
        private int counter = 0;

        public Window() : base(GameWindowSettings.Default, new NativeWindowSettings())
        {
            this.CenterWindow(new Vector2i(1280, 720));
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);

            base.OnResize(e);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (KeyboardState.IsKeyDown(Keys.LeftShift))
            {
                // Change the max iterations
                maxIterations = Math.Max(2, maxIterations + (int)e.OffsetY * 10);
            }
            else
            {
                // Zoom in/out
                scale *= (1.0f - e.OffsetY * 0.05f);
            }

            base.OnMouseWheel(e);
        }


        protected unsafe override void OnMouseMove(MouseMoveEventArgs e)
        {
            if (MouseState.IsButtonDown(MouseButton.Middle))
            {
                // Hide the mouse cursor
                GLFW.SetInputMode(WindowPtr, CursorStateAttribute.Cursor, CursorModeValue.CursorHidden);

                // Set the translations speed when moving arround
                var translationSpeed = 0.003d * scale;

                centerX -= e.DeltaX * translationSpeed;
                centerY += e.DeltaY * translationSpeed;
            }
            else
            {
                // Show the mouse cursor
                GLFW.SetInputMode(WindowPtr, CursorStateAttribute.Cursor, CursorModeValue.CursorNormal);
            }

            base.OnMouseMove(e);
        }

        protected override void OnLoad()
        {
            GL.ClearColor(new Color4(0.3f, 0.4f, 0.5f, 1.0f));

            float[] vertex = new float[]
            {
            -1.0f, 1.0f, 0.0f,
            -1.0f, -1.0f, 0.0f,
            1.0f, 1f, 0.0f,
            1.0f,  1.0f, 0.0f,
            1.0f, -1.0f, 0.0f,
            -1.0f, -1.0f, 0.0f
            };

            // Generate and bind the vertex buffer
            vBufferH = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vBufferH);
            GL.BufferData(BufferTarget.ArrayBuffer, vertex.Length * sizeof(float), vertex, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            vArrayH = GL.GenVertexArray();
            GL.BindVertexArray(vArrayH);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vBufferH);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindVertexArray(0);

            shaderProgram = ShaderFactory.CreateProgram("Shaders/vertex_shader.glsl", "Shaders/fragment_shader.glsl");

            base.OnLoad();
        }

        protected override void OnUnload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(vBufferH);

            GL.UseProgram(0);
            GL.DeleteProgram(shaderProgram);

            base.OnUnload();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
        }

        private unsafe void CheckPerformance()
        {
            currentTime = GLFW.GetTime();
            timeDiff = currentTime - prevTime;
            counter++;
            if (timeDiff >= 1.0 / 30.0)
            {
                GLFW.SetWindowTitle(this.WindowPtr, $"MandelbrotGL | " +
                    $"{((1.0 / timeDiff) * counter):0.00}fps | " +
                    $"{((timeDiff / counter) * 1000):0.00}ms | " +
                    $"{maxIterations} iters | " +
                    $"{(1 / scale):00}x");

                prevTime = currentTime;
                counter = 0;
            }
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            CheckPerformance();

            double windowAspect = (double)ClientSize.X / (double)ClientSize.Y;

            GL.UseProgram(shaderProgram);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "windowAspect"), windowAspect);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "centerX"), centerX);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "centerY"), centerY);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "scale"), scale);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "maxIterations"), maxIterations);

            // Bind Vertex Array Object
            GL.BindVertexArray(vArrayH);
            // Draw
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

            this.Context.SwapBuffers();
            base.OnRenderFrame(args);
        }
    }
}
