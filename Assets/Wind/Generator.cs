using UnityEngine;
using System.Collections;

public class Generator : MonoBehaviour
{
    public static IEnumerator FindGenerator(GameObject obj, out GameObject generatorObj)
    {
        var g = FindGeneratorSub(obj);
        generatorObj = g;
        return FindGeneratorWait(g);
    }

    static IEnumerator FindGeneratorWait(GameObject g)
    {
        yield return new WaitUntil(() => g != null);
    }

    static GameObject FindGeneratorSub(GameObject obj)
    { 
        Transform parent = obj.transform.parent;
        while(!parent.gameObject.name.ToLower().Contains("basewind"))
        {
            parent = parent.parent;
        }
        for (int i = 0; i < parent.childCount; i++)
        {
            if (parent.GetChild(i).name == "Generator")
            {
                parent = parent.GetChild(i);
                for (int j = 0; j < parent.childCount; j++)
                {
                    if (parent.GetChild(j).name == "Button")
                    {
                        parent = parent.GetChild(j);
                        for (int k = 0; k < parent.childCount; k++)
                        {
                            if (parent.GetChild(k).name == "Object")
                            {
                                return parent.GetChild(k).gameObject;
                            }
                        }
                    }
                }
            }
        }
        return null;
    }

    public bool isOn = true;
    public float delay = .2f;
    float currentWait = 0f;
    bool isWaiting = false;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(isWaiting)
        {
            currentWait += Time.deltaTime;
            if (currentWait >= delay)
                isWaiting = false;
        }
    }

    void OnMouseDown()
    {
        if (!isWaiting && GameManager.Manager.GameState == GameState.Playing)
        {
            isOn = !isOn;
            currentWait = 0;
            isWaiting = true;
        }
    }
}
