using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour {
    bool openDoor = false;
    GameObject doorObj;
    Door door;

    // Use this for initialization
    void Start()
    {
        doorObj = GameManager.FindParent(gameObject, "Door", false);
        door = doorObj.GetComponent<Door>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        if (door.state == Door.State.Waiting && GameManager.Manager.GameState == GameState.Playing)
        {
            door.state = Door.State.Opening;
        }
    }
}
