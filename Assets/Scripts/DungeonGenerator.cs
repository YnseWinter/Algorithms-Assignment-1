using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;
using System;

public class DungeonGenerator : MonoBehaviour
{
    public Vector2Int dungeonSize;
    public Vector2Int minRoomSize;
    public Vector2Int maxRoomSize;
    public int seed;
    public bool fastGeneration;
    [Range(0, .5f)]public float ChanceWeight;

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

    IEnumerator GenerateDungeon()
    {
        rng = new Random(Convert.ToUInt32(seed));
        float weightedChanceValue = 0;
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

        for (int i = 0; i < rooms.Count; i++)
        {
            for (int j = i + 1; j < rooms.Count; j++)
            {
                RectInt intersect = AlgorithmsUtils.Intersect(rooms[i], rooms[j]);
                if (intersect.width >= 4)
                {
                    intersect.x += (intersect.width / 2) - 1;
                    intersect.width = 2;
                    doors.Add(intersect);
                }
                else if (intersect.height >= 4)
                {
                    intersect.y += (intersect.height / 2) - 1;
                    intersect.height = 2;
                    doors.Add(intersect);
                }

                if (!fastGeneration)
                {
                    yield return null;
                }
            }
        }

        for(int i = 0; i < rooms.Count; i++)
        {
            roomNodes.Add(new Vector2Int((rooms[i].width / 2) + rooms[i].x, (rooms[i].height / 2) + rooms[i].y));
            graph.AddNode(roomNodes[i]);
        }

        for (int i = 0; i < doors.Count; i++)
        {
            doorNodes.Add(new Vector2Int(doors[i].x + 1, doors[i].y + 1));
            graph.AddNode(doorNodes[i]);
        }

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

        //for (int i = 0; i < roomNodes.Count; i++)
        //{
        //    if (graph.GetNeighbors(roomNodes[i]).Count == 0)
        //    {
        //        Debug.Log("Room: " + roomNodes[i] + " is not reachable");
        //    }
        //}

        for(int i = 0; i < rooms.Count; i++)
        {

        }

        doneGenerating = true;
        Debug.Log("Done");
    }
}
