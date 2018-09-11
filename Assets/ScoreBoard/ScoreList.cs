using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScoreList : MonoBehaviour {
    public string text;
    GameObject menuBackground;
    GameObject tab;
    SpriteRenderer menuSprite;
    int height;
    LeftTabButtons tabScript;

    Vector3 position;

    NetworkManager networkManager;
    List<FileData> fileData;
    Status status = Status.Displaying;

    public GUIStyle labelStyle2;
    // Use this for initialization
    void Start()
    {
        menuBackground = transform.parent.Find("MenuBackground").gameObject;
        tab = transform.parent.Find("Tabs").gameObject;
        networkManager = GameObject.Find("UserProfile").GetComponent<NetworkManager>();
    }
	
	// Update is called once per frame
	void Update () {
        menuSprite = menuBackground.GetComponent<SpriteRenderer>();
        tabScript = tab.GetComponent<LeftTabButtons>();
        switch(status)
        {
            case Status.Displaying:
                break;
            case Status.Retrieving:
                CheckRun();
                break;
        }
    }

    void OnGUI()
    {
        var worldMax = menuSprite.bounds.max;
        var worldMin = menuSprite.bounds.min;

        var size = Camera.main.WorldToScreenPoint(worldMax) - Camera.main.WorldToScreenPoint(worldMin);
        size.x -= tabScript.topTabRect.width;
        var menuBackgroundPos = Camera.main.WorldToScreenPoint(worldMin);
        
        menuBackgroundPos = new Vector3(menuBackgroundPos.x + tabScript.topTabRect.width, menuBackgroundPos.y +size.y*.28f, menuBackgroundPos.z);

        GUI.Label(new Rect(new Vector3(menuBackgroundPos.x, menuBackgroundPos.y - 2), new Vector2(size.x, size.y * .1f)), " Rank  Time    Username", labelStyle2);
        GUI.TextArea(new Rect(new Vector3(menuBackgroundPos.x, menuBackgroundPos.y-2+size.y*.1f), size-new Vector3(0, size.y*.1f, 0)), text);
        
        //GUI.Label(new Rect(Camera.main.WorldToScreenPoint(Vector3.zero), new Vector2(10,10)), "??????????????", labelStyle);
    }

    public void CheckRun()
    {
        text = "";
        if(NetworkManager.referenceData != null)
        {
            if(NetworkManager.referenceData.Count == 0)
            {
                text += "There aren't any successfull runs for this level yet!";
            }
            for(int i = 0; i < 20 && i < NetworkManager.referenceData.Count; i++)
            {
                var d = NetworkManager.referenceData[i];
                var time = TimeSpan.FromTicks(d.Time);
                var minutes = time.Minutes.ToString();
                var seconds = time.Seconds.ToString();
                var miliSeconds = time.Milliseconds.ToString();

                if (seconds.Length == 1)
                    seconds = "0" + seconds;
                if (miliSeconds.Length == 1)
                    miliSeconds = "00" + miliSeconds;
                else if (miliSeconds.Length == 2)
                    miliSeconds = "0" + miliSeconds;
                else if (miliSeconds.Length > 3)
                    miliSeconds = miliSeconds.Substring(0, 3);
                text += " " + (i + 1);
                text += i < 9 ? "             " : "             ";
                text += minutes + ":" + seconds + ":" + miliSeconds + "     " + d.Username + "\n";
            }
            status = Status.Displaying;
        }
    }

    public void GetRuns(Levels level)
    {
        fileData = null;
        var d = new Dictionary<RunSearchCriteria, string>();
        d.Add(RunSearchCriteria.Level, GameManager.GetLevelDisplayName(level));
        networkManager.RetrieveRuns(d, out fileData);
        status = Status.Retrieving;
    }

    enum Status
    {
        Displaying,
        Retrieving
    }
}
