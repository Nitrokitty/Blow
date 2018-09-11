using UnityEngine;
using System.Collections;

public class Function : MonoBehaviour {
    public float rate = .55f;
    public bool isBottom = true;
    public float openTime = 2f;

    Vector3 rotationPoint;
    Vector3 rotationAxis;
    Quaternion startingRotation;
    Vector3 startingLocation;
    State state;
    float waitTime;
    float z;
	// Use this for initialization
	void Start () {
        rate = isBottom ? -Mathf.Abs(rate) : Mathf.Abs(rate);
        var parent = transform.parent;
        for (int i = 0; i < parent.childCount; i++)
        {
            if (parent.GetChild(i).gameObject.name == (isBottom ? "Bottom" : "Top") + "RotationPoint")
            {
                rotationPoint = parent.GetChild(i).position;
                break;
            }
        }
        startingLocation = transform.position;
        rotationAxis = new Vector3(0, 0, 1);
        state = State.Waiting;
        startingRotation = transform.rotation;
    }
	
	// Update is called once per frame
	void Update () {
        z = Mathf.Abs(transform.rotation.eulerAngles.z);
        switch (state)
        {
            case State.Waiting:
                if (Input.GetMouseButtonUp(0))
                    state = State.Opening;
                break;
            case State.Opening:
                Open();
                break;
            case State.Closing:
                Close();
                break;
            case State.Open:
                RemainOpen();
                break;
        }
	}

    void Open()
    {
        transform.RotateAround(rotationPoint, rotationAxis, rate);
        
        if((isBottom && z < 270 && z != 0) || (!isBottom && z > 90))
        {
            waitTime = 0;
            state = State.Open;
        }

    }

    void Close()
    {
        transform.RotateAround(rotationPoint, rotationAxis, -rate);
        if (z < 5)
            Reset();
    }

    void RemainOpen()
    {
        waitTime += Time.deltaTime;
        if (waitTime >= openTime)
            state = State.Closing;
    }

    void Reset()
    {
        state = State.Waiting;
        transform.rotation = startingRotation;
        transform.position = startingLocation;
    }

    enum State
    {
        Opening,
        Closing,
        Open,
        Waiting
    }
}
