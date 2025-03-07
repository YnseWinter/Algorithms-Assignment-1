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
    public Vector2Int maxGererationSize;
    public int seed;
    public bool fastGeneration;
    [Range(0, .5f)]public float ChanceWeight;


    List<RectInt> rooms = new List<RectInt>() { new RectInt(0, 0, 100, 100) };
    List<RectInt> doors = new List<RectInt>() { new RectInt(0, 0, 0, 0) };
    private RectInt currentRoom;

    private bool verticalDone;
    private bool horizontalDone;
    private bool stopGenerating;
    private bool doneWithRooms;

    Unity.Mathematics.Random rng;

    private void Start()
    {
        rooms = new List<RectInt>() { new RectInt(0, 0, dungeonSize.x, dungeonSize.y) };
    }

    [Button]
    void SplitVertically()
    {
        int biggestRoomIndex = FindBiggestRoom(true);

        currentRoom = rooms[biggestRoomIndex];

        if (currentRoom.width > maxRoomSize.x + 2)
        {
            rooms.RemoveAt(biggestRoomIndex);
            int maxWidth = maxRoomSize.x;
            if (currentRoom.width - minRoomSize.x < maxRoomSize.x)
            {
                maxWidth = currentRoom.width - minRoomSize.x;
            }
            else if (currentRoom.width > maxGererationSize.x + minRoomSize.x)
            {
                maxWidth = maxGererationSize.x;
            }
            RectInt roomA = currentRoom;
            RectInt roomB = currentRoom;
            int randomValue = rng.NextInt(minRoomSize.x + 2, maxWidth);
            if(rng.NextFloat() < 0.5f)
            {
                randomValue = currentRoom.width - randomValue;
            }
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
        int biggestRoomIndex = FindBiggestRoom(false);

        currentRoom = rooms[biggestRoomIndex];

        if (currentRoom.height > maxRoomSize.y + 2)
        {
            int maxHeight = maxRoomSize.y;
            if (currentRoom.height - minRoomSize.y < maxRoomSize.y)
            {
                maxHeight = currentRoom.height - minRoomSize.y;
            }
            else if(currentRoom.height > maxGererationSize.y + minRoomSize.y)
            {
                maxHeight = maxGererationSize.y;
            }
            rooms.RemoveAt(biggestRoomIndex);
            RectInt roomA = currentRoom;
            RectInt roomB = currentRoom;
            int randomValue = rng.NextInt(minRoomSize.y + 2, maxHeight);
            if (rng.NextFloat() < 0.5f)
            {
                randomValue = currentRoom.height - randomValue;
            }
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
        stopGenerating = true;
        rooms = new List<RectInt>() { new RectInt(0, 0, dungeonSize.x, dungeonSize.y) };
    }

    int FindBiggestRoom(bool width)
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
    }

    IEnumerator GenerateDungeon()
    {
        rng = new Random(Convert.ToUInt32(seed));
        float weightedChanceValue = 0;
        while (!doneWithRooms)
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
                doneWithRooms = true;
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
        if (doneWithRooms)
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                for (int j = i + 1; j < rooms.Count; j++)
                {
                    RectInt intersect = AlgorithmsUtils.Intersect(rooms[i], rooms[j]);
                    if(intersect.width > 3)
                    {
                        intersect.x += (intersect.width / 2) - 1;
                        intersect.width = 2;
                        doors.Add(intersect);
                    }
                    else if(intersect.height > 3)
                    {
                        intersect.y += (intersect.height / 2) - 1;
                        intersect.height = 2;
                        doors.Add(intersect);
                    }
                }
            }
        }
    }
}
