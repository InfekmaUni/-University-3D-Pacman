using System.Collections.Generic;
using System.IO;
using GameEngine.Managers;
using OpenGL_Game.Objects;
using OpenTK;

namespace OpenGL_Game.Managers
{
    public struct WaypointNode
    {
        public int index;
        public List<WaypointNode> adjacentnodes;
        public Vector3 position;

        public WaypointNode(Vector3 pos)
        {
            position = pos;
            index = 0;
            adjacentnodes = new List<WaypointNode>();
        }
    }
    class MazeManager
    {
        List<string> maze = new List<string>();
        List<WaypointNode> waypointList = new List<WaypointNode>();
        private int x = -25;
        private int z = -25;
        public void MakeMaze(EntityManager eManager, ref Vector3 pacPos, List<Vector3> ghostPos, ref int count, List<Vector3> wayPoints, int level)
        {

            StreamReader sr = new StreamReader("Mazes/Maze" + level + ".txt");
            while (sr.Peek() != -1)
            {
                string line = sr.ReadLine();
                for (int i = 0; i < line.Length; i++)
                {
                    switch (line[i].ToString())
                    {
                        case "0":
                            pacPos = new Vector3(x+0.5f, -3, z+0.5f);
                            x += 6;
                            break;
                        case "1":
                            eManager.AddEntity(new Wall(new Vector3(x, -3, z)));
                            x += 6;
                            break;
                        case "2":
                            eManager.AddEntity(new EnergyBall(new Vector3(x, -2, z)));
                            x += 6;
                            count++;
                            break;
                        case "3":
                            eManager.AddEntity(new PowerBall(new Vector3(x, -2, z)));
                            wayPoints.Add(new Vector3(x, 0, z));
                            x += 6;
                            break;
                        case "4":
                          
                            ghostPos.Add(new Vector3(x,-3, z));
                            x += 6;
                            break;
                        case "5":
                            eManager.AddEntity(new EnergyBall(new Vector3(x, -2, z)));
                            wayPoints.Add(new Vector3(x, 0, z));
                           // waypointList.Add(new WaypointNode(new Vector3(x, 0, z)));
                            count++;
                            x += 6;
                            break;
                        case "-":
                            x += 6;
                            break;

                    }
                }
                x = -25;
                z += 6;
            }
            sr.Close();
            x = -25;
            x = -25;
        }

        
    }
}