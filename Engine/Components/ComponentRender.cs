namespace GameEngine.Components
{
    /* Author: Alex DS */
    public class ComponentRender : IComponent
    {
        private int rendererID;
        private string vName, fName;
        private bool init = false;

        /* Author: Alex DS */
        public int RendererID
        {
            get { return rendererID; }
            set { rendererID = value; }
        }
        /* Author: Alex DS */
        public string vertexShaderName { get { return vName; } }
        /* Author: Alex DS */
        public string fragmentShaderName { get { return fName; } }
        /* Author: Alex DS */
        public ComponentRender(string iNv, string iNf)
        {
            vName = iNv;
            fName = iNf;
        }
        /* Author: Alex DS */
        public bool IsInit()
        {
            return init;
        }
        /* Author: Alex DS */
        public void Init(int renderID){
            init = true;
            rendererID = renderID;
        }
        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_RENDER; }
        }
    }
}
