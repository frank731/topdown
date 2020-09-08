﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjacencyCheck : MonoBehaviour
{
    public int needOpening;
    /*
     1 is need down opening
     2 is need top opening
     3 is need right opening
     4 is need left opening
      */
    void Start()
    {
        Destroy(gameObject, 1f);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "RoomSpawnPoint" && collision.name != "replaced")
        {
            RoomController room = collision.transform.parent.gameObject.GetComponent<RoomController>();
            room.AddAdjacencies(needOpening, transform.parent.GetComponent<RoomController>().distance + 1);
            transform.parent.gameObject.GetComponent<RoomController>().adjacentRooms.Add(collision.transform.parent.gameObject);
            Destroy(gameObject, 1f);
        }

    }
}
