using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavPaneAnchor : MonoBehaviour {

    public float heightOffset = 10;

    Vector3 screenPosition;

    private void Awake()
    {
        screenPosition = new Vector3(Screen.width / 2f, Screen.height - heightOffset, 1);
        //transform.position = Camera.main.ScreenToWorldPoint(screenPosition);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {
        //transform.position = Camera.main.ScreenToWorldPoint(screenPosition);
    }
}
