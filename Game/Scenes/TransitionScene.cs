using System;
using System.Collections.Generic;
using OpenTK;
using OpenGL_Game.Managers;
using OpenTK.Graphics.OpenGL;
using GameEngine.Objects;
using GameEngine.Components;
using GameEngine.Systems;
using GameEngine.Managers;
using GameEngine.Scenes;
using OpenGL_Game.Objects;
using OpenTK.Input;

namespace OpenGL_Game.Scenes
{
    
    class TransitionScene : Scene
    {
        EntityManager entityManager;
        SystemManager systemManager;
        SystemCollision systemCollision;
        Pacman player;
        public static TransitionScene menuInstance;
        EventArgs e;
        public int lives;
        public int level;
        int total_score = 0;
        public TransitionScene(SceneManager sceneManager, int inLevel, int score, int inLives)
            : base(sceneManager)
        {
            sceneManager.Title = "Transition Scene";
            sceneManager.loader = Load;
            sceneManager.renderer = Render;
            sceneManager.updater = Update;
            entityManager = new EntityManager();
            systemManager = new SystemManager();
            menuInstance = this;
            e = new EventArgs();
            total_score = score;
            lives = inLives;
            level = inLevel;
        }

        List<IRenderableSystem> renderSystems = new List<IRenderableSystem>();
        List<IUpdateableSystem> updateSystems = new List<IUpdateableSystem>();
        private void CreateSystems()
        {
            IRenderableSystem newRenderSystem;
            IUpdateableSystem newUpdateSystem;

            newRenderSystem = new SystemRender(entityManager.Entities());
            systemManager.AddRenderSystem(newRenderSystem);
            newUpdateSystem = new SystemCollision(entityManager.Entities());
            systemManager.AddUpdateSystem(systemCollision = (SystemCollision)newUpdateSystem);
            systemManager.AddUpdateSystem(new SystemInput(entityManager.Entities()));
        }
        private void CreateEntities()
        {
            entityManager.AddEntity(new UIScreen(new Vector2(SceneManager.WindowWidth, SceneManager.WindowHeight)));
            CreateMenuButtons();
        }

        private void CreateMenuButtons()
        {
            Vector2 center = new Vector2(SceneManager.WindowWidth / 2, SceneManager.WindowHeight / 2);
            Vector2 buttonSize = new Vector2(75, 25);
            Vector2 offset = new Vector2(0, buttonSize.Y * 3);

            Button but = new Button("UI-Play", center - (buttonSize / 2), buttonSize, "NextLevel");
            but.buttonEvent = NextLevel;
            buttons.Add(but);
            entityManager.AddEntity(but);

            //but = new Button("UI-Exit", center - (buttonSize / 2) + offset, buttonSize, "Exit");
            //but.buttonEvent = CloseGame;
            //buttons.Add(but);
            //entityManager.AddEntity(but);

            entityManager.AddEntity(new Text("Next Level - score " + total_score + " lives left " + lives, 16, new Vector3(center.X, 20, 0)));
            entityManager.AddEntity(new Text("Group 7", 6, new Vector3(sceneManager.Width - 20, sceneManager.Height - 20, 0)));


            Image newImage;
            entityManager.AddEntity(newImage = new Image("UI-Background", new Vector2(0, 0), new Vector2(sceneManager.Width, sceneManager.Height), "menu-background"));
            newImage.TextScale = 4;
        }

        public override void Load(EventArgs e)
        {
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            // view = Matrix4.LookAt(new Vector3(0, 0, 3), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            //projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), SceneManager.WindowWidth / SceneManager.WindowHeight, 0.01f, 100f);

            CreateEntities();
            CreateSystems();
        }

        public override void Render(FrameEventArgs e)
        {
            GL.Viewport(0, 0, sceneManager.Width, sceneManager.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            systemManager.RenderSystems(entityManager);

            GL.Flush();
        }

        List<Button> buttons = new List<Button>();
        public override void Update(FrameEventArgs e)
        {
            float dt = (float)e.Time;
            systemManager.UpdateSystems(entityManager, dt);
        }
        public void NextLevel()
        {
            sceneManager.NewScene(new MyGame(sceneManager, level, total_score, lives));
        }
        public void CloseGame()
        {
            sceneManager.Exit();
        }
    }
}
