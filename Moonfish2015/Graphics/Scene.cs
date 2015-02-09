using OpenTK.Graphics.ES30;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Moonfish.Graphics
{
    public class Scene
    {
        public Performance Performance { get; private set; }
        public MeshManager ObjectManager { get; set; }
        Dictionary<string, Program> Shaders { get; set; }
        Stopwatch Timer { get; set; }
        public Camera Camera { get; set; }

        public event EventHandler OnFrameReady;


        CoordinateGrid Grid;

        public Scene()
        {
            Initialize();
        }

        private void LoadDefaultShader()
        {
            Program defaultProgram;
            var vertex_shader = new Shader("data/vertex.vert", ShaderType.VertexShader);
            var fragment_shader = new Shader("data/fragment.frag", ShaderType.FragmentShader);
            defaultProgram = new Program("shaded"); OpenGL.ReportError();
            GL.BindAttribLocation(defaultProgram.ID, 0, "position"); OpenGL.ReportError();
            GL.BindAttribLocation(defaultProgram.ID, 1, "texcoord"); OpenGL.ReportError();
            GL.BindAttribLocation(defaultProgram.ID, 2, "compressedNormal"); OpenGL.ReportError();
            GL.BindAttribLocation(defaultProgram.ID, 3, "colour"); OpenGL.ReportError();
            defaultProgram.Link(new List<Shader>(2) { vertex_shader, fragment_shader }); OpenGL.ReportError();
            Shaders["default"] = defaultProgram;

            Shaders["default"]["LightPositionUniform"] = new OpenTK.Vector3(1, 1, 1);

        }

        public virtual void Initialize()
        {
            Console.WriteLine(GL.GetString(StringName.Version));
            Timer = new Stopwatch();
            Camera = new Camera();
            Shaders = new Dictionary<string, Program>();
            ObjectManager = new MeshManager();
            Performance = new Performance();
            Grid = new CoordinateGrid();

            LoadDefaultShader();

            Camera.ViewProjectionMatrixChanged += Camera_ViewProjectionMatrixChanged;
            Camera.ViewMatrixChanged += Camera_ViewMatrixChanged;
            Camera.Viewport.ViewportChanged += Viewport_ViewportChanged;

            OpenGL.ReportError();
            GL.ClearColor(Colours.Green);
            OpenGL.ReportError();
            GL.FrontFace(FrontFaceDirection.Ccw);
            OpenGL.ReportError();
            GL.Enable(EnableCap.CullFace);
            OpenGL.ReportError();
            GL.Enable(EnableCap.DepthTest);
            OpenGL.ReportError();
        }

        void Viewport_ViewportChanged(object sender, Viewport.ViewportEventArgs e)
        {
            GL.Viewport(0, 0, e.Viewport.Width, e.Viewport.Height);
        }

        void Camera_ViewMatrixChanged(object sender, MatrixChangedEventArgs e)
        {
            foreach (var program in Shaders)
                program.Value["viewMatrix"] = e.Matrix;
        }

        void Camera_ViewProjectionMatrixChanged(object sender, MatrixChangedEventArgs e)
        {
            foreach (var program in Shaders)
                program.Value["viewProjectionMatrix"] = e.Matrix;
        }

        public virtual void RenderFrame()
        {
            //Console.WriteLine("RenderFrame()");
            BeginFrame();
            Draw(Performance.Delta);
            EndFrame();
        }

        private void EndFrame()
        {
            //Console.WriteLine("EndFrame()");
            GL.Finish();
            Performance.EndFrame();
            if (OnFrameReady != null) OnFrameReady(this, new EventArgs());
        }

        private void BeginFrame()
        {
            //Console.WriteLine("BeginFrame()");
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Performance.BeginFrame();
        }

        public virtual void Draw(float delta)
        {
            //Console.WriteLine("Draw()");

            ObjectManager.Draw(Shaders["default"]);
        }

        public virtual void Update()
        {
            //Console.WriteLine("Update()");
            Camera.Update();
        }
    };
}
