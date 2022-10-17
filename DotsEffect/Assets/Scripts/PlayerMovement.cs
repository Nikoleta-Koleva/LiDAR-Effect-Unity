using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Vector3 moveInput;
    private Vector2 mouseInput;

    private float rotationX;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform ground;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform playerCamera;
    [Space]
    [SerializeField] private float speed;
    [SerializeField] private float sensitivity;
    [SerializeField] private float jumpForce;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        // Get input data
        moveInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        movePlayer();
        moveCamera();
    }

    private void movePlayer()
    {
        Vector3 MoveVector = transform.TransformDirection(moveInput) * speed;
        rb.velocity = new Vector3(MoveVector.x, rb.velocity.y, MoveVector.z);

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
           // if (Physics.CheckSphere(ground.position, 0.01f, groundLayer))
           // {
          //      rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
          //  }
        //}
    }

    private void moveCamera()
    {
        rotationX -= mouseInput.y * sensitivity;

        transform.Rotate(0f, mouseInput.x * sensitivity, 0f);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);    
        rotationX = Mathf.Clamp(rotationX, -90, 90);

    }
}
