using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    List<RectInt> rooms = new List<RectInt>() { new RectInt(0, 0, 100, 100) };
    public int splitCount = 20;
    public Vector2 minRoomSize;
    public Vector2 maxRoomSize;
    public Vector2 maxGererationSize;

    private RectInt currentRoom;

    private bool verticalDone;
    private bool horizontalDone;
    [Button]
    void SplitVertically()
    {
        int biggestRoomIndex = FindBiggestRoom(true);

        currentRoom = rooms[biggestRoomIndex];

        if (currentRoom.width > maxRoomSize.x + 2)
        {
            rooms.RemoveAt(biggestRoomIndex);
            float maxWidth = maxRoomSize.x;
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
            int randomValue = (int)Random.Range(minRoomSize.x + 2, maxWidth);
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
            float maxHeight = maxRoomSize.y;
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
            int randomValue = (int)Random.Range(minRoomSize.y + 2, maxHeight);
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
        rooms = new List<RectInt>() { new RectInt(0, 0, 100, 100) };
        verticalDone = false;
        horizontalDone = false;
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
    }

    IEnumerator GenerateDungeon()
    {
        float weightedChanceValue = 0;
        while (true)
        {
            if ((Random.value + weightedChanceValue < .5f && !verticalDone) || (horizontalDone && !verticalDone))
            {
                SplitVertically();
                weightedChanceValue += .2f;
            }
            else if (!horizontalDone)
            {
                SplitHorizontally();
                weightedChanceValue -= .2f;
            }
            else
            {
                break;
            }
            //yield return new WaitForSeconds(.2f);
            yield return null;
        }
    }
}
