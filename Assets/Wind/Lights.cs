using UnityEngine;
using System.Collections;

public class Lights : MonoBehaviour {

    SpriteRenderer on;
    SpriteRenderer off;
    Generator generator;

    // Use this for initialization
    void Start () {
	    for(int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name.ToLower() == "on")
            {
                on = transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>();
            }
            else if (transform.GetChild(i).name.ToLower() == "off")
            {
                off = transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>();
            }
        }
        GameObject generatorObj;
        StartCoroutine(Generator.FindGenerator(gameObject, out generatorObj));
        generator = generatorObj.GetComponent<Generator>();
        on.enabled = false;
        off.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
        if ((generator.isOn && !on.enabled) || (!generator.isOn && !off.enabled))
            ToggleLights();
    }

    void ToggleLights()
    {
        on.enabled = !on.enabled;
        off.enabled = !off.enabled;
    }
}
