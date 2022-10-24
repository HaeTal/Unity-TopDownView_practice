using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Enemy : MonoBehaviour
{
    public Gun gun;
    
    void Start()
    {
        gun = GetComponentInChildren<Gun>();
    }

    
    void Update()
    {
        gun.Shot();
    }
}
