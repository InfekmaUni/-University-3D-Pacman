using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Audio;
using OpenTK.Input;
using GameEngine.Components;
using GameEngine.Systems;
using GameEngine.Managers;
using GameEngine.Scenes;
using OpenGL_Game.Managers;
using GameEngine.Objects;
using OpenGL_Game.Objects;
using OpenGL_Game.Scenes;
using OpenGL_Game.Objects.In_Game_Objects;

namespace OpenGL_Game
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
     class MyGame : Scene
    {
        public  Matrix4 view, projection;
        EntityManager entityManager;
        List<Entity> entityList;
        bool collisionFlag = true;
        private SystemRender renderSystem;
        private SystemInput inputSystem;
        private SystemCollision collisionSystem;
        private int count;
        private int vecIndex;
        SystemManager systemManager;
        MazeManager maze;
        Pacman pacman;
        Vector3 pacmanPos;
        public static List<Vector3> Waypoints = new List<Vector3>();
        public static List<Vector3> ghostPos = new List<Vector3>();
        public static MyGame gameInstance;
        public static float dt = 0;
        public static float pt = 10;
        public static bool collected = false;
        public static int score = 0;
        public static int lives = 3;
        public static int level = 1;

        public List<Entity> Ghost = new List<Entity>();
        public enum PacmanState
        {
            Frenzy,
            Dead,
            Default
        }
        public static PacmanState pacState = PacmanState.Default;

        public MyGame(SceneManager sceneManager, int inLevel = 1, int inScore = 0, int inLives = 3)
            : base(sceneManager)
        {
            score = inScore;
            level = inLevel;
            lives = inLives;
            sceneManager.Title = "Group 7 - 3D Pacman";
            sceneManager.loader = Load;
            sceneManager.renderer = Render;
            sceneManager.updater = Update;
            gameInstance = this;
            entityManager = new EntityManager();
            systemManager = new SystemManager();
            maze = new MazeManager();
            AudioContext AC = new AudioContext();
        }

        public void NextLevel(EventArgs e)
        {
            level++;
            sceneManager.NewScene(new TransitionScene(sceneManager, level, score, lives));
        }
       public void GameOver()
        {
            sceneManager.NewScene(new GameOverScene(sceneManager, score));
        }
        public void Died()
        {

        }

         public void CheckPacman(float dt){
             player.Update(dt);
            pacmanPos = player.Position;
         }

         Matrix4 gameProjection;
         public override void Load(EventArgs e)
        {
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

            gameProjection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), sceneManager.Width / sceneManager.Height, 0.01f, 100f);
           
            entityList = entityManager.Entities();

            CreateEntities();
            CreateSystems();
        }

         public override void Render(FrameEventArgs e)
        {
            GL.Viewport(0, 0, sceneManager.Width, sceneManager.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            systemManager.RenderSystems(entityManager);
            GL.Flush();
            //    SwapBuffers();
        }

         public override void Update(FrameEventArgs e)
        {
            dt = (float)e.Time;
            if (collected == true)
            {
                pt -= (float)e.Time;
                if (pt <= 0)
                {
                    collected = false;
                    pacState = PacmanState.Default;
                    pt = 10;
                    Pacman temp = (Pacman)player;
                    temp.Frenzy = (pacState == PacmanState.Frenzy);
                }
            }
            if (SceneManager.Debug)
            {
                if (GameKeyboard.IsKeyDown(Key.P))
                { // to test the energyball sound currently
                    NextLevel(e);
                }
                if (GameKeyboard.IsKeyDown(Key.L))
                    sceneManager.NewScene(new GameOverScene(sceneManager, score));

                if (GameKeyboard.IsKeyDown(Key.N))
                    count = 0;

                if (GameKeyboard.IsKeyDown(Key.Escape))
                {
                    sceneManager.Exit();
                }
            }
            if (count <= 0)
            {
                sceneManager.NewScene(new TransitionScene(sceneManager, level, score, lives));
            }
            systemManager.UpdateSystems(entityManager, (float)e.Time);
            SetCameraToPacman();
            CheckBallEntities();
            CheckPacman(dt);

            foreach(Entity ghosty in Ghost)
            {          
                GhostBall TempGhosty = (GhostBall)ghosty;
                Vector3 pacPos = ((ComponentTransform)player.GetComponent(ComponentTypes.COMPONENT_TRANSFORM)).Position;
                TempGhosty.MoveGhost(pacPos);
            }
            energyText.TextString = "Energy Left: " + count;
            scoreText.TextString = "Score: " + score;
        }

        /* Author: Alex DS */
        #region PowerBall & Energyball collided check, handles power and energy state
        /// <summary>
         /// this method iterates through the entity list and checks whether the entity is a powerball or energyball and re-directs them
         /// </summary>
        public void CheckBallEntities()
        {
            for (int i = 0; i < entityList.Count-1; i++)
            {
                string entName = entityList[i].name;
                if (entName.Contains("PowerBall"))
                    CheckPowerBall(i);
                else if (entName.Contains("EnergyBall"))
                    CheckEnergyBall(i);
                else if (entName.Contains("GhostBall"))
                    CheckGhosts(i);
            }
        }

         /// <summary>
         /// this method checks if the powerball was collided with, if true we remove the ball from the list and set the power mode
         /// </summary>
         /// <param name="index">index of entity in the list</param>
         
        public void CheckGhosts(int index)
        {
            GhostBall ghost = entityList[index] as GhostBall;
           if (ghost.collided)
            {
                if (pacState == PacmanState.Default)
                {
                    lives--;
                    if (lives > 0)
                    {
                        hearts[hearts.Count - 1].Dispose = true;
                        hearts.RemoveAt(hearts.Count - 1);
                    }
                    ((ComponentTransform)player.GetComponent(ComponentTypes.COMPONENT_TRANSFORM)).Position = pacmanPos;
                    for (int i = 0; i < entityList.Count; i++)
                    {
                        if (entityList[i].Name.Contains("GhostBall"))
                        {
                           ((ComponentTransform)entityList[i].GetComponent(ComponentTypes.COMPONENT_TRANSFORM)).Position = ghostPos[vecIndex];
                            vecIndex++;
                        }
                    }
                    vecIndex = 0;
                    ghost.collided = false;
                    if (lives <= 0)
                    {
                        GameOver();
                    }
                }
                else if (pacState == PacmanState.Frenzy)
                {
                    score += 500;
                    ((ComponentTransform)ghost.GetComponent(ComponentTypes.COMPONENT_TRANSFORM)).Position = ghostPos[vecIndex];
                }
            }
        }
        public void CheckPowerBall(int index)
        {
            PowerBall ball = entityList[index] as PowerBall;
            if (ball.collided) // collision occured
            {
                entityList[index].Dispose = true; // set entity to null
                entityList.RemoveAt(index); // remove from list

                // set power mode
                collected = true;
                pacState = PacmanState.Frenzy;
                Pacman temp = (Pacman)player;
                temp.Frenzy = (pacState == PacmanState.Frenzy);
            }
        }

        /// <summary>
        /// this method checks if the energyball was collided with, if true we remove the ball from the list and increment score;
        /// </summary>
        /// <param name="index">index of entity in the list</param>
        public void CheckEnergyBall(int index)
        {
            EnergyBall ball = entityList[index] as EnergyBall;
            if (ball.collided) // collision occured
            {
                score += 100;
                count--;
                entityList[index].Dispose = true; // set entity to null
                entityList.RemoveAt(index); // remove from list
            }
        }
        #endregion

        private Vector3 mapSize = new Vector3(150, 150,1);
        private EnergyBall testEnergyBall;
        private PowerBall testPowerBall;
        private Text energyText;
        private Text scoreText;
        private void CreateEntities()
        {
            entityManager.AddEntity(energyText = new Text("Energy Left: "+count, 6, new Vector3(20, 20, 0)));
            entityManager.AddEntity(scoreText = new Text("Current Score: " + score, 6, new Vector3(20, 40, 0)));
            entityManager.AddEntity(new Text("Group 7", 6, new Vector3(sceneManager.Width - 20, sceneManager.Height - 20, 0)));
            entityManager.AddEntity(new UIScreen(new Vector2(sceneManager.Width, sceneManager.Height), true));
            entityManager.AddEntity(new PrimitiveGeometry("object", new Vector3(0,0,0), new Vector3(0,70,0), new Vector3(0,0,0), "Triangle", "spaceship.png"));

          
            entityManager.AddEntity(floor = new PrimitiveGeometry("Floor", new Vector3(0, -2, 0), new Vector3(-90, 0, 0), mapSize, "Square", "floor_placeholder.png", 10));


            entityManager.AddEntity(new PrimitiveGeometry("Skybox", new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(200, 10, 200), "Cube", "sky.png"));

            maze.MakeMaze(entityManager,ref pacmanPos, ghostPos, ref count, Waypoints, level);
          
            
            entityManager.AddEntity(player = new Pacman(pacmanPos, gameProjection, false));

            entityManager.AddEntity(new GhostBall(new Vector3(ghostPos[0])));
            Ghost.Add(entityList[entityList.Count - 1]);
            entityManager.AddEntity(new GhostBall(new Vector3(ghostPos[1])));
            Ghost.Add(entityList[entityList.Count - 1]);
            entityManager.AddEntity(new GhostBall(new Vector3(ghostPos[2])));
            Ghost.Add(entityList[entityList.Count - 1]);
            entityManager.AddEntity(new GhostBall(new Vector3(ghostPos[3])));
            Ghost.Add(entityList[entityList.Count - 1]);

            CreateHearts();
        }

        List<Entity> hearts = new List<Entity>();
        private void CreateHearts()
        {
            int x_space = 10, y_space = 10;
            int x_size = 25, y_size = 25;
            int bottom = SceneManager.WindowHeight;
            for (int i = 1; i <= lives; i++)
            {
                Vector2 position = new Vector2((2 * x_size * i) + (x_space * i) - x_size, bottom-y_size - y_space);
                hearts.Add(entityManager.AddEntity(new Image("UI-Heart", position, new Vector2(x_size, y_size), "Heart")));
            }
        }

         /* Author: Alex DS */
        private Pacman player;
        private Entity floor;
        private ComponentCamera cam;
        private ComponentTransform playerTransform;
        private void SetCameraToPacman()
        {
            cam = (ComponentCamera)player.GetComponent(ComponentTypes.COMPONENT_CAMERA);
            playerTransform = (ComponentTransform)player.GetComponent(ComponentTypes.COMPONENT_TRANSFORM);
            // set camera behind the player transform based on its inverted forward vector
            // after setting position, look at the sphere
            if(cam != null)
                 cam.View = Matrix4.LookAt(playerTransform.ModelSpace.ExtractTranslation() + (-playerTransform.Forward/100)+ new Vector3(0,3,0), playerTransform.ModelSpace.ExtractTranslation() + new Vector3(0, 3, 0), playerTransform.Up);
        }

        private void CreateSystems()
        {
            IRenderableSystem irenderSystem;
            IUpdateableSystem iupdateSystem;

            irenderSystem = new SystemRender(entityList);
            systemManager.AddRenderSystem(renderSystem=(SystemRender)irenderSystem);
            iupdateSystem = new SystemPhysics(entityList);
            systemManager.AddUpdateSystem(iupdateSystem);
            iupdateSystem = new SystemAudio(entityList);
            systemManager.AddUpdateSystem(iupdateSystem);
            iupdateSystem = new SystemInput(entityList);
            systemManager.AddUpdateSystem(inputSystem = (SystemInput)iupdateSystem);
            inputSystem.MouseLock = true; // sets mouse lock
            iupdateSystem = new SystemCollision(entityList);
            systemManager.AddUpdateSystem(collisionSystem = (SystemCollision)iupdateSystem);
            collisionSystem.AddEntityToCollisionCheckList(player);
        }
    }
}
