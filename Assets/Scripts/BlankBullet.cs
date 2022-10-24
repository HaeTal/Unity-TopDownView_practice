using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlankBullet : MonoBehaviour
{
    public GameObject effectObj;
    //public LayerMask layerMask;
    int enemyLayer = (1 << 9) | (1 << 10);          // Enemy, EnemyBullet Layer

    public float range = 20f;
    public float power = 200f;

    public void Use()
    {
        //Debug.Log("BlankBullet");
        Collider[] colls = Physics.OverlapSphere(transform.position, range, enemyLayer);

        foreach(Collider coll in colls)
        {
            Debug.Log(coll.name);

            if(coll.gameObject.tag == "EnemyBullet")
            {
                Destroy(coll.gameObject);
            }

            Rigidbody enemyRigid = coll.GetComponent<Rigidbody>();
            Vector3 reactVec = (enemyRigid.position - transform.position).normalized;
            enemyRigid.AddForce(reactVec * power, ForceMode.VelocityChange);
            //enemyRigid.AddExplosionForce(power, transform.position, range);
        }
        

    }
}


