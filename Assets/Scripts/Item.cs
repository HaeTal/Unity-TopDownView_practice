using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string displayName;
    public enum Type
    {
        Ammo, Coin, Key, BlankBullet, Heart, Passive, Active
    }

    public Type type;
    public int value;       // ÃÑ¾Ë, µ¿Àü, ÇÏÆ® È¹µæ·®

    public GameObject player;
    public bool roomIsClear;

    [Header("Passive | Active")]
    public int itemIndex;
    public float addSpeed;
    public float addDamage;
    public float addAccuracy;
    public float addMag;
    public int addMaxHeart;

    [Header("ShopItem")]
    public bool isShopItem;
    public int price;


    Rigidbody rigid;
    BoxCollider boxCollider;


    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();

        if(type == Type.Coin)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    private void Update()
    {
        if(type == Type.Coin && roomIsClear)
        {
            MoveToPlayer();
        }
    }

    public void MoveToPlayer()
    {
        if (player != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position + Vector3.up, Time.deltaTime * 20);
            
        }
    }




    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor" && type != Type.Coin)
        {
            rigid.isKinematic = true;
            boxCollider.enabled = false;
        }
    }

}
