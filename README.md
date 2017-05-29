# University-3D-Pacman
3D Pacman Game and Component Engine Development set by Darren McKie of University of Hull

Component based engine developed in OpenGL using glsl shaders, systems and components.

Pacman game developed in C#.

# Engine Features

| Physics                  | UI            |
| -------------            | ------------- |
| Velocity Movement        | Text          |
| Motion (hovering)        | Images        |
| Gravity                  | Minimap       |


Audio System

Collision & Prevention System

Input System - modifies velocity component which in return makes the physics system update that objects movement.

Render System - Handles UI & 3D rendering.

# Game Features
Scene State Manager

Maze manager which creates the maze and is created by reading in the MazeX.txt where X is the level.

Objects used within the game - pacman / ghost / walls / heart lives.

Game Manager.

# Main Menu
![Main Menu](https://raw.githubusercontent.com/InfekmaUni/-University-3D-Pacman/master/Main_Menu.jpg)

# Game Starting Zone
![Start](https://raw.githubusercontent.com/InfekmaUni/-University-3D-Pacman/master/Start.jpg)

# Gameplay - Confronting a Ghost
![Confronting Ghost](https://raw.githubusercontent.com/InfekmaUni/-University-3D-Pacman/master/Gameplay_Confronting_Ghost.jpg)

### Group 7 members:
Alexander Dos Santos - Engine & Game

Adam James Klein-sprokkelhorst - Game

Jack Davis - Maze generation and blender model parser

John Williamson - AI

### KNOWN ISSUES
Horrible sound scratches when changing levels

Models are missing normal, therefore game is black if ran on a AMD CPU, not entirely sure why this is the case I assume that perhapse the nvidia GPU is able to determine normals for undefined geometries.
