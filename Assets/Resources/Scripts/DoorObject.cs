using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorObject : HackableObject
{
    public Sprite open;
    public Sprite closed;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        hackName = "door" + HackableObject.GetNum();
    }

    public override void OnOpen()
    {
        GetComponent<SpriteRenderer>().sprite = open;
    }

    public override void OnClose()
    {
        GetComponent<SpriteRenderer>().sprite = closed;
    }
}
