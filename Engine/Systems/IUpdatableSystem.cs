namespace GameEngine.Systems
{
    /* Author: Alex DS */
    public interface IUpdateableSystem
    {	
        void OnUpdate(float dt);

        // Property signatures: 
        string Name
        {
            get;
        }
    }
}
