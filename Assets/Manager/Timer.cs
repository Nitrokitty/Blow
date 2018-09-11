using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using UnityEngine;

public class Timer : MonoBehaviour {

    Stopwatch timer;
    string minutes;
    string seconds;
    string miliSeconds;
    Vector3 newPosition;

    public float X = 0;
    public float Y = 0;
    public float width = 50;
    public float height = 10;
    public GUIStyle style;
    bool isCounting = false;

    GameManager manager;
    // Use this for initialization
    void Start () {

        timer = new Stopwatch();
        manager = transform.parent.GetComponent<GameManager>();

    }
	
	// Update is called once per frame
	void Update () {
        if (GameManager.Manager.GameState == GameState.Paused && isCounting)
            PauseTimer();
        else if (GameManager.Manager.GameState == GameState.Playing && !isCounting)
            StartTimer();
        else if (GameManager.Manager.GameState == GameState.Finished && isCounting)
            PauseTimer();
        GameManager.Manager.CurrentRunTime = GetTime();
    }

    void OnGUI()
    {
        minutes = timer.Elapsed.Minutes.ToString();
        seconds = timer.Elapsed.Seconds.ToString();
        miliSeconds = timer.Elapsed.Milliseconds.ToString();

        if (seconds.Length == 1)
            seconds = "0" + seconds;
        if (miliSeconds.Length == 1)
            miliSeconds = "00" + miliSeconds;
        else if (miliSeconds.Length == 2)
            miliSeconds = "0" + miliSeconds;
        else if (miliSeconds.Length > 3)
            miliSeconds = miliSeconds.Substring(0, 3);

        GUI.Label(new Rect(X, Y, width, height), minutes + ":" + seconds + ":" + miliSeconds, style);
    }

    public void PauseTimer()
    {
        timer.Stop();
        isCounting = false;
    }

    public void StartTimer()
    {
        timer.Reset();
        timer.Start();
        isCounting = true;
    }

    public TimeSpan GetTime()
    {
        return timer.Elapsed;
    }
}
