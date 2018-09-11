using UnityEngine;
using System.Collections;

public class Wind : MonoBehaviour
{

    public float force = 10f;

    Rigidbody rigidBody;
    CharacterController primaryController;
    bool isOn = false;
    Generator generator;
    Vector3 windDirection;
    GameObject generatorObj;
    // Use this for initialization

    void Start()
    {
        Vector3 generatorPosition = new Vector3(0f, 0f, 0f);
        StartCoroutine(Generator.FindGenerator(gameObject, out generatorObj));
        rigidBody = GameObject.FindGameObjectWithTag("Primary").GetComponent<Rigidbody>();
        //parent gen button obj
        generatorPosition = generatorObj.transform.position;
        generator = generatorObj.GetComponent<Generator>();
        windDirection = (transform.position - generatorPosition).normalized;
        
  
    }

    // Update is called once per frame
    void Update()
    {
        if(generator == null && generatorObj != null)
            generator = generatorObj.GetComponent<Generator>();
        else
            isOn = generator.isOn;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Primary" && isOn)
        {
            rigidBody.AddForce(windDirection * force);

        }
    }

    void OnTriggerStay(Collider col)
    {
        if (col.tag == "Primary" && isOn)
        {
            rigidBody.AddForce(windDirection * force);

        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Primary")
        {
        }
    }

}
