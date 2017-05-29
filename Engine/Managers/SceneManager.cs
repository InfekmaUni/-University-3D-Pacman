using GameEngine.Scenes;
using OpenTK;
using OpenTK.Input;
using GameEngine.Systems;
using System;
using OpenTK.Graphics.OpenGL;

namespace GameEngine.Managers
{
    /* Author: Alex DS, Initial Author: Adam */
    public class SceneManager : GameWindow
    {
        Scene scene;
        static int width;
        static int height;
        public delegate void SceneDelegate(FrameEventArgs e);
        public SceneDelegate renderer;
        public SceneDelegate updater;
        public delegate void loadDelegate(EventArgs e);
        public loadDelegate loader;
        public static bool Debug = false;
        EventArgs e = new EventArgs();
        public static int WindowWidth
        {
            get { return width; }
        }
        public static int WindowHeight
        {
            get { return height; }
        }

        public SceneManager(int width, int height)
        {
            SetWindowDimensions(width, height);
        }

        public void SetWindowDimensions(int inWidth, int inHeight)
        {
            base.Width = width = inWidth;
            base.Height = height = inHeight;
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            MousePosition = new Vector2(this.Mouse.X, this.Mouse.Y);
            if (GameKeyboard.KeyPress(Key.F1))
                Debug = !Debug;
            updater(e);
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
            SceneManager.width = Width;
            SceneManager.height = Height;
        }

        public static Vector2 MousePosition;
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            renderer(e);

            GL.Flush();
            SwapBuffers();
        }

        public void NewScene(Scene newScene)
        {
            scene = newScene;
            loader(e);
            GL.Enable(EnableCap.DepthTest);
        }
    }
}
