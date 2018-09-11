using UnityEngine;
using System.Collections;

public class TitleButton : MonoBehaviour {
    GameManager manager;
    public static bool ShowAbout = false;
    Rect box;
    Rect back;
    float size = .5f;
    string linkedin = @"https://www.linkedin.com/in/audrey-talley-006282a1\n";
    string cgc = @"http://web.sa.sc.edu/cgc/";
    public GameObject login;

    // Use this for initialization
    void Start () {
        box = new Rect((int)(Screen.width * (1 - size) / 2f), (int)(Screen.height * (1 - size) / 2f), (int)(Screen.width * size), (int)(Screen.height * size));
        //next = new Rect((int)(boxW * 0.3f + boxX), (int)(boxH * 0.8f + boxY), (int)(boxW * .4f), (int)(boxH * .1f));
        back = new Rect((int)(box.x + box.width * .1f), (int)(box.y + box.height * 0.8f), (int)(box.width * 0.3), (int)(box.height * 0.1f));
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        
        if (ShowAbout)
        {
            GUI.skin.box = manager.manager.BoxStyle;
            GUI.color = new Color(1f, 1f, 1f, .5f);
            GUI.skin.button = manager.manager.ButtonStyle;
            
            GUI.Box(box, "Created by Audrey \"Danielle\" Talley, president and founder of the Carolina Gamers Club (CGC), for the Global Game Jam 2017. The CGC is a student organization at the University of South Carolina dedicated to the education and progression of game design and development!\nThe goal of this game was to create an extremely intuative game that requires no instructions by providing a low entry skill that is difficult to master!\nMe " + linkedin + "\nCGC: " + cgc +" \n\nPS: I am not an artist... so sorry... (especially for these awful text boxes)");
            if (GUI.Button(back, "Done"))
                ShowAbout = false;
        }

    }

     void OnMouseDown()
    {   
        if (!ShowAbout)
        {
            Debug.Log(gameObject.name);
            if (gameObject.name.ToLower() == "play")
            {
                if (UserProfile.UserName == "Guest" && !UserProfile.ContinueAsGuest && !UserProfile.IsLoggedIn)
                    login.GetComponent<Login>().show = true;
                else
                    GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>().manager.Start();
            }
            else if (gameObject.name.ToLower().Contains("away"))
            {
                GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>().manager.Exit();
            }
            else if (gameObject.name.ToLower().Contains("about"))
            {
                ShowAbout = true;
            }
        }
    }
}
