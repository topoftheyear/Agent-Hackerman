using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HackableObject : MonoBehaviour
{
    public static int totalObjects = 0;

    public string hackName;
    public string state;
    public bool permanentlyDead;

    public TextMesh billboard;

    // Start is called before the first frame update
    public void Start()
    {
        hackName = "nothing";
        state = "none";
        permanentlyDead = false;

        //gameObject.AddComponent<TextMesh>();
        //billboard = gameObject.GetComponent<TextMesh>();
    }

    public void Update()
    {
        //billboard.text = hackName;
        switch (state)
        {
            case ("none"):                      break;
            case ("open"):      OnOpen();       break;
            case ("close"):     OnClose();      break;
            case ("explode"):   OnExplode();    break;
        }
    }

    public virtual void OnOpen()
    {

    }

    public virtual void OnClose()
    {

    }

    public virtual void OnExplode()
    {
        GameObject explosion = Instantiate((GameObject)Resources.Load("Prefabs/Explosion"));
        explosion.transform.position = this.transform.position;
        explosion.transform.rotation = this.transform.rotation;
        this.gameObject.SetActive(false);
        permanentlyDead = true;
    }

    public virtual void EnableThings()
    {
        if (!permanentlyDead)
        {
            this.gameObject.SetActive(true);
        }
    }

    public virtual void DisableThings()
    {
        this.gameObject.SetActive(false);
    }

    public static int GetNum()
    {
        totalObjects++;
        return totalObjects;
    }
}
