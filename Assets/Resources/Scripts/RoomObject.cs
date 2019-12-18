using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomObject : MonoBehaviour
{
    public Dictionary<GameObject, GameObject> doors;

    public GameObject item;

    private void Awake()
    {
        doors = new Dictionary<GameObject, GameObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void EnableThings()
    {
        foreach (Transform c in transform)
        {
            GameObject child = c.gameObject;

            child.SetActive(true);
        }

        foreach (GameObject door in doors.Keys)
        {
            door.GetComponent<DoorObject>().EnableThings();
        }
    }

    public void DisableThings()
    {
        foreach (Transform c in transform)
        {
            GameObject child = c.gameObject;

            child.SetActive(false);
        }

        foreach (GameObject door in doors.Keys)
        {
            door.GetComponent<DoorObject>().DisableThings();
        }
    }
}
