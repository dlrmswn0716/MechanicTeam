using UnityEngine;


public class MomCat : MonoBehaviour
{
    public float moveSpeed = 50f;
    private Rigidbody rb;

    public float mouseMovementSpeedX = 250f;
    public float mouseMovementSpeedY = 250f;
    public Transform playerCamera;
    public float mouseXRotationLimit = 50f;
    private float xRotation = 0f;

    public float jumpForce = 5f;
    private bool isGrounded = true;

    public GameObject[] legs;
    private bool isMove = false;
    private bool isPrevMove = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        xRotation = playerCamera.eulerAngles.x;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMove();
        HandleJump();
        HandleInteracte();
    }

    private void LateUpdate()
    {
        HandleMouseLook();
    }

    void HandleMove()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = new Vector3(moveX, 0, moveZ).normalized;

        rb.MovePosition(transform.position + moveDir * moveSpeed * Time.deltaTime);

        if (moveX != 0f || moveZ != 0f)
            isMove = true;
        else
            isMove = false;

        if(isMove != isPrevMove && isGrounded == true)
        {
            SetLegMovement(isMove);
        }
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseMovementSpeedX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseMovementSpeedY * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -mouseXRotationLimit, mouseXRotationLimit);

        playerCamera.rotation = Quaternion.Euler(xRotation, playerCamera.eulerAngles.y, playerCamera.eulerAngles.z);
    }

    void HandleJump()
    {
        if ((Input.GetKeyDown(KeyCode.Space) && isGrounded == true) == false)
            return;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        
        isGrounded = false;
        isMove = false;
        SetLegMovement(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Untagged"))
        {
            isGrounded = true;
            HandleMove();
        }
    }

    void HandleInteracte()
    {
        if (Input.GetKeyDown(KeyCode.E) == false)
            return;

        // TODO : Interact Object
    }

    void SetLegMovement(bool isActive)
    {
        isPrevMove = isMove;
        foreach (GameObject leg in legs)
        {
            leg.GetComponent<MomCatLegMove>().isActive = isActive;
        }
    }

}
