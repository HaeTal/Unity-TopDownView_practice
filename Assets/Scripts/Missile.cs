using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public float Range;
    public GameObject rangeEffect;
    public GameObject explosionEffect;
    public LayerMask targetLayer;
    public float power;
    public bool isDown;


    private void OnTriggerEnter(Collider other)
    {
        if((other.gameObject.tag == "Floor" || other.gameObject.tag == "Wall") && isDown)
        {
            Explosion(Range);
        }
    }

    void Explosion(float Range)
    {
        Instantiate(explosionEffect, transform.position, Quaternion.identity);

        Collider[] colls = Physics.OverlapSphere(transform.position, Range, targetLayer);

        foreach (Collider coll in colls)
        {
            Debug.Log(coll.name);

            Rigidbody rigid = coll.GetComponent<Rigidbody>();
            rigid.AddExplosionForce(power, transform.position, Range);
        }

        Destroy(gameObject, 1);
    }

    public void DrawRange(Vector3 targetPos)
    {
        //Debug.Log("DrawRange");
        Destroy(Instantiate(rangeEffect, targetPos + new Vector3(0, 0.5f, 0), Quaternion.identity), 0.3f);
    }
}
