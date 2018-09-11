using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveNaviagationPane : MonoBehaviour {

    public NavigationPaneObject timer;
    public NavigationPaneObject actions;
    public GameObject navPaneCenterAnchor;

    // Use this for initialization
    void Start () {
        timer = transform.Find("Timer").gameObject.GetComponent<NavigationPaneObject>();
        actions = transform.Find("GameManager").Find("Action").gameObject.GetComponent<NavigationPaneObject>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Move(Vector3 playerPosition)
    {
        var centerScreenPoint = Camera.main.WorldToScreenPoint(new Vector3(playerPosition.x, playerPosition.y, -2));

        timer.Move(new Vector3(centerScreenPoint.x*.22f, centerScreenPoint.y * 0.065f, centerScreenPoint.z), true);
        actions.Move(Camera.main.ScreenToWorldPoint(new Vector3(centerScreenPoint.x * .1f, centerScreenPoint.y * 1.85f, centerScreenPoint.z)));
    }
}
