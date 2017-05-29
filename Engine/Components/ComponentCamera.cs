using OpenTK;

namespace GameEngine.Components
{
    /* Author: Alex DS */
    public class ComponentCamera : IComponent
    {
        private Matrix4 view;
        private Matrix4 projection;
        public ComponentCamera(Vector3 pos, Matrix4 proj)
        {
            view = Matrix4.CreateTranslation(pos);
            projection = proj;
        }

        public Matrix4 Projection
        {
            get { return projection; }
            set { projection = value; }
        }
        public Matrix4 View
        {
            get { return view; }
            set { view = value; }
        }

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_CAMERA; }
        }
    }
}
