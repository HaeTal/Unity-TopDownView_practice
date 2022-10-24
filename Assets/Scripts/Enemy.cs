using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float maxHealth = 100;
    public float curHealth = 100;
    protected float fireDelay = 0;
    public float fireRate;

    public bool isChase;
    public bool isAttack;
    public bool isDead;
    public float dist;
    protected bool hasTarget
    {
        get
        {
            if (target != null)
                return true;
            return false;
        }
    }

    public Player target;
    public LayerMask targetLayer;
    protected NavMeshAgent nav;
    protected Gun gun;

    protected Rigidbody rigid;
    protected CapsuleCollider capsuleCollider;
    protected MeshRenderer[] meshs;
    protected Animator anim;

    public Item coin;
    public ParticleSystem deathEffect;

    protected virtual void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider> ();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        gun = GetComponentInChildren<Gun>();

        if(deathEffect)
            deathEffect.GetComponent<ParticleSystemRenderer>().material = this.GetComponentInChildren<SkinnedMeshRenderer>().material;
    }

    protected virtual void Update()
    {
        fireDelay += Time.deltaTime;

        if(target == null)
        {
            StartCoroutine(FindTarget());
        }        
    }

    protected virtual void FixedUpdate()
    {
        FreezeVelocity();
    }

    IEnumerator FindTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 40f, targetLayer);
        if(colliders.Length > 0)
        {
            for(int i = 0; i < colliders.Length; i++)
            {
                Player targetPlayer = colliders[i].GetComponent<Player>();
                if(targetPlayer != null && !targetPlayer.isDead)
                {
                    nav.enabled = true;
                    target = targetPlayer;
                    isChase = true;
                    anim.SetBool("isWalk", true);
                }
            }
        }

        else
        {
            target = null;

            yield return new WaitForSeconds(10f);
        }

        yield return new WaitForSeconds(1f);
    }

    protected void StartChase()
    {
        nav.SetDestination(target.transform.position);
    }

    protected IEnumerator Attack()
    {
        if (target != null)
        {
            nav.isStopped = true;

            anim.SetBool("isWalk", false);

            LookTarget();
            yield return new WaitForSeconds(1f);

            gun.StartCoroutine(gun.EnemyShoot(target.transform.position));

            nav.isStopped = false;
            anim.SetBool("isWalk", true);
        }
    }

    protected void FreezeVelocity()
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero; 
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bullet")
        {
            float gunDamage = other.GetComponent<Bullet>().damage;
            curHealth -= gunDamage;
            Vector3 reactVec = (transform.position - other.transform.position).normalized;

            rigid.AddForce(-reactVec * gunDamage, ForceMode.Impulse);

            Destroy(other.gameObject);
            StartCoroutine(OnDamage(reactVec));
        }
    }

    IEnumerator OnDamage(Vector3 reactVec)
    {
        yield return null;

        if (curHealth <= 0 && !isDead)
        {  
            gameObject.layer = 11;      // EnemyDead ·¹ÀÌ¾î
            isDead = true;
            isChase = false;
            nav.enabled = false;
            anim.SetTrigger("doDie");
            rigid.isKinematic = true;

            if(deathEffect != null)
            {
                Destroy(Instantiate(deathEffect.gameObject, transform.position, Quaternion.FromToRotation(Vector3.forward, -reactVec)) as GameObject, 2);
            }

            if (coin)
            {
                Instantiate(coin, transform.position + Vector3.up, Quaternion.Euler(90, 0, 0));
            }

            if (GetComponentInParent<Room>())
            {
                Room room = GetComponentInParent<Room>();
                room.enemyCount--;
                GameManager.Instance.killCount++;
            }

            Destroy(gameObject);
        }
    }

    public void LookTarget()
    {
        if (target == null)
            return;

        transform.LookAt(target.transform.position);
        gun.transform.LookAt(target.transform.position + new Vector3(0, 1.5f, 0));
    }
}

   