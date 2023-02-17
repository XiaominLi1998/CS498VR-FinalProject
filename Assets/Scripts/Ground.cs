using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    void Awake(){
        var allChildren = gameObject.GetComponentsInChildren<Transform>();
        foreach (var child in allChildren) {
            if (child.gameObject.GetComponent<MeshRenderer>() == null){
                child.gameObject.AddComponent<MeshRenderer>();
            }
            if (child.gameObject.GetComponent<MeshCollider>() == null){
                child.gameObject.AddComponent<MeshCollider>();
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
