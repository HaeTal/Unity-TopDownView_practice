using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using UnityEngine.Animations.Rigging;

public class Player : MonoBehaviour
{
    Camera mainCam;
    
    Rigidbody rigid;
    
    public Vector3 heightCorrectedPoint { get; private set; }       // 조준점
    public Transform crosshairs;
    Ray ray;
    
    float h;
    float v;
    Vector3 moveVec;
    Vector3 dodgeVec;

    [Header("Player Status")]
    public float speed = 8f;
    public float finalSpeed;
    public int maxHealth = 10;
    public int finalMaxHeart;
    public int curHealth = 5;


    [Header("Inventory")]
    public GameObject firstGun;
    public List<Item> itemList = new List<Item>();
    public List<GameObject> gunList = new List<GameObject>();
    public int coin;
    public int key;


    [Header("Item Status")]         // 최대 체력, 이동 속도는 player 스크립트  |  데미지, 반동은 gun 스크립트에서 관리
    public GameObject nearObject;
    public GameObject itemParent;

    // 퍼센트 증가
    public float addDamage;
    public float addAccuracy;
    public float addMag;
    public float addSpeed;
    public int addMaxHeart;
    

    [Header("Animation")]
    public UnityEngine.Animations.Rigging.Rig handIk;
    Animator anim;
    AnimatorOverrideController overrides;
    public Rig aimLayer;
    public Transform gunParent;
    public Transform gunLeftGrip;
    public Transform gunRightGrip;

    [Header("Weapon")]
    
    public Gun equipGun;
    public int gunIndex = 0;
    float fireDelay;
    BlankBullet blankBullet;
    public int hasBlanks = 10;

    private KeyCode[] keyCodes = 
        {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
            KeyCode.Alpha6,
            KeyCode.Alpha7,
            KeyCode.Alpha8,
            KeyCode.Alpha9,
        };



    bool fireDown;              // 사격
    bool reloadDown;            // 재장전
    bool dodgeDown;             // 회피
    public bool blankDown;             // 공포탄
    bool interDown;             // 상호작용
    
    bool itemThrowDown;

    bool throwDown;             // 버리기
    bool isLongKeyDown;
    float keyDownTime = 0f;

    
    public bool isDead;
    bool isBorder;
    bool isHit;
    bool isDodge;
    bool isFireReady;
    bool isReload;
    bool isBlank;

    void Start()
    {
        mainCam = Camera.main;
        anim = GetComponentInChildren<Animator>();
        overrides = anim.runtimeAnimatorController as AnimatorOverrideController;
        rigid = GetComponent<Rigidbody>();
        equipGun = GetComponentInChildren<Gun>();
        blankBullet = GetComponent<BlankBullet>();

        Equip(Instantiate(firstGun, transform.position, transform.rotation).GetComponent<Gun>());

        finalSpeed = speed;
        finalMaxHeart = maxHealth;
    }

    
    void Update()
    {
        if (!isDead && GameManager.Instance.Pause == false)
        {
            GetInput();
            StopToWall();

            
            Shot();
            Reload();
            BlankBullet();

            Interaction();

            Swap();
            ThrowGun();
            Unequip();


            if (!equipGun)
            {
                handIk.weight = 0.0f;
                anim.SetLayerWeight(1, 0.0f);
            }
        }
    }

    private void FixedUpdate()
    {
        
        
        Move();
        Turn();
        Dodge();
    }

    private void LateUpdate()
    {
        Aim();
    }

    void GetInput()
    {
        h = Input.GetAxisRaw("Horizontal"); anim.SetFloat("Horizontal", h);
        v = Input.GetAxisRaw("Vertical"); anim.SetFloat("Vertical", v);
        ray = mainCam.ScreenPointToRay(Input.mousePosition);

        fireDown = Input.GetKey(KeyCode.Mouse0);
        dodgeDown = Input.GetKey(KeyCode.Mouse1);
        reloadDown = Input.GetKeyDown(KeyCode.R);
        blankDown = Input.GetKeyDown(KeyCode.Q);
        interDown = Input.GetKeyDown(KeyCode.E);
        itemThrowDown = Input.GetKeyDown(KeyCode.G);

        if (Input.GetKey(KeyCode.F))
        {
            throwDown = true;
        }
        else
        {
            throwDown = false;
        }
    }

    void Swap()
    {
        for (int i = 0; i < keyCodes.Length; i++)
        {
            if (Input.GetKeyDown(keyCodes[i]))
            {
                //int numberPressed = i + 1;
                int numberPressed = i;
                Debug.Log(numberPressed);

                if(numberPressed < gunIndex)
                {
                    equipGun.gameObject.SetActive(false);
                    equipGun.enabled = false;
                    equipGun = gunList[numberPressed].GetComponent<Gun>();
                    equipGun.gameObject.SetActive(true);
                    equipGun.enabled = true;
                    Invoke(nameof(SetAnimationDelayed), 0.001f);
                }
            }
        }   
    }


    // 무기 버릴 시 강제 스왑
    void Swap(int index)
    {
        if (index > -1)
        {
            equipGun.gameObject.SetActive(false);
            equipGun.enabled = false;
            equipGun = gunList[index].GetComponent<Gun>();
            equipGun.gameObject.SetActive(true);
            equipGun.enabled = true;
            Invoke(nameof(SetAnimationDelayed), 0.001f);
        }
    }

    void ThrowGun()
    {
        if (throwDown)
        {
            keyDownTime += Time.deltaTime;
            if (1 < keyDownTime) isLongKeyDown = true;
        }

        if (isLongKeyDown && gunIndex > 1)
        {
            isLongKeyDown = false;
            keyDownTime = 0;

            Gun throwGun = equipGun.GetComponent<Gun>();
            throwGun.player = null;

            Instantiate(equipGun, transform.position, transform.rotation);

            gunList.Remove(equipGun.gameObject);
            GunIndexSet(equipGun.gunIndex);
            Destroy(equipGun.gameObject);

            Swap(0);
            
            gunIndex--;
        }      
    }

    void GunIndexSet(int index)
    {
        if(index != gunIndex)
        {
            for (int i = index; i < gunList.Count; i++)
            {
                gunList[i].GetComponent<Gun>().gunIndex--;
            }
        }
    }

    void Move()
    {
        moveVec = new Vector3(h, 0, v).normalized;

        if (!isBorder)
        {
            if (isDodge)
            {
                moveVec = dodgeVec;
            }
           
            rigid.MovePosition(transform.position + moveVec * finalSpeed * Time.deltaTime);
        }
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec);

        if(!isDodge)
        {
            RaycastHit rayHit;

            if (Physics.Raycast(ray, out rayHit, 100))
            {

                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 0;
                heightCorrectedPoint = transform.position + nextVec;
                transform.LookAt(heightCorrectedPoint);
            }
        }
    }

    void Aim()
    {
        Plane groundPlane = new Plane(Vector3.up, Vector3.up * 1.5f);
        float rayDistance;

        if(groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            crosshairs.position = point;
        }
    }

    public void Equip(Gun newGun)
    {
        if (equipGun)           // 기존 착용한 무기 비활성화
        {
            equipGun.gameObject.SetActive(false);
            equipGun.enabled = false;
        }

        equipGun = newGun.GetComponent<Gun>();
        equipGun.enabled = true;
        equipGun.player = this;
        equipGun.transform.parent = gunParent;
        equipGun.transform.localPosition = Vector3.zero;
        equipGun.transform.localRotation = Quaternion.identity;

        handIk.weight = 1.0f;
        anim.SetLayerWeight(1, 1.0f);

        Invoke(nameof(SetAnimationDelayed), 0.001f);

        gunList.Add(equipGun.gameObject);
        equipGun.gunIndex = gunIndex;
        gunIndex++;

        equipGun.OnItem();
    }

    void SetAnimationDelayed()
    {
        overrides["gun_anim_empty"] = equipGun.gunAnimation;
    }

    void Dodge()
    {
        if (dodgeDown && !isDodge && moveVec != Vector3.zero)
        {
            isDodge = true;
            anim.SetTrigger("doDodge");
            dodgeVec = moveVec;
            //rigid.AddForce(Vector3.up * 3f, ForceMode.VelocityChange); //위로 뜨는 힘


            finalSpeed += 3;

            aimLayer.weight = 1;
            
            Invoke("DodgeOut", 0.6f);
        }
    }

    void DodgeOut()
    {
        aimLayer.weight = 0;
        finalSpeed -= 3;
        isDodge = false;
    }

    void Shot()
    {
        if (equipGun == null)
            return;

        fireDelay += Time.deltaTime;
        isFireReady = equipGun.fireRate < fireDelay;

        if (fireDown && isFireReady && !isReload && !isDodge && equipGun != null && equipGun.curMagAmmo > 0)
        {

            equipGun.Shot();
            AudioManager.Instance.PlaySound(equipGun.shootAudio, transform.position, true);
            
            fireDelay = 0;
        }
    }

    void Reload()
    {
        if (equipGun == null || equipGun.ammo <= 0 || equipGun.curMagAmmo == equipGun.finalMagAmmo)
            return;

        if (reloadDown && !isDodge && !isReload)
        {
            Debug.Log("reloadTime: " + equipGun.reloadtime);
            isReload = true;
            AudioManager.Instance.PlaySound(equipGun.reloadAudio, transform.position, true);
            Invoke("ReloadOut", equipGun.reloadtime);
        }
    }

    void ReloadOut()
    {
        int needAmmo = equipGun.ammo < equipGun.finalMagAmmo ? equipGun.ammo : equipGun.finalMagAmmo - equipGun.curMagAmmo;

        equipGun.curMagAmmo = equipGun.curMagAmmo + needAmmo;
        equipGun.ammo -= needAmmo;

        isReload = false;
    }

    void BlankBullet()
    {
        if (hasBlanks < 1) return;

        if(blankDown && !isBlank)
        {
            Debug.Log("BlankDown");
            hasBlanks--;
            isBlank = true;
            blankBullet.Use();
            Invoke("BlankBulletOut", 1);
        }
        
    }

    void BlankBulletOut()
    {
        isBlank = false;
    }

    void Equip(Item item)
    {
        item.transform.parent = itemParent.transform;
        
        item.gameObject.SetActive(false);
        itemList.Add(item);

        // 속도 증가
        addSpeed = speed * item.addSpeed;
        finalSpeed = finalSpeed + addSpeed;

        // 최대 체력 증가
        addMaxHeart = addMaxHeart + item.addMaxHeart;
        finalMaxHeart = maxHealth + addMaxHeart;


        addDamage += item.addDamage;
        addMag += item.addMag;
        addAccuracy += item.addAccuracy;
        

        if (equipGun)
        {
            equipGun.OffItem();
            equipGun.OnItem();
        }
    }

    void Unequip()
    {
        if(itemThrowDown && itemList.Count > 0)
        {
            GameObject throwItemObject = itemParent.transform.GetChild(0).gameObject;
            Item throwItem = throwItemObject.GetComponent<Item>();

            addSpeed = speed * throwItem.addSpeed;
            finalSpeed = finalSpeed - addSpeed;

            addMaxHeart = addMaxHeart - throwItem.addMaxHeart;
            finalMaxHeart = finalMaxHeart - throwItem.addMaxHeart;

            addDamage -= throwItem.addDamage;
            addAccuracy -= throwItem.addAccuracy;
            addMag -= throwItem.addMag;
                   

            throwItemObject = Instantiate(throwItemObject, gameObject.transform.position, Quaternion.Euler(90, 0, 0));
            throwItemObject.SetActive(true);
            throwItemObject.transform.localPosition = gameObject.transform.position + Vector3.up;
            Destroy(itemParent.transform.GetChild(0).gameObject);
            itemList.RemoveAt(0);

            if (equipGun)
            {
                equipGun.OffItem();
                equipGun.OnItem();
            } 
        }
    }

    void Interaction()
    {
        if(interDown && nearObject != null && !isDodge && !isDead)
        {
            if (nearObject.tag == "Item")
            {
                Item item = nearObject.GetComponent<Item>();

                if (item.isShopItem)
                {
                    if (coin < item.price)
                        return;

                    else
                    {
                        coin -= item.price;
                        item.isShopItem = false;
                    }
                }

                switch (item.type)
                {
                    case Item.Type.Ammo:
                        if (!equipGun) return;
                        equipGun.ammo = equipGun.maxAmmo;
                        Destroy(nearObject.gameObject);
                        break;

                    case Item.Type.Heart:
                        curHealth += item.value;
                        if (curHealth > maxHealth)
                            curHealth = maxHealth;
                        Destroy(nearObject.gameObject);

                        break;

                    case Item.Type.BlankBullet:
                        hasBlanks++;
                        Destroy(nearObject.gameObject);

                        break;

                    case Item.Type.Key:
                        key++;
                        Destroy(nearObject.gameObject);

                        /*UIManager.Instance.Key();*/
                        break;

                    case Item.Type.Passive:
                        Equip(item);
                        break;
                }
                GameManager.Instance.OffItemPanel();
                nearObject = null;
            }

            else if(nearObject.tag == "Gun")
            {
                Gun gun = nearObject.GetComponentInChildren<Gun>();  

                if (gun.isShopItem == true && coin > gun.price)     // 구매 가능
                {
                    gun.isShopItem = false;
                    coin -= gun.price;
                }

                else if(gun.isShopItem == true && coin < gun.price)     // coin 부족
                {
                    return;
                }

                //gun.isShopItem == false 인 경우
                gun = Instantiate(nearObject.GetComponentInChildren<Gun>());
                Equip(gun);
                Destroy(nearObject.gameObject);
            }

            else if(nearObject.tag == "Box" && key > 0)
            {
                key--;

                Box box = nearObject.GetComponentInChildren<Box>();
                if (box.isOpen)
                    return;

                box.Open();
            }

            else if(nearObject.tag == "Door")
            {
                nearObject.SetActive(false);
            }
        }
    }

    void StopToWall()
    {
        Debug.DrawRay(transform.position, moveVec * 5f, Color.white);
        isBorder = Physics.Raycast(transform.position, moveVec, 0.1f, LayerMask.GetMask("Wall"));
    }

    void HitOut()
    {
        isHit = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Gun" || other.tag == "Item" || other.tag == "Box" || other.tag == "Door" || other.tag == "Desk")
        {

            // 윤곽선 처리
            if(other.tag == "Item")
            {
                other.GetComponent<SpriteOutline>().directions = new SpriteOutline.Directions(true, true, true, true);
                
                if(other.GetComponent<Item>().isShopItem)
                    GameManager.Instance.OnItemPanel(other.GetComponent<Item>(), other.transform.position);
            }

            else if(other.tag == "Gun")
            {
                if (other.GetComponentInChildren<Gun>().isShopItem)
                    GameManager.Instance.OnItemPanel(other.GetComponentInChildren<Gun>(), other.transform.position);
            }

            else if(other.tag == "Box")
            {
                other.GetComponent<Outline>().OutlineWidth = 3f;
            }

            nearObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Gun" || other.tag == "Item" || other.tag == "Box")
        {
            nearObject = null;

            // 윤곽선 처리
            if (other.tag == "Item")
            {
                other.GetComponent<SpriteOutline>().directions = new SpriteOutline.Directions(false, false, false, false);
            }

            else if (other.tag == "Box")
            {
                other.GetComponent<Outline>().OutlineWidth = 0f;
            }

            GameManager.Instance.OffItemPanel();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Coin")
        {
            Item coinObject = other.GetComponent<Item>();
            coin += coinObject.value;
            Destroy(coinObject.gameObject);
        }

        if (!isDodge && !isHit)
        {
            if (other.tag == "EnemyBullet")
            {
                Debug.Log("Hit by EnemyBullet");
                Destroy(other.gameObject);
                curHealth--;

                isHit = true;
            }

            else if(other.tag == "Enemy")
            {
                Debug.Log("Hit by Enemy");

                Vector3 reactVec = (transform.position - other.transform.position).normalized;
                rigid.AddForce(reactVec * 15, ForceMode.Impulse);
                curHealth--;

                isHit = true;
            }

            if (curHealth <= 0 && !isDead)
            {
                anim.SetTrigger("doDie");
                isDead = true;
                rigid.isKinematic = true;
                Destroy(gameObject, 2);

                GameManager.Instance.Gameover();
            }

            Invoke("HitOut", 1f);
        }

        
    }
}
