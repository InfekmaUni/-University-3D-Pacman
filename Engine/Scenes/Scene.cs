using GameEngine.Managers;
using System;
using OpenTK;

namespace GameEngine.Scenes
{
    /* Author: Alex DS */
    public abstract class Scene
    {
        protected SceneManager sceneManager;

        public Scene(SceneManager inSceneManager)
        {
            sceneManager = inSceneManager;
        }

        public abstract void Render(FrameEventArgs e);
        public abstract void Update(FrameEventArgs e);
        public abstract void Load(EventArgs e);
    }
}
