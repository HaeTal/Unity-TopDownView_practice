using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement3D : MonoBehaviour
{
    public float moveSpeed = 5;
    private float gravity = -9.81f;
    private float jumpForce = 3.0f;
    private Vector3 moveDirection;

    private CharacterController characterController;

    public float MoveSpeed
    {
        set => moveSpeed = Mathf.Clamp(value, 2.0f, 5.0f);
    }

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(characterController.isGrounded == false)
        {
            moveDirection.y += gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
    }

    public void MoveTo(Vector3 direction)
    {
        moveDirection = new Vector3(direction.x, moveDirection.y, direction.z);
    }

    public void JumpTo()
    {
        if(characterController.isGrounded == true)
        {
            moveDirection.y = jumpForce;
        }
    }
}
