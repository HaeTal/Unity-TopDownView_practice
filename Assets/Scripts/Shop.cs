using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    //소모품, 소모품, 소모품, 무기, 아이템 생성

    public GameObject[] shopItemHolders = new GameObject[5];

    public Item[] shopItems_Normal = new Item[3];       // 소모품 : 체력, 탄약, 열쇠
    public Gun[] shopItems_Weapon;
    public Item[] shopItems_Item;

    void Awake()
    {
        int random;
        Item item;

        for (int i = 0; i < 3; i++)
        {
            random = Random.Range(0, 3);

            item = Instantiate(shopItems_Normal[random], shopItemHolders[i].transform.position, Quaternion.Euler(90, 0, 0), shopItemHolders[i].transform) as Item;
            item.isShopItem = true;
        }

        random = Random.Range(0, shopItems_Weapon.Length);
        Gun gun = Instantiate(shopItems_Weapon[random], shopItemHolders[3].transform.position, Quaternion.Euler(90, 0, 0), shopItemHolders[3].transform) as Gun;
        gun.isShopItem = true;


        random = Random.Range(0, shopItems_Item.Length);
        item = Instantiate(shopItems_Item[random], shopItemHolders[4].transform.position, Quaternion.Euler(90, 0, 0), shopItemHolders[4].transform) as Item;
        item.isShopItem = true;
    }

    
    void Update()
    {
        
    }
}
