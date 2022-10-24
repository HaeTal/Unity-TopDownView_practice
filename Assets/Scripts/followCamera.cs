using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    
    void FixedUpdate()
    {
        if(target != null)
        {
            transform.position = target.position + offset;
        }
    }

}
