using UnityEngine;
using System.Collections;



public class ToggleManager : MonoBehaviour
{

    public SpriteRenderer play;
    public SpriteRenderer pause;
    public GameManager manager;
    public Camera camera;
    

    public float screenHeight;
    // Use this for initialization
    void Start()
    {

        //manager = transform.parent.gameObject.GetComponent<GameManager>();
        if (GameManager.Manager.CurrentLevel < 0)
        {
            Destroy(transform.gameObject);
            return;
        }
        play.enabled = true;
        pause.enabled = false;
        
    }

    void LateUpdate()
    {

    }

    void OnGUI()
    {
        
    }

    void OnMouseDown()
    {
        var state = GameManager.Manager.GameState;
        if (state != GameState.Finished)
        {
            if (state == GameState.Playing)
            {
                GameManager.Manager.GameState = GameState.Paused;
                ToggleIcons();
            }
            else if (state == GameState.Paused)
            {
                GameManager.Manager.GameState = GameState.Playing;
                ToggleIcons();
            }
        }
    }

    void ToggleIcons()
    {
        play.enabled = !play.enabled;
        pause.enabled = !pause.enabled;
    }
}
