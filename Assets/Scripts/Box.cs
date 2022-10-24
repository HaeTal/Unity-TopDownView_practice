using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    public enum Type
    {
        D, C ,B ,A, S
    }
    public Type type;
    public List<GameObject> items;
    public Gun[] guns;
    public bool isOpen;

    private void Awake()
    {
        if(type == Type.S)
            items = ItemDB.Instance.S_items;

        else if(type == Type.A)
            items = ItemDB.Instance.A_items;

        else if (type == Type.B)
            items = ItemDB.Instance.B_items;

        else if (type == Type.C)
            items = ItemDB.Instance.C_items;

        else if (type == Type.D)
            items = ItemDB.Instance.D_items;

    }

    public void Open()
    {
        int num = Random.Range(0, items.Count); ;
        
        Instantiate(items[num], gameObject.transform.position + new Vector3(1, 1, 0), Quaternion.Euler(90,0,0));
        isOpen = true;
        Destroy(gameObject, 0.5f);
    }
}
