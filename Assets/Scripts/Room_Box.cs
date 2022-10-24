using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room_Box : MonoBehaviour
{
    public GameObject boxHolder;
    public Box[] boxes;

    public void Awake()
    {
        Box box = Instantiate(boxes[Random.Range(0, boxes.Length)], boxHolder.transform.position, boxHolder.transform.rotation);
    }
}
