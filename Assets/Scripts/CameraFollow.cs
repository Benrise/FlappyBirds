using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    [SerializeField]
    private float _followSpeed = 2f;

    [SerializeField]
    private float _yOffset =-6f;

    public Transform target;

    private void Start(){
        transform.position = new Vector3(target.position.x, 0.5f, target.position.z);
    }

    private void Update()
    {
        if (target != null && target.transform.position.y < 3){
            Vector3 newPos = new Vector3(target.position.x,target.position.y + _yOffset, -3f);
            transform.position = Vector3.Slerp(transform.position,newPos,_followSpeed*Time.deltaTime);
        }
    }
}
