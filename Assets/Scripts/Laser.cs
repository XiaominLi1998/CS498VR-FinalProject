using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour {
    public float minShootInterval = 0.45f;
    public float flickerDuration = 0.4f;
    public float laserWidth = 0.08f;
    private float lastShot = 0f;
    private LineRenderer lr;
    private GameObject shootPos; 
    private Color color;
    void Start () {
        shootPos = GameObject.Find("ShootPos");
        lr= GetComponent<LineRenderer>();
        lr.startWidth = laserWidth;
        lr.endWidth = laserWidth;
        //color = lr.material.GetColor("_TintColor");
        //lr.material.SetColor("_TintColor", color);
    }

    // Update is called once per frame
    void Update () {
        float progress = (Time.time - lastShot) / flickerDuration;
        if (progress > 1f) progress = 1f;
        //Color color = Color.Lerp(new Color(0.5f, 0.5f, 0.5f , 0.5f), new Color(0f, 0f, 0f, 0f), progress);
        //Debug.Log(color);
        //lr.startColor = color;
        //lr.endColor = color;
        //lr.materials[0].SetColor("_TintColor", color);
        float alpha = Mathf.Lerp(1f, 0f, progress);
        //lr.material.SetColor("_TintColor", color); 
        Color start = Color.red;
        start.a = alpha;
        Color end = Color.red;
        end.a = alpha;
        lr.SetColors(start, end);

    }

    public bool Shoot(Vector3 direction){
        if (Time.time - lastShot > minShootInterval) 
        {
            transform.position = shootPos.transform.position;
            transform.rotation = Quaternion.LookRotation(direction);
            lastShot = Time.time;
            lr.SetPosition(0, transform.position);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit))
            {
                if (hit. collider)
                {
                    lr.SetPosition(1, hit.point);
                }
            }
            else{
                lr.SetPosition(1, transform.forward*300);
            }
            return true;
        } else {
            return false;
        }
    }
}