using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Boss1 : Enemy
{
    public GameObject bulletStart;

    public Bullet pattern1_Bullet;
    public Bullet pattern2_Bullet;
    public Missile pattern3_Missile;


    public bool isPattern;


    protected override void Awake()
    {
        base.Awake();
    }


    protected override void Update()
    {
        if (nav.enabled && nav.isStopped == false)
        {
            base.Update();      // target 찾기


            if (target != null)
            {
                dist = Vector3.Distance(transform.position, target.transform.position);

                StartChase();
                LookTarget();

                if (dist < 20 && !isPattern)
                {
                    int patternNumber = Random.Range(1, 4);
                    Debug.Log(patternNumber);

                    StartCoroutine("Pattern" + patternNumber);
                    //StartCoroutine(Pattern3());
                }
            }
        }
    }



    IEnumerator Pattern1()      // 파동형태 발사
    {
        isPattern = true;

        for (int i = 0; i < 30; i++)
        {
            LookTarget();

            float angle = Mathf.Sin(30 * i * Mathf.Deg2Rad) * 30;

            bulletStart.transform.localRotation = Quaternion.Euler(0, angle, 0);
            Bullet bullet = Instantiate(pattern1_Bullet, bulletStart.transform.position, bulletStart.transform.rotation);
            Rigidbody bulletRigid = bullet.GetComponent<Rigidbody>();
            bulletRigid.velocity = bullet.transform.forward * bullet.speed;

            yield return new WaitForSeconds(0.15f);
        }

        yield return new WaitForSeconds(3f);
        PatternExit();
    }

    IEnumerator Pattern2()      // 부채꼴 형태로 2번 발사
    {
        isPattern = true;

        for(int i = 0; i < 2; i++)
        {
            LookTarget();

            for (int j = 0; j < 15; j++)
            {
                float angle = Mathf.Sin((Mathf.PI * 2 * j / 15)) * 30;

                bulletStart.transform.localRotation = Quaternion.Euler(0, angle, 0);

                Bullet bullet = Instantiate(pattern2_Bullet, bulletStart.transform.position, bulletStart.transform.rotation);
                Rigidbody bulletRigid = bullet.GetComponent<Rigidbody>();

                bulletRigid.velocity = bullet.transform.forward * bullet.speed;
            }
            yield return new WaitForSeconds(2f);
        }

        PatternExit();
    }

    IEnumerator Pattern3()      // 미사일 발사
    {
        isPattern = true;
        Missile[] missile = new Missile[5];
        LookTarget();
        nav.isStopped = true;
        anim.SetBool("isWalk", false);
        for (int i = 0; i < missile.Length; i++)
        {
            missile[i] = Instantiate(pattern3_Missile, transform.position, Quaternion.identity);
            Rigidbody rigid = missile[i].GetComponent<Rigidbody>();
            rigid.AddForce(Vector3.up * 2000);

            yield return new WaitForSeconds(0.5f);
        }     

        nav.isStopped = false;
        anim.SetBool("isWalk", true);
        for (int i = 0; i < missile.Length; i++)
        {
            missile[i].isDown = true;

            float random = Random.Range(0, 3);
            missile[i].DrawRange(target.transform.position + new Vector3(random, 0, random));
            missile[i].transform.position = new Vector3(target.transform.position.x + random, 40, target.transform.position.z + random);
            missile[i].transform.localRotation = Quaternion.Euler(180, 0, 0);

            Rigidbody rigid = missile[i].GetComponent<Rigidbody>();
            rigid.AddForce(Vector3.down * 4000);

            yield return new WaitForSeconds(0.5f);
        }

        PatternExit();

        yield return new WaitForSeconds(4f);
    }

    IEnumerator Pattern4()      // 부채꼴 형태로 2번 발사
    {
        isPattern = true;

        int bulletNum = 60;

        for (int i = 0; i < 2; i++)
        {
            LookTarget();

            for (int j = 0; j < bulletNum; j++)
            {

                float angle = Mathf.Sin((Mathf.PI * 2 * j / bulletNum)) * 180;

                //Debug.Log(Mathf.Sin(30 * j * Mathf.Deg2Rad));

                bulletStart.transform.localRotation = Quaternion.Euler(0, angle, 0);

                Bullet bullet = Instantiate(pattern2_Bullet, bulletStart.transform.position, bulletStart.transform.rotation);
                Rigidbody bulletRigid = bullet.GetComponent<Rigidbody>();

                bulletRigid.velocity = bullet.transform.forward * bullet.speed;

                //yield return new WaitForSeconds(0.2f);

            }

            yield return new WaitForSeconds(2f);
        }

        PatternExit();
    }


    void PatternExit()
    {
        bulletStart.transform.LookAt(this.transform.forward);
        isPattern = false;
    }
}
