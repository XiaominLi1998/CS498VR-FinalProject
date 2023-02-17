using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropAnimal : MonoBehaviour
{
    // Start is called before the first frame update
    public static bool shouldDrop = false;
    public GameObject relativeObj;
    void Start()
    {        
        relativeObj = GameObject.Find("AnimalPos");
    }

    // Update is called once per frame
    void Update()
    {        
        if (shouldDrop) {
            transform.position = relativeObj.transform.position;
            transform.rotation = relativeObj.transform.rotation;
            //gameObject.GetComponent<BoxCollider>().isTrigger = true;
            //gameObject.GetComponent<Rigidbody>().useGravity = true;
            shouldDrop = false;
        }
    }
}
