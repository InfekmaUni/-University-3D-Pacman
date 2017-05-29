using OpenTK;
using GameEngine.Components;
using GameEngine.Objects;
using OpenTK.Input;

namespace OpenGL_Game.Objects
{
    /* Author: Alex DS */
    class Button : Image
    {
        private ComponentUICollision collision;
        private ComponentAudio compAudio;
        string textureName;

        public delegate void MethodToCall();
        public MethodToCall buttonEvent;
        /* Author: Alex DS */
        public Button(string name, Vector2 pos, Vector2 size, string textName)
            : base(name, pos, size, textName)
        {
            base.AddComponent(collision = new ComponentUICollision(size));
          //  base.AddComponent(compAudio = new ComponentAudio("button-press.wav", false, false));
            textureName = textName;

            // set delegates functions from collision component to this object
            collision.hover = OnHover;
            collision.hoverExit = OnHoverExit;
            collision.clicked = OnClicked;
        }

        /* Author: Alex DS */
        /// <summary>
        /// this method is called whenever the mouse is within its ui collision component and the mouse left button state is true
        /// </summary>
        public void OnClicked()
        {
            if (buttonEvent != null)
            { // if clicked and the button event is not null
                buttonEvent(); // call assigned function
                //compAudio.Play(true);
            }
        }

        /* Author: Alex DS */
        /// <summary>
        /// this method is called whenever the mouse is within its ui collision component
        /// </summary>
        public void OnHover()
        {
            texture.ChangeTexture("Textures/UI/" + textureName + "-hover.png");
        }

        /* Author: Alex DS */
        /// <summary>
        /// this method is called whenever the mouse is NOT within the UI collision component
        /// </summary>
        public void OnHoverExit()
        {
            texture.ChangeTexture("Textures/UI/" + textureName + ".png");
        }
    }
}
