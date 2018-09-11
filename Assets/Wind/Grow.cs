using UnityEngine;
using System.Collections;

public class Grow : MonoBehaviour {
    public Camera camera;
    public float startTime = -1;
    public float waitTime = 5f;
    public int percent = 20;
    public int rate = 1;
    public int currentPercent = 0;
    Vector3 startingScale;
	// Use this for initialization
	void Start () {
        startingScale = transform.localScale;

    }
	
	// Update is called once per frame
	void Update () {
        var position = camera.WorldToViewportPoint(transform.position);
        if (startTime < 0 && position.x > 0 && position.y > 0)
            startTime = Time.time;
        if(startTime >= 0 && Time.time - startTime > waitTime)
        {
            transform.localScale = startingScale + startingScale * (currentPercent /100f);
            currentPercent += rate;
            if(currentPercent < 0 || currentPercent >= percent)
            {
                rate = -rate;
                currentPercent += 2 * rate;
            }
        }

    }
}
