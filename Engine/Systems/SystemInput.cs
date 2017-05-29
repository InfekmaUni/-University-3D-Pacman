using System.Collections.Generic;
using GameEngine.Objects;
using GameEngine.Components;
using GameEngine.Managers;
using OpenTK.Input;
using OpenTK;
using System;

namespace GameEngine.Systems
{
    /* Author: Alex DS */
    // static mouse container
    public static class GameMouse
    {
        #region Game Mouse
        public static Vector2 gameCurPosition = new Vector2(0, 0);
        public static Vector2 gamePrevPosition = new Vector2(0, 0);
        public static Vector2 GameMouseDifference
        {
            get { return gameCurPosition - gamePrevPosition; }
        }
        public static bool IsGameMouseMoving
        {
            get { return gameCurPosition != gamePrevPosition; }
        }
        #endregion

        #region Screen Mouse
        public static Vector2 screenCurPosition = new Vector2(0, 0);
        public static Vector2 screenPrevPosition = new Vector2(0, 0);
        public static Vector2  ScreenMouseDifference
        {
            get { return screenCurPosition - screenPrevPosition; }
        }

        public static bool IsScreenMouseMoving
        {
            get { return screenCurPosition != screenPrevPosition; }
        }

        public static bool curClickedMouse = false;
        public static bool prevClickedMouse = false;
        public static bool MouseClicked
        {
            get { return curClickedMouse && !prevClickedMouse; }
        }
        #endregion
    }

    /* Author: Alex DS */
    // static keyboard container
    public static class GameKeyboard
    {
        public static KeyboardState keyState;
        public static KeyboardState lastKeyState;
        public static bool KeyPress(Key key)
        {
            return keyState.IsKeyDown(key) && lastKeyState.IsKeyUp(key);
        }

        public static bool IsKeyDown(Key key)
        {
            return keyState.IsKeyDown(key);
        }

        public static bool IsKeyUp(Key key)
        {
            return keyState.IsKeyUp(key);
        }
    }
    public class SystemInput : BaseSystem, IUpdateableSystem
    {
        const ComponentTypes INPUTMASK = (ComponentTypes.COMPONENT_TRANSFORM | ComponentTypes.COMPONENT_VELOCITY | ComponentTypes.COMPONENT_INPUT);

        List<Entity> inputEntities = new List<Entity>();
        public SystemInput(List<Entity> entityList)
            : base("SystemInput")
        {
            ProcessEntityMask(entityList, inputEntities, INPUTMASK);
        }

        /* Edited: Alex DS */
        public void MovePlayer(ComponentTransform transform, ComponentCamera camera, ComponentVelocity vel)
        {
            float Movement_Speed = 10;
            Vector3 movement = new Vector3();
 
            #region Keyboard Movement
            if (GameKeyboard.IsKeyDown(Key.W) || GameKeyboard.IsKeyDown(Key.Up)) // set velocity relative to forward vector
            {
                movement += (transform.Forward * Movement_Speed);
            }
            if (GameKeyboard.IsKeyDown(Key.S) || GameKeyboard.IsKeyDown(Key.Down)) // set velocity relative to inverted forward vector
            {
                movement += -(transform.Forward * Movement_Speed);
            }

            if (GameKeyboard.IsKeyDown(Key.A) || GameKeyboard.IsKeyDown(Key.Left)) // set velocity relative to right vector
            {
                movement += (transform.Right * Movement_Speed);
            }
            if (GameKeyboard.IsKeyDown(Key.D) || GameKeyboard.IsKeyDown(Key.Right)) // set velocity relative to inverted right vector
            {
                movement += -(transform.Right * Movement_Speed);
            }

            // debug
            if (SceneManager.Debug) { 
                if (GameKeyboard.IsKeyDown(Key.Q))
                    movement += new Vector3(0, 5, 0);
                if (GameKeyboard.IsKeyDown(Key.E))
                    movement -= new Vector3(0, 5, 0);
            }
            #endregion

            #region Mouse Rotation
            if (GameMouse.IsScreenMouseMoving && mouseLocked) // if mouse is moving
            {
                Vector2 mouseMovement = GameMouse.ScreenMouseDifference; // get mouse change

                transform.Rotation += new Vector3(0, mouseMovement.X/2, 0); // rotate pacmans Y rotation relative to mouse X
            }
            #endregion

            vel.Velocity = movement; // set movement to velocity component
        }

        /* Author: Alex DS */
        private bool mouseLocked = false;
        public bool MouseLock
        {
            get { return mouseLocked; }
            set { mouseLocked = value;  }
        }

        /* Author: Alex DS */
        // method which updates game mouse
        void UpdateGameMouse()
        {
            // get mouse pos
            Vector2 gameMousePos = SceneManager.MousePosition;

            if (GameMouse.gameCurPosition != null) // save last mouse pos
                GameMouse.gamePrevPosition = GameMouse.gameCurPosition;
            GameMouse.gameCurPosition = gameMousePos; // save current mouse pos
        }

        /* Author: Alex DS */
        // methodw hich updates screen mouse
        public void UpdateScreenMouse()
        {
             MouseState mouse = Mouse.GetState();
            Vector2 screenMousePos = new Vector2(mouse.X, mouse.Y);

            if (GameMouse.screenCurPosition != null) // save last mouse pos
                GameMouse.screenPrevPosition = GameMouse.screenCurPosition;
            GameMouse.screenCurPosition = screenMousePos; // save current mouse pos
            if (mouseLocked)// if ctrl is down, do not update
                Mouse.SetPosition(0, 0);// reset mouse position

            // update mouse left click
            if (GameMouse.curClickedMouse != null)
                GameMouse.prevClickedMouse = GameMouse.curClickedMouse;
             GameMouse.curClickedMouse = mouse[MouseButton.Left];
        }

        /* Author: Alex DS */
        // update keyboard static container
        public void UpdateKeyBoard()
        {
            if (GameKeyboard.keyState != null)
                GameKeyboard.lastKeyState = GameKeyboard.keyState;
            GameKeyboard.keyState = Keyboard.GetState();
        }

        /* Author: Alex DS */
        public void OnUpdate(float dt)
        {
            inputEntities.RemoveAll(en => en.Dispose == true);

            UpdateGameMouse();
            UpdateScreenMouse();
            UpdateKeyBoard();

            if (GameKeyboard.KeyPress(Key.LControl))// toggle lock state
                mouseLocked = !mouseLocked;

            foreach(Entity ent in inputEntities){
                ComponentTransform transform = (ComponentTransform)ent.GetComponent(ComponentTypes.COMPONENT_TRANSFORM);
                ComponentCamera camera = (ComponentCamera)ent.GetComponent(ComponentTypes.COMPONENT_CAMERA);
                ComponentVelocity vel = (ComponentVelocity)ent.GetComponent(ComponentTypes.COMPONENT_VELOCITY);

                MovePlayer(transform, camera, vel);
            }
        }
    }
}