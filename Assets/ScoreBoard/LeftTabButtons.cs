using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LeftTabButtons : MonoBehaviour {

    GameObject menuBackground;
    public Rect topTabRect;
    public GUIStyle tabButtonStyle;
    int multiplier = 0;

    Rect staticTopRact;
	// Use this for initialization
	void Start () {
        menuBackground = transform.parent.Find("MenuBackground").gameObject;

    }
	
	// Update is called once per frame
	void Update () {
        
    }

    Rect GetTabPosition()
    {
        var menuBackgroundPos = Camera.main.WorldToScreenPoint(menuBackground.GetComponent<SpriteRenderer>().bounds.min);
        var size = Camera.main.WorldToScreenPoint(menuBackground.GetComponent<SpriteRenderer>().bounds.max) - Camera.main.WorldToScreenPoint(menuBackground.GetComponent<SpriteRenderer>().bounds.min);

        staticTopRact = new Rect(menuBackgroundPos.x + topTabRect.x, menuBackgroundPos.y + topTabRect.y + size.y * .28f, topTabRect.width, topTabRect.height);

        return new Rect(staticTopRact.x, staticTopRact.y + multiplier * staticTopRact.height, staticTopRact.width, staticTopRact.height);
    }

    void OnGUI()
    {
        foreach(var level in Enum.GetValues(typeof(Levels)))
        {
            var l = (Levels)level;
            if (l >= 0)
            {
                if(GUI.Button(GetTabPosition(), GameManager.GetLevelDisplayName(l), tabButtonStyle))
                {
                    transform.parent.Find("Scores").GetComponent<ScoreList>().GetRuns(l);
                }
                multiplier++;
            }
        }
        multiplier = 0;
    }
}

