using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    public GameObject endRoom;

    public List<GameObject> rooms;

    public GameObject agent;
    
    // Start is called before the first frame update
    void Start()
    {
        rooms = new List<GameObject>();

        IEnumerator worldmethod = GenerateWorld();
        StartCoroutine(worldmethod);
    }

    // Update is called once per frame
    void Update()
    {
        var currentRoom = agent.GetComponent<AgentBehavior>().currentRoom;
        currentRoom.GetComponent<RoomObject>().EnableThings();

        var hn = GameObject.Find("GameMaster/Canvas/HackName");
        hn.GetComponent<Text>().text = "";

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            var other = hit.collider.gameObject;
            if (other.name.Contains("Door"))
            {
                hn.GetComponent<Text>().text = other.GetComponent<DoorObject>().hackName;
                hn.transform.position = Input.mousePosition + new Vector3(0, 20, 0);
            }
        }

        if (currentRoom == endRoom)
        {
            print("Yay win");
        }

        // Auto select input box
        var inputbox = GameObject.Find("GameMaster/Canvas/InputField");
        inputbox.GetComponent<InputField>().Select();
        inputbox.GetComponent<InputField>().ActivateInputField();

        if (Input.GetKey(KeyCode.Return))
        {
            var textInput = inputbox.GetComponent<InputField>();
            var text = textInput.text.ToLower();
            textInput.text = "";
            if (text != "")
            {
                HandleText(text);
            }
        }
    }

    void HandleText(string text)
    {
        string[] statements = text.Split('.');

        if (statements.Length == 2)
        {
            string otherName = statements[0];
            string command = statements[1];

            // Find object
            HackableObject objectInQuestion = null;

            foreach (HackableObject thing in GameObject.FindObjectsOfType(typeof(HackableObject)))
            {
                if (thing.hackName.Equals(otherName))
                {
                    objectInQuestion = thing;
                    break;
                }
            }

            // Set object's state
            if (objectInQuestion != null)
            {
                objectInQuestion.state = command;
            }
        }
    }

    IEnumerator GenerateWorld()
    {
        int numRooms = 1;
        int maxRooms = 6;

        var currentWorkingRooms = new List<GameObject>();
        List<(int, int)> takenDoorPosition = new List<(int, int)>();

        var firstRoom = Instantiate((GameObject)Resources.Load("Prefabs/Room"));
        firstRoom.transform.position = new Vector3(0, 0, 0);
        firstRoom.name = "Room000";
        currentWorkingRooms.Add(firstRoom);

        while (numRooms < maxRooms)
        {
            var nextWorkingRooms = new List<GameObject>();
            foreach (GameObject room in currentWorkingRooms)
            {
                // go through every possible exit location in a room
                foreach (Transform ct in room.transform)
                {
                    var child = ct.gameObject;

                    // Verify it is a temp door position
                    if (!child.name.Contains("TempDoor"))
                    {
                        continue;
                    }

                    // Delete if door already in place
                    if (takenDoorPosition.Contains(((int)child.transform.position.x, (int)child.transform.position.z)))
                    {
                        Destroy(child);
                        continue;
                    }

                    // Check if rooms will be connected
                    if (OddsChecking())
                    {
                        numRooms++;

                        // Add a new door
                        GameObject newDoor = Instantiate((GameObject)Resources.Load("Prefabs/Door"));
                        newDoor.transform.position = child.transform.position;
                        newDoor.transform.rotation = child.transform.rotation;

                        takenDoorPosition.Add(((int)newDoor.transform.position.x, (int)newDoor.transform.position.z));

                        // Add a new room if one current doesn't exist
                        Vector3 pos = room.transform.position + new Vector3(child.GetComponent<TempDoorInfo>().x * 4, 0, child.GetComponent<TempDoorInfo>().z * 4);
                        GameObject newRoom = GameObject.Find("Room" + pos.x + pos.y + pos.z);
                        if (newRoom == null)
                        {
                            newRoom = Instantiate((GameObject)Resources.Load("Prefabs/Room"));
                            newRoom.transform.position = pos;
                            newRoom.name = "Room" + pos.x + pos.y + pos.z;
                            nextWorkingRooms.Add(newRoom);
                        }

                        // Connect the rooms to eachother through the door
                        room.GetComponent<RoomObject>().doors[newDoor] = newRoom;
                        newRoom.GetComponent<RoomObject>().doors[newDoor] = room;

                        //newRoom.GetComponent<RoomObject>().DisableThings();

                        // Delete temp doors
                        Destroy(child);
                    }
                }

                endRoom = room;
            }

            if (nextWorkingRooms.Count != 0)
            {
                currentWorkingRooms = nextWorkingRooms;
            }
        }

        // Create Agent
        agent = Instantiate((GameObject)Resources.Load("Prefabs/Agent"));
        agent.transform.position = firstRoom.transform.position + new Vector3(0, 0.25f, 0);
        agent.GetComponent<AgentBehavior>().currentRoom = firstRoom;

        GameObject cam = GameObject.Find("Main Camera");
        cam.transform.SetParent(agent.transform);

        yield return null;
    }

    bool OddsChecking()
    {
        return (Random.Range(0f, 1f) < 0.5f);
    }
}
