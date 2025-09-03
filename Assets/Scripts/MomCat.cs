using Unity.VisualScripting;
using UnityEngine;


public class MomCat : MonoBehaviour
{
    public float moveSpeed = 50f;
    private Rigidbody rb;

    public float mouseMovementSpeedX = 250f;
    public float mouseMovementSpeedY = 250f;
    public Transform playerCamera;
    public float mouseXRotationLimit = 50f;
    private float xRotation = -50f;

    public float jumpForce = 5f;
    private bool isGrounded = true;

    public GameObject[] legs;
    private bool isMove = false;
    private bool isPrevMove = false;

    private bool isInteracting = false;
    public bool canInteracting = false;
    public GameObject interactObject = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.instance.PC = this;

        rb = GetComponent<Rigidbody>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMove();
        HandleJump();
        HandleInteract();
    }

    private void LateUpdate()
    {
        HandleMouseLook();
    }

    void HandleMove()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 forward = playerCamera.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = playerCamera.right;
        right.y = 0;
        right.Normalize();

        Vector3 moveDir = (forward * moveZ + right * moveX).normalized;

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

        if (collision.gameObject.name == "Fish" && isInteracting == false)
        {
            interactObject = collision.gameObject;
            canInteracting = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name == "Fish" && isInteracting == false)
        {
            interactObject = null;
            canInteracting = false;
        }
    }

    void HandleInteract()
    {
        if (Input.GetKeyDown(KeyCode.E) == false)
            return;

        // TODO : Interact Object
        Interact();
    }

    void SetLegMovement(bool isActive)
    {
        isPrevMove = isMove;
        foreach (GameObject leg in legs)
        {
            leg.GetComponent<MomCatLegMove>().isActive = isActive;
        }
    }

    void Interact()
    {
        if (canInteracting == false)
            return;

        if (isInteracting == false)
        {
            isInteracting = true;
            // 물기
            Transform attachPoint = transform.Find("FishOffset");
            interactObject.transform.SetParent(attachPoint);

            Rigidbody interactRb = interactObject.GetComponent<Rigidbody>();
            interactRb.isKinematic = true;

            BoxCollider interactBC = interactObject.GetComponent<BoxCollider>();
            interactBC.isTrigger = true;

            Debug.Log("Enter");
        }
        else
        {
            isInteracting = false;
            // 놓아주기
            interactObject.transform.SetParent(null);

            Rigidbody interactRb = interactObject.GetComponent<Rigidbody>();
            interactRb.isKinematic = false;

            BoxCollider interactBC = interactObject.GetComponent<BoxCollider>();
            interactBC.isTrigger = false;
        }
    }
}