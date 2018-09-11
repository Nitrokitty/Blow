using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
    GameObject primary;
    GameObject navigationPane;
    Vector3 iconDisplacement;

    void Start()
    {
        primary = GameObject.FindGameObjectWithTag("Primary");
        navigationPane = GameObject.FindGameObjectWithTag("Navigation Pane");
    }

    void LateUpdate()
    {
        var newPosition = new Vector3(primary.transform.position.x, primary.transform.position.y, transform.position.z);
        transform.position = newPosition;
        if(navigationPane != null)
            navigationPane.GetComponent<MoveNaviagationPane>().Move(newPosition);
    }
}
