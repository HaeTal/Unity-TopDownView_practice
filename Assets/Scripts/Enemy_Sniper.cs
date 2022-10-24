using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Sniper : Enemy
{
    public Transform bulletStart;
    LineRenderer lineRenderer;

    RaycastHit hit;
    //public int layerMask;

    protected override void Awake()
    {
        base.Awake();

        lineRenderer = GetComponent<LineRenderer>();
    }


    protected override void Update()
    {
        if(nav.enabled && nav.isStopped == false)
        {
            base.Update();
        }
        

        if (nav.enabled && target != null)
        {
            if (nav.enabled && target != null)
            {
                dist = Vector3.Distance(transform.position, target.transform.position);
                StartChase();
            }

            if (dist < 15)
            {
                fireDelay += Time.deltaTime;
                nav.isStopped = true;
                anim.SetBool("isWalk", false);
                transform.LookAt(target.transform.position);
                gun.transform.LookAt(target.transform.position + new Vector3(0, 1.5f, 0));

                if (fireDelay > gun.fireRate)
                {
                    StartCoroutine(SniperAttack());
                    fireDelay = 0;
                }

            }
            else
            {
                nav.isStopped = false;
                anim.SetBool("isWalk", true);
            }
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if(dist < 15)
            DrawLine();
    }

    void DrawLine()
    {
        Vector3 lineTarget;

        lineRenderer.SetPosition(0, bulletStart.position);

        if(Physics.Raycast(bulletStart.position, bulletStart.forward, out hit, 15f))
        {
            lineTarget = hit.point;
            lineRenderer.SetPosition(1, lineTarget);
        }
    }

    IEnumerator SniperAttack()
    {
        gun.StartCoroutine(gun.EnemyShoot(target.transform.position));
        
        yield return null;
    }
}
