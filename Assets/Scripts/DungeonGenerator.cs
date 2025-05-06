using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;
using System;
using System.Runtime.CompilerServices;
using Unity.AI.Navigation;

public class DungeonGenerator : MonoBehaviour
{
    public Vector2Int dungeonSize; 
    public Vector2Int minRoomSize;
    public Vector2Int maxRoomSize;
    public int seed;
    public bool fastGeneration;
    [Range(0, .5f)]public float ChanceWeight;
    public GameObject wall;
    public GameObject floor;
    public NavMeshSurface navMeshSurface;

    List<RectInt> rooms = new List<RectInt>() { new RectInt(0, 0, 100, 100) };
    List<RectInt> doors = new List<RectInt>();
    List<Vector2Int> roomNodes = new List<Vector2Int>();
    List<Vector2Int> doorNodes = new List<Vector2Int>();

    Graph<Vector2Int> graph = new Graph<Vector2Int>();

    private RectInt currentRoom;

    private bool verticalDone;
    private bool horizontalDone;
    private bool doneGenerating;
    private bool stopGenerating;

    Unity.Mathematics.Random rng;

    private void Start()
    {
        rooms = new List<RectInt>() { new RectInt(0, 0, dungeonSize.x, dungeonSize.y) };
    }

    /// <summary>
    /// Splits biggest based on width RectInt vertically at a random distance
    /// </summary>
    [Button]
    void SplitVertically()
    {
        int biggestRoomIndex = FindBiggestRoomIndex(true);

        currentRoom = rooms[biggestRoomIndex];

        if (currentRoom.width > maxRoomSize.x + 2)
        {
            rooms.RemoveAt(biggestRoomIndex);
            RectInt roomA = currentRoom;
            RectInt roomB = currentRoom;

            int maxWidth = currentRoom.width - minRoomSize.x;
            int randomValue = rng.NextInt(minRoomSize.x + 2, maxWidth);
            roomA.width = randomValue;
            roomB.xMin = randomValue + currentRoom.xMin - 1;

            rooms.Add(roomA);
            rooms.Add(roomB);
        }
        else
        {
            verticalDone = true;
        }
    }

    /// <summary>
    /// splits biggest RectInt based on height horizontally at a random distance
    /// </summary>
    [Button]
    void SplitHorizontally()
    {
        int biggestRoomIndex = FindBiggestRoomIndex(false);

        currentRoom = rooms[biggestRoomIndex];

        if (currentRoom.height > maxRoomSize.y + 2)
        {
            rooms.RemoveAt(biggestRoomIndex);
            RectInt roomA = currentRoom;
            RectInt roomB = currentRoom;

            int maxHeight = currentRoom.height - minRoomSize.y;
            int randomValue = rng.NextInt(minRoomSize.y + 2, maxHeight);
            roomA.height = randomValue;
            roomB.yMin = randomValue + currentRoom.yMin - 1;

            rooms.Add(roomA);
            rooms.Add(roomB);
        }
        else
        {
            horizontalDone = true;
        }
    }

    [Button]
    void StartGenerating()
    {
        StartCoroutine(GenerateDungeon());
    }

    [Button]
    void ResetDungeon()
    {
        verticalDone = false;
        horizontalDone = false;
        if (!doneGenerating)
        {
            stopGenerating = true;
        }
        rooms = new List<RectInt>() { new RectInt(0, 0, dungeonSize.x, dungeonSize.y) };
        doors = new List<RectInt>();
        roomNodes = new List<Vector2Int>();
        doorNodes = new List<Vector2Int>();
        graph = new Graph<Vector2Int>();
    }

    /// <summary>
    /// Returns the biggest room's index
    /// </summary>
    /// <param name="width"></param>
    /// <returns></returns>
    int FindBiggestRoomIndex(bool width)
    {
        RectInt biggestRoom = RectInt.zero;
        int biggestRoomIndex = 0;
        if (width)
        {
            for(int i = 0; i < rooms.Count; i++)
            {
                if (rooms[i].width > biggestRoom.width)
                {
                    biggestRoom = rooms[i];
                    biggestRoomIndex = i;
                }
                else if (rooms[i].width == biggestRoom.width)
                {
                    if (rooms[i].height > biggestRoom.height)
                    {
                        biggestRoom = rooms[i];
                        biggestRoomIndex = i;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                if (rooms[i].height > biggestRoom.height)
                {
                    biggestRoom = rooms[i];
                    biggestRoomIndex = i;
                }
                else if (rooms[i].height == biggestRoom.height)
                {
                    if (rooms[i].width > biggestRoom.width)
                    {
                        biggestRoom = rooms[i];
                        biggestRoomIndex = i;
                    }
                }
            }
        }
        return biggestRoomIndex;
    }

    void Update()
    {
        //debugs entire dungeon
        for (int i = 0; i < rooms.Count; i++)
        {
            AlgorithmsUtils.DebugRectInt(rooms[i], Color.blue);
        }
        for (int i = 0; i < doors.Count; i++)
        {
            AlgorithmsUtils.DebugRectInt(doors[i], Color.yellow);
        }
        foreach(Vector2Int room in graph.adjacencyList.Keys)
        {
            foreach(Vector2Int adjecantDoor in graph.GetNeighbors(room))
            {
                Debug.DrawLine(new Vector3(room.x, 0, room.y), new Vector3(adjecantDoor.x, 0, adjecantDoor.y), Color.red);
            }
        }
    }

    /// <summary>
    /// Generates entire dungeon
    /// </summary>
    /// <returns></returns>
    IEnumerator GenerateDungeon()
    {
        rng = new Random(Convert.ToUInt32(seed));
        float weightedChanceValue = 0;  //

        //generates all rooms by splitting the the dungeon randomly, splitting randomly between vertical and horizontal
        while (true)
        {
            if (stopGenerating)
            {
                stopGenerating = false;
                break;
            }
            else if ((rng.NextFloat() + weightedChanceValue < .5f && !verticalDone) || (horizontalDone && !verticalDone))
            {
                SplitVertically();
                weightedChanceValue += ChanceWeight;
            }
            else if (!horizontalDone)
            {
                SplitHorizontally();
                weightedChanceValue -= ChanceWeight;
            }
            else
            {
                break;
            }

            if (fastGeneration)
            {
                yield return null;
            }
            else
            {
                yield return new WaitForSeconds(.2f);
            }
        }

        //calculates doors by finding the intersection of all rooms and determining if and where the door should be placed
        for (int i = 0; i < rooms.Count - 1; i++)
        {
            for (int j = i + 1; j < rooms.Count; j++)
            {
                RectInt intersect = AlgorithmsUtils.Intersect(rooms[i], rooms[j]);
                if (intersect.width >= 4)
                {
                    //intersect.x += (intersect.width / 2) - 1;
                    intersect.x += rng.NextInt(1, (intersect.width - 2));
                    intersect.width = 2;
                    doors.Add(intersect);
                }
                else if (intersect.height >= 4)
                {
                    intersect.y += rng.NextInt(1, (intersect.height - 2));
                    intersect.height = 2;
                    doors.Add(intersect);
                }

                if (!fastGeneration)
                {
                    yield return null;
                }
            }
        }

        //creates graph nodes for the rooms
        for(int i = 0; i < rooms.Count; i++)
        {
            roomNodes.Add(new Vector2Int((rooms[i].width / 2) + rooms[i].x, (rooms[i].height / 2) + rooms[i].y));
            graph.AddNode(roomNodes[i]);
        }

        //creates graph nodes for the doors
        for (int i = 0; i < doors.Count; i++)
        {
            doorNodes.Add(new Vector2Int(doors[i].x + 1, doors[i].y + 1));
            graph.AddNode(doorNodes[i]);
        }

        //adds edges for the graph using the nodes
        for (int i = 0; i < roomNodes.Count; i++)
        {
            for (int j = 0; j < doorNodes.Count; j++)
            {
                if(AlgorithmsUtils.Intersects(rooms[i], doors[j]))
                {
                    graph.AddEdge(roomNodes[i], doorNodes[j]);
                }

                if (!fastGeneration)
                {
                    yield return null;
                }
            }
        }

        //checks if all rooms are connected using the nodes
        if (graph.DFS(roomNodes[0]).Count != roomNodes.Count + doorNodes.Count)
        {
            Debug.Log("Not all rooms are connected");
        }
        else
        {
            Debug.Log("All rooms are connected");
        }

        doneGenerating = true;
        Debug.Log("Done");
    }

    [Button]
    public void SpawnDungeonAssets()
    {
        Dictionary<GameObject, HashSet<Vector3Int>> walls = new();
        Dictionary<GameObject, HashSet<Vector2Int>> floorTiles = new();
        HashSet<Vector3Int> allWalls = new();
        HashSet<Vector2Int> allFloorTiles = new();
        for (int i = 0; i < rooms.Count; i++)
        {
            GameObject room = new GameObject("Room: " + rooms[i].ToString());
            walls.Add(room, new HashSet<Vector3Int>());
            for (int j = 0; j < rooms[i].width; j++)
            {
                if (!allWalls.Contains(new Vector3Int(j + rooms[i].x, 0, rooms[i].y)))
                {
                    allWalls.Add(new Vector3Int(j + rooms[i].x, 0, rooms[i].y));
                    walls[room].Add(new Vector3Int(j + rooms[i].x, 0, rooms[i].y));
                }

                if (!allWalls.Contains(new Vector3Int(j + rooms[i].x, 0, rooms[i].y + rooms[i].height - 1)))
                {
                    allWalls.Add(new Vector3Int(j + rooms[i].x, 0, rooms[i].y + rooms[i].height - 1));
                    walls[room].Add(new Vector3Int(j + rooms[i].x, 0, rooms[i].y + rooms[i].height - 1));
                }
            }
            for (int j = 0; j < rooms[i].height; j++)
            {
                if (!allWalls.Contains(new Vector3Int(rooms[i].x, 0, j + rooms[i].y)))
                {
                    allWalls.Add(new Vector3Int(rooms[i].x, 0, j + rooms[i].y));
                    walls[room].Add(new Vector3Int(rooms[i].x, 0, j + rooms[i].y));
                }

                if (!allWalls.Contains(new Vector3Int(rooms[i].x + rooms[i].width - 1, 0, j + rooms[i].y)))
                {
                    allWalls.Add(new Vector3Int(rooms[i].x + rooms[i].width - 1, 0, j + rooms[i].y));
                    walls[room].Add(new Vector3Int(rooms[i].x + rooms[i].width - 1, 0, j + rooms[i].y));
                }
            }

            floorTiles.Add(room, new HashSet<Vector2Int>());
            foreach (Vector2Int position in rooms[i].allPositionsWithin)
            {
                if (!allFloorTiles.Contains(position))
                {
                    allFloorTiles.Add(position);
                    floorTiles[room].Add(position);
                }
            }
        }

        HashSet<Vector2Int> doorPositions = new();
        foreach(RectInt door in doors)
        {
            foreach (Vector2Int pos in door.allPositionsWithin)
            {
                doorPositions.Add(pos);
            }
        }

        foreach (KeyValuePair<GameObject, HashSet<Vector3Int>> theWall in walls)
        {
            foreach (Vector3Int position in theWall.Value)
            {
                Vector2Int position2D = new(position.x, position.z);

                if (!doorPositions.Contains(position2D))
                {
                    GameObject actualWall = Instantiate(wall, position + new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, theWall.Key.transform);
                    actualWall.name = "Wall: " + position2D.ToString();
                }
            }
        }

        foreach (KeyValuePair<GameObject, HashSet<Vector2Int>> actualFloor in floorTiles)
        {
            foreach (Vector2Int position in actualFloor.Value)
            {
                GameObject theFloor = Instantiate(floor, new Vector3(position.x, 0, position.y) + new Vector3(0.5f, 0, 0.5f), Quaternion.Euler(90, 0, 0), actualFloor.Key.transform);
                theFloor.name = "Floor: " + position.ToString();
            }
        }
    }

    [Button]
    private void BakeNavMesh()
    {
        navMeshSurface.BuildNavMesh();
    }
}
