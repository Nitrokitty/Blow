using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmitButton : MonoBehaviour {

    public GUIStyle style;
    public SpriteRenderer sprite;
    public string text = "Login";
    public Vector3 positionPercent;
    Vector2 size;
    public bool WasPressed = false;
    public bool isEnabled = false;
    bool prevIsEnabled = false;
    Vector3 worldScreen;

    // Use this for initialization
    void Start () {

        GetComponent<SpriteRenderer>().enabled = false;
       
    }
	
	// Update is called once per frame
	void Update () {

        if (isEnabled && prevIsEnabled)
        {
            transform.position = new Vector3(0 , transform.position.y, transform.position.z);
        }
        else if (!isEnabled && prevIsEnabled)
        {
            GetComponent<SpriteRenderer>().enabled = false;
        }
        else if(!prevIsEnabled && isEnabled)
        {
            GetComponent<SpriteRenderer>().enabled = true;
        }

        prevIsEnabled = isEnabled;
    }

    private void OnGUI()
    {
        if (isEnabled && prevIsEnabled)
        {
            var screen = new Vector3(Screen.width * positionPercent.x, Screen.height * positionPercent.y);
            style.fontSize = Screen.width / 28;
            if (GUI.Button(new Rect(screen, size), text, style))
                WasPressed = true;
           
        }
    }
}
