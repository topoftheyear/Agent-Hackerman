using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentBehavior : MonoBehaviour
{
    public GameObject currentRoom;
    GameMaster gm;
    Queue<GameObject> path;
    GameObject nextPosition;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameMaster").GetComponent<GameMaster>();
        path = new Queue<GameObject>();
        nextPosition = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (path.Count > 0 && nextPosition == null)
        {
            nextPosition = path.Peek();
        }

        if (nextPosition != null)
        {
            transform.position = Vector3.Lerp(transform.position, nextPosition.transform.position, 0.2f);

            if (transform.position == nextPosition.transform.position)
            {
                // Close doors
                if (nextPosition.name.Contains("Door"))
                {
                    nextPosition.GetComponent<DoorObject>().state = "close";
                }

                // Reset next position
                path.Dequeue();
                nextPosition = null;
            }
        }

        // Check for open doors in current room
        if (path.Count == 0)
        {
            List<GameObject> openDoors = new List<GameObject>();
            foreach (GameObject door in currentRoom.GetComponent<RoomObject>().doors.Keys)
            {
                if (door.GetComponent<DoorObject>().state == "open" && !openDoors.Contains(door))
                {
                    openDoors.Add(door);
                }
            }

            // Pick random open door from list
            if (openDoors.Count > 0)
            {
                var pickedDoor = openDoors[Random.Range(0, openDoors.Count)];

                // Add door locations to queue
                path.Enqueue(pickedDoor);

                // Add next room's agent location to queue
                var nextRoom = currentRoom.GetComponent<RoomObject>().doors[pickedDoor];
                foreach (Transform c in nextRoom.transform)
                {
                    GameObject child = c.gameObject;

                    if (!child.name.Contains("AgentLocation"))
                    {
                        continue;
                    }

                    path.Enqueue(child);
                    break;
                }

                currentRoom = nextRoom;
            }
        }
    }
}
