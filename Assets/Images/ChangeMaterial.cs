using UnityEngine;
using System.Collections;

public class ChangeMaterial : MonoBehaviour {
    public Material material;
    //public Vector3 rotation = Vector3.zero;
    Renderer rend;
	// Use this for initialization
	void Start () {
        rend = GetComponent<Renderer>();
        //if (rotation != Vector3.zero)
            
        rend.material = material;
    }
	
	// Update is called once per frame
	void Update () {
        //rend.material.SetMatrix("_TextureRotation", Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(rotation), Vector3.one));

    }
}