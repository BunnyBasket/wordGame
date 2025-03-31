using UnityEngine;

public class Player : MonoBehaviour
{
    private GameObject pickedUpObject = null;
    public float moveSpeed = 5f;
    public float mouseSensitivity = 2f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;

    public Transform cameraTransform;

    private CharacterController controller;
    private float verticalVelocity;
    private float xRotation = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // Gravity
        if (controller.isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;

        if (Input.GetButtonDown("Jump") && controller.isGrounded)
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;

        controller.Move(move * moveSpeed * Time.deltaTime);
    }



void OnControllerColliderHit(ControllerColliderHit hit)
{
    // Check if player is colliding with a tagged object
    if (hit.gameObject.CompareTag("Punctuation"))
    {
        // If Shift is held and no object is picked up yet
        if (Input.GetKey(KeyCode.LeftShift) && pickedUpObject == null)
        {
            pickedUpObject = hit.gameObject;
            pickedUpObject.transform.SetParent(transform); // Make it a child of the player
            pickedUpObject.transform.localPosition = new Vector3(0, 1, 1); // Adjust position in front of player
            Rigidbody rb = pickedUpObject.GetComponent<Rigidbody>();
            if (rb) rb.isKinematic = true; // Optional: disable physics while held
        }
    }
}

// Called every frame to check for drop
void LateUpdate()
{
    if (pickedUpObject != null && !Input.GetKey(KeyCode.LeftShift))
    {
        // Drop the object
        pickedUpObject.transform.SetParent(null);
         Vector3 dropPosition = pickedUpObject.transform.position;
        dropPosition.y = 3.417f;
        pickedUpObject.transform.position = dropPosition;

        Rigidbody rb = pickedUpObject.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = false; // Re-enable physics
        pickedUpObject = null;
    }
}
}
