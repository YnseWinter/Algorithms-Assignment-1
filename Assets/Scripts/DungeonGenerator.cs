using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    List<RectInt> rooms = new List<RectInt>() { new RectInt(0, 0, 100, 50) };
    public int splitCount = 20;
    public Vector2 minRoomSize;
    public Vector2 maxRoomSize;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(GenerateDungeon());
    }

    [Button]
    void SplitVertically()
    {
        RectInt room = rooms[0];
        rooms.RemoveAt(0);
        RectInt roomA = room;
        RectInt roomB = room;
        roomA.width = room.width / 2;
        roomB.xMin = (room.width / 2) + room.xMin - 1;


        rooms.Add(roomA);
        rooms.Add(roomB);
    }

    [Button]
    void SplitHorizontally()
    {
        RectInt room = rooms[0];
        rooms.RemoveAt(0);
        RectInt roomA = room;
        RectInt roomB = room;
        roomA.height = room.height / 2;
        roomB.yMin = (room.height / 2) + room.yMin - 1;


        rooms.Add(roomA);
        rooms.Add(roomB);
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
        while(splitCount > 0)
        {
            if(Random.value < .5f)
            {
                SplitVertically();
            }
            else
            {
                SplitHorizontally();
            }
            yield return new WaitForSeconds(.2f);
            splitCount--;
        }
    }
}
