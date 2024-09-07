using System.Numerics;
using System.Runtime.InteropServices;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace ConsoleApp2
{
    public class Program
    {
        private IWindow? _window;
        private GL _gl;
        private OctreeNode? _octreeRoot;
        private Camera? _camera;
        private Shader? _shader;
        private uint _vao, _vbo;
        private List<Point3D>? _points;
        private IInputContext? _inputContext;
        private IMouse? _mouse;

        private Vector3 _cameraPosition = new Vector3(0, 0, 1000);
        private readonly Vector3 _cameraTarget = Vector3.Zero;

        public static void Main()
        {
            var app = new Program();
            app.Run();
        }

        public void Run()
        {
            var windowOptions = WindowOptions.Default with
            {
                Size = new Vector2D<int>(800, 600),
                Title = "Point Cloud Viewer with Sphere-like Points"
            };
            _window = Window.Create(windowOptions);
            _window.Load += OnLoad;
            _window.Render += OnRender;
            _window.Closing += OnClose;
            _window.Run();
        }

        private Vector2 _lastMousePosition;
        private bool _isRotating = false;
        private bool _isPanning = false;
        private float _yaw = 0.0f;
        private float _pitch = 0.0f;
        private Vector3 _cameraOffset = Vector3.Zero;
        private IKeyboard _keyboard;

        private void OnLoad()
        {
            _gl = GL.GetApi(_window);
            _gl.ClearColor(1.1f, 1.1f, 1.1f, 1.0f);

            _inputContext = _window.CreateInput();
            _keyboard = _inputContext.Keyboards[0];
            _mouse = _inputContext.Mice[0];

            _mouse.Scroll += OnMouseScroll;

            _keyboard = _inputContext.Keyboards[0];

            _mouse.Scroll += OnMouseScroll;
            _mouse.MouseMove += OnMouseMove;
            _mouse.MouseDown += OnMouseDown;
            _mouse.MouseUp += OnMouseUp;

            _camera = new Camera
            {
                ViewMatrix = Matrix4x4.CreateLookAt(_cameraPosition, _cameraTarget, Vector3.UnitY),
                ProjectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView((float)Math.PI / 4, 800f / 600f, 0.01f, 10000f)
            };

            _shader = new Shader(_gl, VertexShaderSource, FragmentShaderSource);
            _shader.Use();

            var pointCloudData = LoadPointCloudData();
            _octreeRoot = new OctreeNode(pointCloudData.BoundingBox);
            PartitionPointCloud(pointCloudData.Points);

            _points = pointCloudData.Points;
            SetupBuffers();
        }

        private void OnMouseDown(IMouse mouse, MouseButton button)
        {
            if (button == MouseButton.Left)
                _isRotating = true;
            if (button == MouseButton.Right)
                _isPanning = true;
        }

        private void OnMouseUp(IMouse mouse, MouseButton button)
        {
            if (button == MouseButton.Left)
                _isRotating = false;
            if (button == MouseButton.Right)
                _isPanning = false;
        }

        private void OnMouseMove(IMouse mouse, Vector2 position)
        {
            if (_isRotating)
            {
                var delta = position - _lastMousePosition;
                _yaw += delta.X * 0.2f;
                _pitch += delta.Y * 0.2f;
            }
            if (_isPanning)
            {
                var delta = position - _lastMousePosition;
                _cameraOffset.X += delta.X * 0.01f;
                _cameraOffset.Y -= delta.Y * 0.01f;
            }
            _lastMousePosition = position;
            UpdateCamera();
        }

        private void PartitionPointCloud(List<Point3D> points)
        {
            foreach (var point in points)
            {
                _octreeRoot.InsertPoint(point);
            }
        }


        private void OnMouseScroll(IMouse mouse, ScrollWheel scroll)
        {
            var zoomAmount = scroll.Y * 5f;

            var cursorPosition = _mouse.Position;
            var worldCursor = UnprojectCursorToWorld(cursorPosition);

            var direction = Vector3.Normalize(worldCursor - _cameraPosition);

            _cameraPosition += direction * zoomAmount;

            UpdateCamera();
        }

        private Vector3 UnprojectCursorToWorld(Vector2 cursorPosition)
        {
            var windowSize = _window.Size;
            var normalizedCursor = new Vector2(
                (2.0f * cursorPosition.X) / windowSize.X - 1.0f,
                1.0f - (2.0f * cursorPosition.Y) / windowSize.Y
            );

            var rayClip = new Vector4(normalizedCursor, -1.0f, 1.0f);

            Matrix4x4.Invert(_camera.ProjectionMatrix, out var invProjection);
            var rayEye = Vector4.Transform(rayClip, invProjection);
            rayEye.Z = -1.0f;
            rayEye.W = 0.0f;

            Matrix4x4.Invert(_camera.ViewMatrix, out var invView);
            var rayWorld = Vector3.Normalize(new Vector3(rayEye.X, rayEye.Y, rayEye.Z));

            return _cameraPosition + rayWorld * 100.0f;
        }

        private void UpdateCamera()
        {
            var yawRadians = _yaw * (MathF.PI / 180.0f);
            var pitchRadians = _pitch * (MathF.PI / 180.0f);

            var rotation = Matrix4x4.CreateFromYawPitchRoll(yawRadians, pitchRadians, 0.0f);

            var rotatedTarget = Vector3.Transform(Vector3.UnitZ, rotation);
            var cameraTarget = rotatedTarget + _cameraOffset;

            _camera.ViewMatrix = Matrix4x4.CreateLookAt(_cameraPosition, cameraTarget, Vector3.UnitY);

            _camera.ProjectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView((float)Math.PI / 4, 800f / 600f, 0.01f, 10000f);
        }



        private void SetupBuffers()
        {
            _vao = _gl.GenVertexArray();
            _vbo = _gl.GenBuffer();

            _gl.BindVertexArray(_vao);

            var pointData = new float[_points.Count * 3];
            for (var i = 0; i < _points.Count; i++)
            {
                pointData[i * 3 + 0] = _points[i].X;
                pointData[i * 3 + 1] = _points[i].Y;
                pointData[i * 3 + 2] = _points[i].Z;
            }

            ReadOnlySpan<byte> span = MemoryMarshal.Cast<float, byte>(pointData);

            _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
            _gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)span.Length, span, BufferUsageARB.StaticDraw);

            _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            _gl.EnableVertexAttribArray(0);

            _gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
            _gl.BindVertexArray(0);
        }

        private void OnRender(double deltaTime)
        {
            _gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            _gl.Enable(GLEnum.LineSmooth);
            _gl.Hint(GLEnum.Points, GLEnum.Nicest);

            _shader.Use();

            var projectionLocation = _gl.GetUniformLocation(_shader.Handle, "projection");
            var projectionMatrix = Matrix4X4ToFloatArray(_camera.ProjectionMatrix);
            _gl.UniformMatrix4(projectionLocation, 1, false, projectionMatrix);

            var viewLocation = _gl.GetUniformLocation(_shader.Handle, "view");
            var viewMatrix = Matrix4X4ToFloatArray(_camera.ViewMatrix);
            _gl.UniformMatrix4(viewLocation, 1, false, viewMatrix);

            _gl.BindVertexArray(_vao);
            RenderOctreeNode(_octreeRoot);

            _window.SwapBuffers();
        }

        private void RenderOctreeNode(OctreeNode node)
        {
            if (node == null) return;

            if (!_camera.IsInView(node.BoundingBox)) return;

            if (node.Points.Count > 0)
            {
                _gl.DrawArrays(PrimitiveType.Points, 0, (uint)_points.Count);
            }

            if (node.Children != null)
            {
                foreach (var child in node.Children)
                {
                    RenderOctreeNode(child);
                }
            }
        }

        private static float[] Matrix4X4ToFloatArray(Matrix4x4 mat)
        {
            return 
            [
                mat.M11, mat.M12, mat.M13, mat.M14,
                mat.M21, mat.M22, mat.M23, mat.M24,
                mat.M31, mat.M32, mat.M33, mat.M34,
                mat.M41, mat.M42, mat.M43, mat.M44
            ];
        }

        private void OnClose()
        {
            _gl.DeleteVertexArray(_vao);
            _gl.DeleteBuffer(_vbo);
            _shader?.Dispose();
        }

        private static PointCloudData LoadPointCloudData()
        {
            var points = new List<Point3D>();
            var rand = new Random();

            for (int? i = 0; i < 1000; i++)
            {
                points.Add(new Point3D
                {
                    X = (float)(rand.NextDouble() * 100 - 50),
                    Y = (float)(rand.NextDouble() * 100 - 50),
                    Z = (float)(rand.NextDouble() * 100 - 50)
                });
            }

            return new PointCloudData(points);
        }

        private const string VertexShaderSource = """
                                                  
                                                              #version 330 core
                                                              layout (location = 0) in vec3 aPos;
                                                              uniform mat4 projection;
                                                              uniform mat4 view;
                                                              void main()
                                                              {
                                                                  gl_Position = projection * view * vec4(aPos, 1.0);
                                                                  gl_PointSize = 10.0; // Larger point size for sphere effect
                                                              }
                                                  """;

        private const string FragmentShaderSource = """
                                                    
                                                                #version 330 core
                                                                out vec4 FragColor;
                                                                uniform vec3 pointColor;
                                                    
                                                                void main()
                                                                {
                                                                    // Create a smooth circle effect
                                                                    vec2 coord = gl_PointCoord - 0.5;
                                                                    float distance = dot(coord, coord);
                                                                    if (distance > 0.25) discard;
                                                                    
                                                                    // Apply color to the point
                                                                    FragColor = vec4(pointColor, 1.0);
                                                                }
                                                    """;
    }
}
