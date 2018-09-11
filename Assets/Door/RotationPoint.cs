using UnityEngine;
using System.Collections;

public class RotationPoint : MonoBehaviour {
    Door door;
    public bool isBottom;
    float rotation;
    GameObject rotatingObject;
    Vector3 startingLocation;
    Vector3 rotationAxis = new Vector3(0, 0, 1);
    Vector3 startingRotation;
    float targetRotation;
    float currentRotation;
    float waitTime;
    // Use this for initialization
    //void Start () {
    //    door = GameManager.FindParent(gameObject, "Door", true).GetComponent<Door>();
    //    rotatingObject = transform.parent.gameObject;
    //    if (!isBottom)
    //        rotation = -door.maxRotation;
        
    //    startingRotation = rotatingObject.transform.rotation.eulerAngles;
    //    startingLocation = rotatingObject.transform.position;
    //    targetRotation = startingRotation.z + rotation;
    //    if (targetRotation < 0) targetRotation += 360;
    //    else if (targetRotation >= 360) targetRotation -= 360;
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    currentRotation = Mathf.Abs(rotatingObject.transform.rotation.eulerAngles.z);
    //    switch (door.state)
    //    {
    //        case Door.State.Waiting:
    //            break;
    //        case Door.State.Opening:
    //            Open();
    //            break;
    //        case Door.State.Closing:
    //            Close();
    //            break;
    //        case Door.State.Open:
    //            RemainOpen();
    //            break;
    //    }
    //}

    //void Open()
    //{
    //    transform.RotateAround(transform.position, rotationAxis, door.openSpeed);

    //    if ( Mathf.Abs(targetRotation-currentRotation) < door.openSpeed)
    //    {
    //        waitTime = 0f;
    //        door.state = Door.State.Open;
    //    }

    //}

    //void Close()
    //{
    //    transform.RotateAround(transform.position, rotationAxis, -door.openSpeed);
    //    if (Mathf.Abs(targetRotation - startingRotation.z) < door.openSpeed)
    //        Reset();
    //}

    //void RemainOpen()
    //{
    //    waitTime += Time.deltaTime;
    //    if (waitTime >= door.timeToRemainOpen)
    //        state = Door.State.Closing;
    //}

    //void Reset()
    //{
    //    state = Door.State.Waiting;
    //    rotatingObject.transform.rotation = Quaternion.Euler(startingRotation);
    //    rotatingObject.transform.position = startingLocation;
    //}
}
