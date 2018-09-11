using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;


public class GameManager : MonoBehaviour
{
    public static GameObject FindParent(GameObject obj, string target, bool isName = true)
    {
        GameObject parent = obj.transform.parent.gameObject;
        if (isName)
        {
            while (parent.name != target)
                parent = parent.transform.parent.gameObject;
        }
        else
        {
            while (parent.tag != target)
                parent = parent.transform.parent.gameObject;
        }
        return parent;
    }

    //public static Vector3 GetWorldPosition(Transform objectTransform)
    //{
    //    Vector3 position = Vector3.zero;
    //    Transform current = objectTransform;

    //    do
    //    {
    //        position += current.posi
    //        current = current.parent;
    //    } while (current != null);
    //}

    public class Manager
    {
        float originalTimeScale;
        BoxStyle styles;
        static List<FileData> userTopRuns;
        public static int MaxRank = 5;
        public bool HasBeenSaved = false;

        public Manager(GameState state)
        {
            Manager.GameState = state;
            originalTimeScale = 1;
            Time.timeScale = originalTimeScale;
            styles = GameObject.FindGameObjectWithTag("Manager").GetComponent<BoxStyle>();
           
            UserProfile.ID = UserProfile.ID == UserProfile.GuestID ? Guid.NewGuid().ToString(): UserProfile.ID;
        }

        public GUIStyle BoxStyle
        {
            get { return styles.boxStyle; }
        }

        public GUIStyle ButtonStyle
        {
            get { return styles.buttonStyle; }
        }

        public static Levels CurrentLevel
        {
            get { return (Levels)Enum.Parse(typeof(Levels), SceneManager.GetActiveScene().name); }
        }

        public static Levels NextLevel
        {
            get { return (Levels)(((int)CurrentLevel) + 1); }
        }

        public void ReloadLevel()
        {
            SceneManager.LoadScene(CurrentLevel.ToString());
        }

        public static GameState GameState { get; set; }

        public static TimeSpan CurrentRunTime { get; set; }

        public static List<FileData> UserTopRuns
        {
            get
            {
                userTopRuns = userTopRuns ?? FileManager.GetRuns(CurrentLevel, MaxRank, UserProfile.ID);
                return userTopRuns;
            }
        }

        public static FileData CurrentRun
        {
            get { return new FileData(true); }
        }

        public static int CurrentRunRank
        {
            get
            {
                if (UserTopRuns == null || UserTopRuns.Count == 0)
                    return -1;

                int rank;
                for(rank = 1; rank < UserTopRuns.Count; rank++)
                {
                    if (TimeSpan.FromTicks(UserTopRuns[rank].Time) > CurrentRunTime)
                        break;
                }
                return rank;
            }
        }

        public static void SaveRun()
        {
            var currentRank = CurrentRunRank;
            if (currentRank == -1)
                FileManager.Append(CurrentRun);
            else if(UserTopRuns.Count >= MaxRank && CurrentRunRank <= MaxRank)
                FileManager.Overwrite<FileData>(UserTopRuns.Last().ID, CurrentRun);
            else if (CurrentRunRank <= MaxRank)
                FileManager.Append(CurrentRun);
            
            Debug.Log("Rank: " + CurrentRunRank);
            var runs = FileManager.GetRuns(CurrentLevel, MaxRank);
            Debug.Log("----Current Runs ----");
            
            foreach (var r in runs)
                Debug.Log(r.ToString());
            Debug.Log("---- End Runs: ----");
        }

        public void LoadNextLevel()
        {
            SceneManager.LoadScene(NextLevel.ToString());
            Time.timeScale = originalTimeScale;
        }

        public void Pause()
        {
            if (Time.timeScale != 0) Time.timeScale = 0;
            
        }

        public void Play()
        {
            if (Time.timeScale != originalTimeScale) Time.timeScale = originalTimeScale;
        }

        public void End()
        {
            if (Time.timeScale != 0) Time.timeScale = 0;
        }

        public void MainMenu()
        {
            SceneManager.LoadScene(Levels.MainMenu.ToString());
            Time.timeScale = originalTimeScale;
            var data = new List<FileData>();
            FileManager.Load(out data);
        }

        public void Start()
        {
            LoadNextLevel();
        }

        public static void GoToScoreboard()
        {
            SceneManager.LoadScene("Scoreboards");
        }

        public void Exit()
        {
            Application.Quit();
        }

        public void LoadDoor()
        {
            SceneManager.LoadScene(Levels.Level_Door_1.ToString());
        }
       
    }

   

    public Manager manager;
    public Gravity ball;
    NetworkManager networkManager;
    Rect box;
    Rect next;
    Rect reload;
    Rect mainMenu;

    float size = .3f;
    float buttonSize;
    private void Awake()
    {
        networkManager = new NetworkManager();
    }

    void Start()
    {
        buttonSize = size / 2;
        manager = new Manager(GameState.Playing);
        box = new Rect((int)(Screen.width * (1 - size) / 2f), (int)(Screen.height * (1 - size) / 2f), (int)(Screen.width * size), (int)(Screen.height * size));
        //next = new Rect((int)(boxW * 0.3f + boxX), (int)(boxH * 0.8f + boxY), (int)(boxW * .4f), (int)(boxH * .1f));
        next = new Rect((int)(box.x + box.width * .6f), (int)(box.y + box.height * 0.8f), (int)(box.width * 0.3), (int)(box.height * 0.1f));
        mainMenu = new Rect((int)(box.x + box.width * .1f), (int)(box.y + box.height * 0.8f), (int)(box.width * 0.3), (int)(box.height * 0.1f));
        ball = GameObject.FindGameObjectWithTag("Primary").GetComponent<Gravity>();
    }


    void Update()
    {    
        if (Manager.CurrentLevel != Levels.MainMenu)
        { 
            if (ball.state == Gravity.PrimaryState.Dead)
                Manager.GameState = GameState.Finished;
            switch (Manager.GameState)
            {
                case GameState.Paused:
                    manager.Pause();
                    break;
                case GameState.Playing:
                    manager.Play();
                    break;
                case GameState.Finished:
                    if (ball.state != Gravity.PrimaryState.Dead && !manager.HasBeenSaved)
                    {
                        if (UserProfile.UserName != "Guest")
                        {                           
                            Manager.SaveRun();
                            networkManager.PostRun(new FileData(true));
                        }
                        manager.HasBeenSaved = true;
                    }
                    manager.End();
                    break;
            }
        }
    }

    void OnGUI()
    {
        GUI.skin.box = manager.BoxStyle;
        GUI.color = new Color(1f, 1f, 1f, .75f);
        GUI.skin.button = manager.ButtonStyle;
        if (Manager.GameState == GameState.Finished)
        {
            if (ball.state == Gravity.PrimaryState.Dead)
            {
                GUI.Box(box, "BOOO :(");
                if (GUI.Button(next, "Restart"))
                    manager.ReloadLevel();
            }
            else if (ball.state == Gravity.PrimaryState.Okay)
            {
                if (Manager.CurrentLevel == Levels.Level_Generator_3)
                {
                    GUI.Box(box, "So that's as far as I got! Wanna try the next feature (it's unfinished)?");
                    if (GUI.Button(next, "Try It!"))
                        manager.LoadDoor();
                   
                }
                else if(Manager.CurrentLevel == Levels.Level_Door_1)
                {
                    GUI.Box(box, "Ohh nice! *High Five* Hmmm.... welp, that's all I got so far! ^_^");
                }
                else
                {
                    GUI.Box(box, "Yay! Rank : " + (Manager.CurrentRunRank < 0? 1: Manager.CurrentRunRank));
                    if (GUI.Button(next, "Next"))
                        manager.LoadNextLevel();
                }
                
            }
            if (GUI.Button(mainMenu, "Menu"))
                manager.MainMenu();
        }
        else if(Manager.GameState == GameState.Paused)
        {
            GUI.Box(box, "PAUSED !!");
            if (GUI.Button(next, "Restart"))
                manager.ReloadLevel();
            if (GUI.Button(mainMenu, "Menu"))
                manager.MainMenu();
        }
    }


    public static string GetLevelDisplayName(Enum level)
    {
        if (level.GetType() != typeof(Levels))
            return null;
        var l = (Levels)(Enum.Parse(typeof(Levels), level.ToString()));
        switch (l)
        {
            case Levels.Level_Generator_1:
                return "Level 1: The Generator";
            case Levels.Level_Generator_2:
                return "Level 2: The Generator Returns";
            case Levels.Level_Generator_3:
                return "Level 3: The Generator Strikes Back";
            case Levels.Level_Door_1:
                return "Level 4: Hold The Door";
            default:
                return l.ToString();
        }
    }

    public static Levels GetLevelFromDisplayName(string level)
    {
        if (level == "Level 1: The Generator")
            return Levels.Level_Generator_1;
        else if (level == "Level 2: The Generator Returns")
            return Levels.Level_Generator_2;
        else if (level == "Level 3: The Generator Strikes Back")
            return Levels.Level_Generator_3;
        else if (level == "Level 4: Hold The Door")
            return Levels.Level_Door_1;
        else
            return Levels.MainMenu;
            
    }
}

public enum GameState
{
    Paused,
    Playing,
    Finished
}

public enum Levels
{
    Scoreboards = -2,
    MainMenu = -1,
    Level_Generator_1 = 0,
    Level_Generator_2 = 1,
    Level_Generator_3 =2,
    Level_Door_1 = 3,
    
}

