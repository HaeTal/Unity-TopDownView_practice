using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject spawnEntity;

    public void Spawn()
    {
        Instantiate(spawnEntity, transform.position, transform.rotation, this.transform.parent);
    }
}
