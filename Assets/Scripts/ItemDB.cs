using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDB : MonoBehaviour
{
    private static ItemDB instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static ItemDB Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }

    public List<GameObject> S_items = new List<GameObject>();
    public List<GameObject> A_items = new List<GameObject>();
    public List<GameObject> B_items = new List<GameObject>();
    public List<GameObject> C_items = new List<GameObject>();
    public List<GameObject> D_items = new List<GameObject>();
    public List<GameObject> common_items = new List<GameObject> ();
}
