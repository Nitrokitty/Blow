using UnityEngine;
using System.Collections;

public class Gravity : MonoBehaviour {

    public float gravity = -9.0f;
    Vector3 originalLocation;
    public PrimaryState state = PrimaryState.Okay;
    Rigidbody rb;
	// Use this for initialization
	void Start () {
        originalLocation = transform.position;
        //rb = GetComponent<Rigidbody>().mat
	}
	
	// Update is called once per frame
	void Update () {
       
        switch(state)
        {
            case PrimaryState.Pop:
                state = PrimaryState.Dead;
                break;
        }

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, transform.rotation.eulerAngles.z));
        transform.position = new Vector3(transform.position.x, transform.position.y, originalLocation.z);

    }

    public void Pop()
    {
        state = PrimaryState.Dead;
        
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.name == "End")
        {
            GameManager.Manager.GameState = GameState.Finished;
        }
        else if(col.tag == "Spike")
        {
            Pop();
        }

    }

    public enum PrimaryState
    {
        Okay,
        Pop,
        Dead
    }

}
