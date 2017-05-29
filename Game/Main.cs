#region Using Statements
using GameEngine.Managers;
using GameEngine.Scenes;
using OpenGL_Game.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace OpenGL_Game
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class MainEntry
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (SceneManager game = new SceneManager(1028, 512))
            {
                game.NewScene(new MenuScene(game));
                game.Run(60.0f);
            }
      }
    }
#endif
}
