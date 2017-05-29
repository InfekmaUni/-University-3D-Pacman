namespace GameEngine.Systems
{
    /* Author: Alex DS */
    public interface IRenderableSystem
    {
        void OnRender();

        // Property signatures: 
        string Name
        {
            get;
        }
    }
}
