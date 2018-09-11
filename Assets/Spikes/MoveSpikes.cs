using UnityEngine;
using System.Collections;

public class MoveSpikes : MonoBehaviour {
    public float speed = 3f;
    public float backSpeed = 1f;
    public float delay = 1f;
    public Vector3 direction = new Vector3(1,0,0);

    State state = State.StartDelay;
    Vector3 startingPosition;
    float currentWait = 0;
    Rigidbody rb;
	// Use this for initialization
	void Start () {
        direction = direction.normalized;
        startingPosition = transform.position;
        currentWait = 0;
        rb = GetComponent<Rigidbody>();
        speed = Mathf.Abs(speed);
        backSpeed = -Mathf.Abs(backSpeed);
    }
	
	// Update is called once per frame
	void Update () {
	    switch(state)
        {
            case State.StartDelay:
            case State.BackDelay:
                Delay();
                break;
            case State.Forward:
                Forward();
                break;
            case State.Backward:
                Backward();
                break;
        }
	}

    void Forward()
    {
        rb.AddForce(direction * speed);
    }

    void Backward()
    {
        rb.AddForce(direction * backSpeed);
        if ((transform.position - startingPosition).magnitude <= (direction * backSpeed).magnitude)
        {
            state = State.StartDelay;
            Stop();
        }
    }

    void Delay()
    {
        currentWait += Time.deltaTime;
        if(currentWait >= delay)
        {
            if (state == State.StartDelay)
                state = State.Forward;
            else
                state = State.Backward;
            currentWait = 0;
        }
    }

    void Stop()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    void OnTriggerEnter(Collider col)
    {
        Debug.Log(col.name + "\t" + col.tag); 
        if (col.tag == "Solid" || (col.tag == "Door" && col.GetComponent<Door>() != null && col.GetComponent<Door>().state != Door.State.Opening))
        {
            state = State.BackDelay;
            Stop();
        }
    }


    enum State
    {
        Forward,
        Backward,
        BackDelay,
        StartDelay
    }
}
