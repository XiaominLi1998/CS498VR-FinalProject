using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwardDrop : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody gameObjectsRigidBody;

    void Start()
    {
      GameObject myGameObject = new GameObject("Test Object"); // Make a new GO.

      gameObjectsRigidBody = myGameObject.AddComponent<Rigidbody>(); // Add the rigidbody.
      gameObjectsRigidBody.mass = 5;
      gameObjectsRigidBody.useGravity = false;
    }

    // Update is called once per frame
    void Update()
    {
        //if(GameController.correct_answer){
          //gameObjectsRigidBody.useGravity = true;
      //  }
    }
}
