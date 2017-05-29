using OpenTK;
using GameEngine.Components;
using GameEngine.Objects;
using System.Drawing;

namespace OpenGL_Game.Objects
{
    /* Author: Alex DS */
    class Text : Entity
    {
        private ComponentUIText compText;
        public string TextString
        {
            get { return compText.Text; }
            set { compText.Text = value; }
        }
        /* Author: Alex DS */
        public Text(string text, float textSize, Vector3 pos, bool centered = false)
            : base("Text")
        {
            base.AddComponent(compText = new ComponentUIText(text, new Font("Tahoma", textSize), centered));
            base.AddComponent(new ComponentTransform(pos, new Vector3(0, 0, 0), new Vector3(1,1,1)));
            base.AddComponent(new ComponentGeometry("Geometry/SquareGeometry.txt"));
            base.AddComponent(new ComponentRender("vs_UI", "fs_UI"));
        }
    }
}
