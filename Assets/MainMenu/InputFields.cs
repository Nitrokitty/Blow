using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class InputFields : MonoBehaviour {

    public Vector2 Size;
    public Vector2 MaxSize;
    RectTransform rect;
    SpriteRenderer boxBackground;

	// Use this for initialization
	void Start () {
        rect = GetComponent<RectTransform>();
        boxBackground = GameObject.Find("Login").transform.Find("Box Background").gameObject.GetComponent<SpriteRenderer>();

    }
	
	// Update is called once per frame
	void Update () {
        var size = Camera.main.ScreenToWorldPoint(new Vector3((float)Screen.width, (float)Screen.height));
        MaxSize = size;
        rect.localScale = new Vector3(size.x*.15f, size.y * .20f, 1);

    }
}
