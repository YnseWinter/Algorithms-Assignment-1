using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    List<RectInt> rooms = new List<RectInt>() { new RectInt(0, 0, 100, 50) };
    public int splitCount = 20;
    public Vector2 minRoomSize;
    public Vector2 maxRoomSize;

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
        RectInt room = FindBiggestRoom(true);

        if (room.width > (minRoomSize.x * 2) + 2)
        {
            float maxWidth = maxRoomSize.x;
            if (room.width - minRoomSize.x < maxRoomSize.x)
            {
                maxWidth = room.width - minRoomSize.x;
            }
            RectInt roomA = room;
            RectInt roomB = room;
            int randomValue = (int)Random.Range(minRoomSize.x + 2, maxWidth);
            roomA.width = randomValue;
            roomB.xMin = randomValue + room.xMin - 1;


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

        for (int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i].height > (minRoomSize.y * 2) + 2)
            {
                RectInt room = rooms[i];
                float maxHeight = maxRoomSize.x;
                if (rooms[i].height - minRoomSize.y < maxRoomSize.y)
                {
                    maxHeight = rooms[i].height - minRoomSize.y;
                }
                rooms.RemoveAt(i);
                RectInt roomA = room;
                RectInt roomB = room;
                int randomValue = (int)Random.Range(minRoomSize.y + 2, maxHeight);
                roomA.height = randomValue;
                roomB.yMin = randomValue + room.yMin - 1;


                rooms.Add(roomA);
                rooms.Add(roomB);
                break;
            }
        }
    }

    [Button]
    void StartGenerating()
    {
        StartCoroutine(GenerateDungeon());
    }

    void FindBiggestRoom(bool width)
    {
        RectInt biggestRoom = RectInt.zero;
        int biggestRoomIndex = 0;
        if (width)
        {
            for(int i = 0; i > rooms.Count; i++)
            {
                if (rooms[i].width > biggestRoom.width)
                {
                    biggestRoom = rooms[i];
                    biggestRoomIndex = i;
                }
            }
        }
        else
        {
            for (int i = 0; i > rooms.Count; i++)
            {
                if (rooms[i].height > biggestRoom.height)
                {
                    biggestRoom = rooms[i];
                    biggestRoomIndex = i;
                }
            }
        }
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
            yield return new WaitForSeconds(.1f);
        }


    }
}
