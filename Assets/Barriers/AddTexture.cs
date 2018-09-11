using UnityEngine;
using System.Collections;

public class AddTexture : MonoBehaviour {

    public Texture texture;

    Renderer rend;

	// Use this for initialization
	void Start () {
        rend = gameObject.GetComponent<Renderer>();
        rend.material.mainTexture = texture;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
