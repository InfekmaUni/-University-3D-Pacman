using System.Drawing;

namespace GameEngine.Components
{
    /* Author: Alex DS */
    public class ComponentUIText : IComponent
    {
        private string msg = "";
        private Font msgFont;
        public Font Font
        {
            get { return msgFont; }
            set { msgFont = value; }
        }
        public string Text
        {
            get { return msg; }
            set { msg = value; }
        }

        private bool center = false;
        public bool Centered
        {
            get { return center; }
            set { center = value; }
        }
        /* Author: Alex DS */
        public ComponentUIText(string text, Font font, bool centered = false)
        {
            msg = text;
            msgFont = font;
            center = centered;
        }

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_UI_TEXT; }
        }
    }
}