using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIAlphaChanel : MonoBehaviour {

    public byte backgroundAlpha = 50;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnGUI()
    {
        GUI.color = new Color32(255, 255, 255, backgroundAlpha);
    }
}
