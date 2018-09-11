using UnityEngine;
using System.Collections;

public class Trigger : MonoBehaviour {
    Generator gen;
	// Use this for initialization
	void Start () {
        gen = transform.parent.GetComponent<Generator>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Primary")
        {
            gen.isOn = true;
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (col.tag == "Primary" && !gen.isOn)
        {
            gen.isOn = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Primary")
        {
            gen.isOn = false;
        }
    }
}
