using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationPaneObject : MonoBehaviour {

    Timer timer;
    Vector3 newPosition;

    // Use this for initialization
    void Start () {
		timer = gameObject.GetComponent<Timer>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Move(Vector3 position, bool isScreenPoint = false)
    {

        if (gameObject.name == "Timer")
        {
            newPosition = !isScreenPoint ? Camera.main.WorldToScreenPoint(position) : position;
            timer.X = newPosition.x;
            timer.Y = newPosition.y;
        }
        else
        {
            newPosition = isScreenPoint ? Camera.main.ScreenToWorldPoint(position) : position;
            transform.position = newPosition;
        }
        
    }
}
