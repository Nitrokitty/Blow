using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {
    public GameObject top;
    public GameObject topPoint;
    public GameObject bottom;
    public GameObject bottomPoint;

    public float timeToRemainOpen = 2;
    public float openSpeed = 1;
    public float maxRotation = -90;
    public State state = State.Waiting;

    Panel bottomPannel;
    Panel topPannel;
    float currentWait = 0;

    void Start()
    {

        bottomPannel = new Panel(bottom, bottomPoint, -openSpeed, maxRotation);
        topPannel = new Panel(top, topPoint, openSpeed, maxRotation);
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case Door.State.Waiting:
                break;
            case Door.State.Opening:
                bottomPannel.Open();
                topPannel.Open();
                if (bottomPannel.IsOpen || topPannel.IsOpen)
                {
                    currentWait = 0;
                    state = State.Open;
                    bottomPannel.AlightWithTarget();
                    topPannel.AlightWithTarget();
                }
                break;
            case Door.State.Closing:
                bottomPannel.Close();
                topPannel.Close();
                if (bottomPannel.IsClosed || topPannel.IsClosed)
                {
                    bottomPannel.Reset();
                    topPannel.Reset();
                    state = State.Waiting;
                }
                break;
            case Door.State.Open:
                currentWait += Time.deltaTime;
                if (currentWait >= timeToRemainOpen)
                    state = State.Closing;
                break;
        }
    }

    public void Open()
    {
        state = State.Opening;
    }

    public enum State
    {
        Opening,
        Closing,
        Open,
        Waiting
    }

    public class Panel
    {
        GameObject panel;
        GameObject point;
        
        float speed;
        Vector3 rotationAxis = new Vector3(0, 0, 1);
        float targetRotation;
        Vector3 originalRotation;
        float currentRotation = 0;
        public Panel(GameObject panel, GameObject point, float speed, float rotation)
        {
            this.panel = panel;
            this.point = point;
            this.speed = speed;
            this.targetRotation = rotation;
            originalRotation = panel.transform.localEulerAngles;

        }

        float CurrentRotation
        {
            get
            {
                var rot = panel.transform.localEulerAngles.z;
                return rot < 0 ? rot + 360 : rot;
            }
        }

        float TargetRotation
        {
            get
            {
                return targetRotation * speed < 0 ? 360f - Mathf.Abs(targetRotation): Mathf.Abs(targetRotation);
            }
        }

        float OriginalRotation
        {
            get
            {
                return originalRotation.z < 0 ? originalRotation.z + 360 : originalRotation.z;
            }
        }

        public bool IsOpen
        {
            get
            {
                return Mathf.Abs(TargetRotation - CurrentRotation) <= Mathf.Abs(speed);
            }
        }

        public bool IsClosed
        {
            get { return Mathf.Abs(CurrentRotation - OriginalRotation) <= speed || (OriginalRotation == 0 && Mathf.Abs(CurrentRotation - 360) <= speed); }
        }

        void RotatePanel(bool negateSpeed = false)
        {
            panel.transform.RotateAround(point.transform.position, rotationAxis, negateSpeed ? -speed : speed);
            currentRotation += negateSpeed ? -speed : speed;
        }

        public void Open()
        {
            RotatePanel();
        }

        public void Close()
        {
            RotatePanel(true);
        }

        public void Reset()
        {
            panel.transform.localRotation = Quaternion.Euler(originalRotation);
        }

        public void AlightWithTarget()
        {
            panel.transform.localRotation = Quaternion.Euler(new Vector3(originalRotation.x, originalRotation.y, targetRotation));
        }
    }


}
