# Unity2D_InfiniteScroller
An 2D infinite scroller project to gain experience with Unity and engineering for intensive real-time games

[Game hosted on itch.io](https://c-and.itch.io/chocolate-mountain)
![alt text](https://github.com/chandrap2/Unity2D_InfiniteScroller/blob/main/Assets/game_Screenshot.png "Screenshot")

## Unity Features
- Sprites for player, terrain and scrolling background
- Player sprite animation
- GameObject colliders
- UI TextMeshPro for game score
- Intro and looping music clips

## Data structures, algorithms, software design
- Procedurally generated terrain,
increasing difficulty implemented with **dynamic cumulative probabilty lookup array**

- **Object pools** of terrain blocks and colliders (preallocated at game start)
for performant object activation/deactivation during gameplay. **Implemented with queues**

- **Linked lists** for tracking active terrain blocks and colliders. Can also be implemented with
**ciruclar lists** to reduce new node allocations during runtime, as terrain object activation/deactivation is frequent when world speed is fast

- Simplified ground colliders to reduce collision checks

- **Player movement states** define states + transition conditions for clarity and maintainability
