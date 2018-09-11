using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class BackButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            var mousePosition = Input.mousePosition;
            var screenButton = Camera.main.WorldToScreenPoint(transform.position);

            var extends = GetComponent<SpriteRenderer>().bounds.extents;
            var delta = new Vector3(screenButton.x * extends.x, screenButton.y * extends.y, screenButton.z * extends.z);

            if (mousePosition.x > (screenButton.x - delta.x) &&
                mousePosition.x < (screenButton.x + delta.x) &&
                mousePosition.y > (screenButton.y - delta.y) &&
                mousePosition.y < (screenButton.y + delta.y))
            {
                SceneManager.LoadScene(Levels.MainMenu.ToString());
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene(Levels.MainMenu.ToString());
    }


}
