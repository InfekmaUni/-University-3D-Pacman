using GameEngine.Components;
using GameEngine.Objects;
using OpenTK;
using System.Collections.Generic;
using GameEngine.Managers;
using OpenTK.Input;
using System;

namespace GameEngine.Systems
{

    #region Deprecated
    /*
    public struct SphereCollisionEntity
    {
        public ComponentTransform compTransform;
        public ComponentSphereCollider compSphere;
        public Entity ent;
        public SphereCollisionEntity(Entity e, ComponentTransform trans, ComponentSphereCollider sphere)
        {
            compTransform = trans;
            compSphere = sphere;
            ent = e;
        }
    }

    public struct LineCollisionEntity
    {
        public ComponentTransform compTransform;
        public ComponentBoxCollider compBox;
        public Entity ent;
        public LineCollisionEntity(Entity e, ComponentTransform trans, ComponentBoxCollider line)
        {
            compTransform = trans;
            compBox = line;
            ent = e;
        }
    }*/
    #endregion

    // system author Adam Deere
    public class SystemCollision : BaseSystem, IUpdateableSystem
    {
        // determines whether collisions are checked for
        private bool COLLISIONFLAG = true;
        public void ToggleCollisionFlag()
        {
            COLLISIONFLAG = !COLLISIONFLAG;
        }

        const ComponentTypes UICOLLISIONMASK = (ComponentTypes.COMPONENT_COLLISION_UI | ComponentTypes.COMPONENT_TRANSFORM);
        const ComponentTypes SPHERECOLLISIONMASK = (ComponentTypes.COMPONENT_SPHERE_COLLIDER | ComponentTypes.COMPONENT_TRANSFORM);
        const ComponentTypes BOXCOLLISIONMASK = (ComponentTypes.COMPONENT_BOX_COLLIDER | ComponentTypes.COMPONENT_TRANSFORM);

        List<Entity> sphereEntityColliders = new List<Entity>();
        List<Entity> boxEntityColliders = new List<Entity>();
        List<Entity> UIEntityColliders = new List<Entity>();
        public SystemCollision(List<Entity> entityList) : base("SystemCollision")
        {
            ProcessEntityMask(entityList, sphereEntityColliders, SPHERECOLLISIONMASK);
            ProcessEntityMask(entityList, boxEntityColliders, BOXCOLLISIONMASK);
            ProcessEntityMask(entityList, UIEntityColliders, UICOLLISIONMASK);
        }

        /* Author: Alex DS */
        List<Entity> collisionEntitiesList = new List<Entity>();
        public void AddEntityToCollisionCheckList(Entity ent)
        {
            if(!collisionEntitiesList.Contains(ent))
                collisionEntitiesList.Add(ent);
        }

        /* Author: Alex DS */
        public void OnUpdate(float dt)
        {
            if (SceneManager.Debug)
            {
                if (GameKeyboard.KeyPress(Key.C))
                    ToggleCollisionFlag();
            }

            if (!COLLISIONFLAG) // if false do not check for collisions
                return;

            //remove all entities that have Dispose true flag
            sphereEntityColliders.RemoveAll(en => en.Dispose == true);
            boxEntityColliders.RemoveAll(en => en.Dispose == true);
            collisionEntitiesList.RemoveAll(en => en.Dispose == true);
            UIEntityColliders.RemoveAll(en => en.Dispose == true);

            // for all entities in list
            foreach (Entity ent in collisionEntitiesList)
            {
                ComponentTransform ob1Transform = ((ComponentTransform)ent.GetComponent(ComponentTypes.COMPONENT_TRANSFORM));
                ComponentSphereCollider ob1Collider = ((ComponentSphereCollider)ent.GetComponent(ComponentTypes.COMPONENT_SPHERE_COLLIDER));

                // iterate through all sphere colliders
                foreach (Entity ent2 in sphereEntityColliders)
                {
                    ComponentTransform ob2Transform = ((ComponentTransform)ent2.GetComponent(ComponentTypes.COMPONENT_TRANSFORM));
                    ComponentSphereCollider ob2Collider = ((ComponentSphereCollider)ent2.GetComponent(ComponentTypes.COMPONENT_SPHERE_COLLIDER));
                    if (ent2 != ent)
                    {
                        if (SphereToSphereCollision(ob2Transform, ob2Collider, ob1Transform, ob1Collider))// check sphere 2 sphere collision
                        {
                            ob1Collider.OnCollisionEnter(ob2Collider); // send sphere 1 sphere 2 collider
                            ob2Collider.OnCollisionEnter(ob1Collider);
                        }
                    }
                }

                // iterate through all box colliders
                foreach (Entity ent2 in boxEntityColliders)
                {
                    ComponentTransform ob2Transform = ((ComponentTransform)ent2.GetComponent(ComponentTypes.COMPONENT_TRANSFORM));
                    ComponentBoxCollider ob2Collider = ((ComponentBoxCollider)ent2.GetComponent(ComponentTypes.COMPONENT_BOX_COLLIDER));
                    if (ent2 != ent)
                    {
                        if (SphereToWallCollision(ob2Transform, ob2Collider, ob1Transform, ob1Collider))// check sphere 2 sphere collision
                        {
                            ob1Collider.OnCollisionEnter(ob2Collider); // send sphere 1 sphere 2 collider
                            ob2Collider.OnCollisionEnter(ob1Collider);
                        }
                    }
                }
            }

            // iterate through all ui entities
            foreach (Entity ent in UIEntityColliders)
            {
                ComponentTransform transform = ((ComponentTransform)ent.GetComponent(ComponentTypes.COMPONENT_TRANSFORM));
                ComponentUICollision uiCollision = ((ComponentUICollision)ent.GetComponent(ComponentTypes.COMPONENT_COLLISION_UI));

                Vector2 curMousePos = GameMouse.gameCurPosition; // get current Mouse Pos

                Vector2 position = new Vector2(transform.Position.X, transform.Position.Y);
                Vector2 size = uiCollision.Size;
                bool hover = false;
                bool clicked = GameMouse.MouseClicked;
                if (curMousePos.X < position.X + size.X && curMousePos.X > position.X - size.X &&
                   curMousePos.Y < position.Y + size.Y && curMousePos.Y > position.Y - size.Y)
                {
                    hover = true;
                }

                if (!COLLISIONFLAG)
                    return;

                if (hover && clicked)
                    uiCollision.OnClicked();
                else if (hover)
                    uiCollision.OnHovered();
                else if (!hover)
                    uiCollision.OnHoveredExit();
            }
        }

        public bool SphereToSphereCollision(ComponentTransform ob1Transform, ComponentSphereCollider ob1Collider, ComponentTransform ob2Transform, ComponentSphereCollider ob2Collider ){
            return (ob1Transform.Position - ob2Transform.Position).Length < (ob2Collider.Radius + ob1Collider.Radius).Length;
        }

        public bool SphereToWallCollision(ComponentTransform ob1Transform, ComponentBoxCollider ob1Collider, ComponentTransform ob2Transform, ComponentSphereCollider ob2Collider)
        {
            Vector2 wallLine = ob1Collider.corners.corn1 - ob1Collider.corners.corn2;
            Vector2 pacmanToWall = ob2Transform.Position.Xz - ob1Collider.corners.corn1;
            wallLine.Normalize();
            float scaler = Vector2.Dot(wallLine, pacmanToWall);
            Vector2 thing = wallLine * scaler;
            Vector2 result = ob1Collider.corners.corn1 + thing - ob2Transform.Position.Xz;
            //  float stuff = result.Length;
            if (result.Length < ob2Transform.Scale.X)
            {
                if (ob2Transform.Position.Z < ob1Collider.corners.corn1.Y && ob2Transform.Position.Z > ob1Collider.corners.corn2.Y)
                {
                    return true;
                }
            }

            wallLine = ob1Collider.corners.corn3 - ob1Collider.corners.corn4;
            pacmanToWall = ob2Transform.Position.Xz - ob1Collider.corners.corn3;
            wallLine.Normalize();
            scaler = Vector2.Dot(wallLine, pacmanToWall);
            thing = wallLine * scaler;
            result = ob1Collider.corners.corn3 + thing - ob2Transform.Position.Xz;
            //  float stuff = result.Length;
            if (result.Length < ob2Transform.Scale.X)
            {
                if (ob2Transform.Position.Z < ob1Collider.corners.corn3.Y && ob2Transform.Position.Z > ob1Collider.corners.corn4.Y)
                {
                    return true;
                }
            }

            wallLine = ob1Collider.corners.corn2 - ob1Collider.corners.corn4;
            pacmanToWall = ob2Transform.Position.Xz - ob1Collider.corners.corn2;
            wallLine.Normalize();
            scaler = Vector2.Dot(wallLine, pacmanToWall);
            thing = wallLine * scaler;
            result = ob1Collider.corners.corn2 + thing - ob2Transform.Position.Xz;
            //  float stuff = result.Length;
            if (result.Length < ob2Transform.Scale.X)
            {
                if (ob2Transform.Position.X < ob1Collider.corners.corn2.X && ob2Transform.Position.X > ob1Collider.corners.corn4.X)
                {
                    return true;
                }
            }

            wallLine = ob1Collider.corners.corn3 - ob1Collider.corners.corn4;
            pacmanToWall = ob2Transform.Position.Xz - ob1Collider.corners.corn3;
            wallLine.Normalize();
            scaler = Vector2.Dot(wallLine, pacmanToWall);
            thing = wallLine * scaler;
            result = ob1Collider.corners.corn3 + thing - ob2Transform.Position.Xz;
            //  float stuff = result.Length;
            if (result.Length < ob2Transform.Scale.X)
            {
                if (ob2Transform.Position.Z < ob1Collider.corners.corn3.Y && ob2Transform.Position.Z > ob1Collider.corners.corn4.Y)
                {
                    return true;
                }
            }
            wallLine = ob1Collider.corners.corn1 - ob1Collider.corners.corn3;
            pacmanToWall = ob2Transform.Position.Xz - ob1Collider.corners.corn1;
            wallLine.Normalize();
            scaler = Vector2.Dot(wallLine, pacmanToWall);
            thing = wallLine * scaler;
            result = ob1Collider.corners.corn1 + thing - ob2Transform.Position.Xz;
            //  float stuff = result.Length;
            if (result.Length < ob2Transform.Scale.X)
            {
                if (ob2Transform.Position.X < ob1Collider.corners.corn1.X && ob2Transform.Position.X > ob1Collider.corners.corn3.X)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
