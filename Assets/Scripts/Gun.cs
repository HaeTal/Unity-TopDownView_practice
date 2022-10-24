using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    Camera mainCam;
    Vector3 heightCorrectedPoint;
    public Player player;

    public AnimationClip gunAnimation;
    public GameObject pickUpSphere;

    public enum Type { pistol, shotgun}
    public Type type;

    [Header("[Gun Status]")]
    public string displayName;
    public int gunIndex;
    public int ammo;
    public int maxAmmo;
    public int curMagAmmo;
    public int magMaxAmmo;
    public int finalMagAmmo;
    public float reloadtime;

    public int shotgunPellet;
    public float fireRate;
    public float recoil;
    public float finalRecoil;

    public int price;
    public bool isShopItem;

    [Header("[Sound]")]
    public AudioClip shootAudio;
    public AudioClip reloadAudio;

    [Header("[Bullet]")]
    public GameObject bullet;
    public Transform bulletStart;

    public bool isEnemy;
    public bool hasSphere = false;

    int addMag;
    float accuracyChange;

    public float GunHeight { get { return transform.position.y; } }

    public Sprite gunSprite;

    public void Awake()
    {
        mainCam = Camera.main;
        finalMagAmmo = magMaxAmmo;
        finalRecoil = recoil;
    }

    private void OnDisable()
    {
        OffItem();
    }

    public void Shot()
    {
        StartCoroutine(Shoot());
    }

    public void OnItem()
    {
        if (player != null)
        {
            //Debug.Log("OnItem");

            //아이템 - 탄창 증가
            addMag = (int)(magMaxAmmo * player.addMag);
            finalMagAmmo = magMaxAmmo + addMag;

            //아이템 - 명중률 증가
            accuracyChange = recoil * player.addAccuracy;
            finalRecoil = recoil - accuracyChange;
        }
    }

    public void OffItem()
    {
        if (player != null)
        {
            //Debug.Log("OffItem");

            //아이템 - 탄창 감소
            addMag = 0;
            finalMagAmmo = magMaxAmmo;

            //아이템 - 명중률 감소
            accuracyChange = 0;
            finalRecoil = recoil;
        }
    }

    private void Update()
    {
        if (player || isEnemy)
        {
            if (player)
            {
                if(hasSphere)
                    hasSphere = false;
                Turn();
                transform.LookAt(heightCorrectedPoint);
            }
        }

        else if (!player && !isEnemy && !hasSphere )
        {
            GameObject sphere = Instantiate(pickUpSphere, transform.position, transform.rotation);
            hasSphere = true;
            gameObject.transform.parent = sphere.transform;
            gameObject.transform.parent.localPosition += Vector3.up * 0.5f;
            gameObject.transform.parent.rotation = Quaternion.Euler(0, 90, 90);
            gameObject.transform.localPosition = Vector3.zero;

            gameObject.transform.parent.name = "Weapon_" + gameObject.name; 
        }
    }

    

    IEnumerator Shoot()
    {
        curMagAmmo--;
        
        if (finalRecoil < 0)
        {
            finalRecoil = 0;
        }

        // 일반
        if (type == Type.pistol)
        {
            Vector3 bulletVec = new Vector3(heightCorrectedPoint.x, bulletStart.position.y, heightCorrectedPoint.z);

            if ((transform.position - heightCorrectedPoint).sqrMagnitude > 4)   // 총과 에임의 거리가 4보다 멀면 반동 추가
                bulletVec = bulletVec + new Vector3(Random.Range(-finalRecoil, finalRecoil), 0, Random.Range(-finalRecoil, finalRecoil));                       
            else
            {
                float dist = Vector3.Distance(transform.position, heightCorrectedPoint);
                Debug.Log(dist);
                bulletVec = bulletVec + new Vector3(Random.Range(-finalRecoil * dist * 0.1f, finalRecoil * dist * 0.1f), 0, Random.Range(-finalRecoil * dist * 0.1f, finalRecoil * dist * 0.1f));
            }

            bulletStart.LookAt(bulletVec);

            GameObject instant = Instantiate(bullet, bulletStart.position, bulletStart.rotation);
            Bullet instantBullet = instant.GetComponent<Bullet>();
            instantBullet.damage += instantBullet.damage * player.addDamage;        // 아이템으로 인한 총알 데미지 증가

            Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
            
            bulletRigid.velocity = bulletStart.forward * instantBullet.GetComponent<Bullet>().speed;

            yield return null;
        }
        

        // 샷건
        else if(type == Type.shotgun)
        {
            GameObject[] bullets = new GameObject[8];
            for (int i = 0; i < shotgunPellet; i++)
            {
                Vector3 bulletVec = new Vector3(heightCorrectedPoint.x, bulletStart.position.y, heightCorrectedPoint.z);

                if ((transform.position - heightCorrectedPoint).sqrMagnitude > 4)   // 총과 에임의 거리가 4보다 멀면 반동 추가
                    bulletVec = bulletVec + new Vector3(Random.Range(-finalRecoil, finalRecoil), 0, Random.Range(-finalRecoil, finalRecoil));
                else
                {
                    float dist = Vector3.Distance(transform.position, heightCorrectedPoint);
                    Debug.Log(dist);
                    bulletVec = bulletVec + new Vector3(Random.Range(-finalRecoil * dist * 0.1f, finalRecoil * dist * 0.1f), 0, Random.Range(-finalRecoil * dist * 0.1f, finalRecoil * dist * 0.1f));
                }

                //bulletVec = bulletVec + new Vector3(Random.Range(-finalRecoil, finalRecoil), 0, Random.Range(-finalRecoil, finalRecoil));
                GameObject instant = Instantiate(bullet, bulletStart.position, bulletStart.rotation);
                instant.SetActive(false);
                instant.transform.LookAt(bulletVec);

                Bullet instantBullet = instant.GetComponent<Bullet>();

                instantBullet.damage += instantBullet.damage * player.addDamage;        // 아이템으로 인한 총알 데미지 증가

                
                instantBullet.GetComponent<Rigidbody>().velocity = instantBullet.transform.forward * instantBullet.GetComponent<Bullet>().speed;
                bullets[i] = instant;
                
            }

            for (int i = 0; i < 8; i++)
            {
                bullets[i].SetActive(true);
            }
        }
    }

    public IEnumerator EnemyShoot(Vector3 targetPosition)
    {
        curMagAmmo--;

        // 일반
        if (type == Type.pistol)
        {
            Vector3 bulletVec = new Vector3(targetPosition.x, bulletStart.position.y, targetPosition.z);
            bulletVec = bulletVec + new Vector3(Random.Range(-recoil, recoil), 0, Random.Range(-recoil, recoil));                       // 반동 추가

            
            bulletStart.LookAt(bulletVec);
            GameObject instantBullet = Instantiate(bullet, bulletStart.position, bulletStart.rotation);

            Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();

            
            bulletRigid.velocity = bulletStart.forward * instantBullet.GetComponent<Bullet>().speed;

            yield return null;
        }


        // 샷건
        else if (type == Type.shotgun)
        {
            GameObject[] bullets = new GameObject[8];
            for (int i = 0; i < 8; i++)
            {
                Vector3 bulletVec = new Vector3(targetPosition.x, bulletStart.position.y, targetPosition.z);        // 반동 추가
                bulletVec = bulletVec + new Vector3(Random.Range(-recoil, recoil), 0, Random.Range(-recoil, recoil));
                GameObject instantBullet = Instantiate(bullet, bulletStart.position, bulletStart.rotation);
                instantBullet.SetActive(false);
                instantBullet.transform.LookAt(bulletVec);

                
                instantBullet.GetComponent<Rigidbody>().velocity = instantBullet.transform.forward * instantBullet.GetComponent<Bullet>().speed;
                bullets[i] = instantBullet;


            }

            for (int i = 0; i < 8; i++)
            {
                bullets[i].SetActive(true);
            }
        }

        AudioManager.Instance.PlaySound(shootAudio, transform.position, false);
    }


    void Turn()
    {
        transform.LookAt(transform.position);

        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;

        if (Physics.Raycast(ray, out rayHit, 100))
        {
            Debug.DrawLine(ray.origin, ray.GetPoint(100), Color.red);

            Vector3 nextVec = rayHit.point - transform.position;
            //nextVec.y = 0;
            heightCorrectedPoint = transform.position + nextVec;
            transform.LookAt(heightCorrectedPoint);
        }
    }
}
