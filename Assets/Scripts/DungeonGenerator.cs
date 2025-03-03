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

    private RectInt currentRoom;

    private bool done;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //StartCoroutine(GenerateDungeon());
    }

    [Button]
    void SplitVertically()
    {
        //for (int i = 0; i < rooms.Count; i++)
        //{
        //    if (rooms[i].width > (minRoomSize.x * 2) + 2)
        //    {
        //        RectInt room = rooms[i];
        //        float maxWidth = maxRoomSize.x;
        //        if (rooms[i].width - minRoomSize.x < maxRoomSize.x)
        //        {
        //            maxWidth = rooms[i].width - minRoomSize.x;
        //        }
        //        rooms.RemoveAt(i);
        //        RectInt roomA = room;
        //        RectInt roomB = room;
        //        int randomValue = (int)Random.Range(minRoomSize.x + 2, maxWidth);
        //        roomA.width = randomValue;
        //        roomB.xMin = randomValue + room.xMin - 1;


        //        rooms.Add(roomA);
        //        rooms.Add(roomB);
        //        break;
        //    }
        //}
        int biggestRoomIndex = FindBiggestRoom(true);

        currentRoom = rooms[biggestRoomIndex];

        if (currentRoom.width > (minRoomSize.x * 2) + 2)
        {
            rooms.RemoveAt(biggestRoomIndex);
            float maxWidth = maxRoomSize.x;
            if (currentRoom.width - minRoomSize.x < maxRoomSize.x)
            {
                maxWidth = currentRoom.width - minRoomSize.x;
            }
            RectInt roomA = currentRoom;
            RectInt roomB = currentRoom;
            int randomValue = (int)Random.Range(minRoomSize.x + 2, maxWidth);
            roomA.width = randomValue;
            roomB.xMin = randomValue + currentRoom.xMin - 1;


            rooms.Add(roomA);
            rooms.Add(roomB);
        }
    }

    [Button]
    void SplitHorizontally()
    {
        //RectInt room = rooms[0];
        //rooms.RemoveAt(0);
        //RectInt roomA = room;
        //RectInt roomB = room;
        //int randomValue = (int)Random.Range(minRoomSize.y, maxRoomSize.y);
        //roomA.height = room.height / 2;
        //roomB.yMin = (room.height / 2) + room.yMin - 1;


        //rooms.Add(roomA);
        //rooms.Add(roomB);

        //for (int i = 0; i < rooms.Count; i++)
        //{
        //    if (rooms[i].height > (minRoomSize.y * 2) + 2)
        //    {
        //        RectInt room = rooms[i];
        //        float maxHeight = maxRoomSize.x;
        //        if (rooms[i].height - minRoomSize.y < maxRoomSize.y)
        //        {
        //            maxHeight = rooms[i].height - minRoomSize.y;
        //        }
        //        rooms.RemoveAt(i);
        //        RectInt roomA = room;
        //        RectInt roomB = room;
        //        int randomValue = (int)Random.Range(minRoomSize.y + 2, maxHeight);
        //        roomA.height = randomValue;
        //        roomB.yMin = randomValue + room.yMin - 1;


        //        rooms.Add(roomA);
        //        rooms.Add(roomB);
        //        break;
        //    }
        //}
        int biggestRoomIndex = FindBiggestRoom(false);

        currentRoom = rooms[biggestRoomIndex];

        if (currentRoom.height > (minRoomSize.y * 2) + 2)
        {
            float maxHeight = maxRoomSize.x;
            if (currentRoom.height - minRoomSize.y < maxRoomSize.y)
            {
                maxHeight = currentRoom.height - minRoomSize.y;
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
    }

    [Button]
    void StartGenerating()
    {
        StartCoroutine(GenerateDungeon());
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

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            AlgorithmsUtils.DebugRectInt(rooms[i], Color.blue);
        }
    }

    IEnumerator GenerateDungeon()
    {
        while (true)
        {
            if (Random.value < .5f)
            {
                SplitVertically();
            }
            else
            {
                SplitHorizontally();
            }
            //yield return new WaitForSeconds(.1f);
            yield return null;
        }


    }
}
