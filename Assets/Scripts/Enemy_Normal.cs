using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Normal : Enemy
{
    
    protected override void Awake()
    {
        base.Awake();
    }


    protected override void Update()
    {
        if (nav.enabled)
        {
            // target Ã£±â
            base.Update();
        }
        

        if (nav.enabled && target != null)
        { 
            dist = Vector3.Distance(transform.position, target.transform.position);
            StartChase();

            if (dist < 20)
            {
                nav.speed = 1.5f;
                gun.transform.LookAt(target.transform.position + new Vector3(0, 1.5f, 0));

                if (fireDelay > fireRate)
                {
                    StartCoroutine(Attack());
                    LookTarget();
                    fireDelay = 0;
                }
            }
            else
            {
                nav.speed = 3f;
            }
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }   
}
