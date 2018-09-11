using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour {

    public Vector3 screenPercentages = new Vector3(.7f, .7f);

    SpriteRenderer spriteRenderer;
    Vector3 max;
    Vector3 min;

    float scaleRatio;
    // Use this for initialization
    void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
	
	// Update is called once per frame
	void Update () {
        max = Camera.main.WorldToScreenPoint(spriteRenderer.bounds.max);
        min = Camera.main.WorldToScreenPoint(spriteRenderer.bounds.min);

        Vector3 screen = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));
        transform.localScale = new Vector3(screen.x * screenPercentages.x, screen.y * screenPercentages.y, transform.localScale.z);

    }
}
