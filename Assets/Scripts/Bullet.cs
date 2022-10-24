using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    
    public float damage;
    public float speed;
    public float litetime = 3;

    void Start()
    {
        Destroy(gameObject, litetime);
    }

    

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Wall")
        {
            Destroy (gameObject);
        }
    }


}
