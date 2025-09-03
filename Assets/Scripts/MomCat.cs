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

    private bool isFirstMouseClicked = false;

    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

    private const int MOUSEEVENTF_LEFTDOWN = 0x02;
    private const int MOUSEEVENTF_LEFTUP = 0x04;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //TO-DO : 임시
        GameManager.instance.Init();

        rb = GetComponent<Rigidbody>();

        xRotation = 0f;
        playerCamera.localRotation = Quaternion.Euler(0f, 0f, 0f);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

    }

    // Update is called once per frame
    void Update()
    {
        HandleMove();
        HandleJump();
        HandleInteract();
        HandleMouseLook();
        HandleMouseClick();
    }

    void HandleMove()
    {
        Vector2 move = InputManager.instance.GetMoveInput();
        float moveX = move.x;
        float moveZ = move.y;

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
        if (isFirstMouseClicked == false)
        {
            isFirstMouseClicked = true;
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            return;
        }

        Vector2 mouse = InputManager.instance.GetMouseInput();
        float mouseX = mouse.x * mouseMovementSpeedX * Time.deltaTime;
        float mouseY = mouse.y * mouseMovementSpeedY * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -mouseXRotationLimit, mouseXRotationLimit);

        playerCamera.rotation = Quaternion.Euler(xRotation, playerCamera.eulerAngles.y, playerCamera.eulerAngles.z);
    }

    void HandleJump()
    {
        bool isJumpInput = InputManager.instance.GetJumpInput();
        if ((isJumpInput && isGrounded) == false)
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

        if (collision.gameObject.name.Contains("Fish") && isInteracting == false)
        {
            interactObject = collision.gameObject;
            canInteracting = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name.Contains("Fish") && isInteracting == false)
        {
            interactObject = null;
            canInteracting = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Achievement"))
            GameManager.instance.Achievement = true;
    }

    void HandleInteract()
    {
        if (InputManager.instance.GetInteractInput() == false)
            return;

        // TODO : Interact Object
        Interact();
    }

    void HandleMouseClick()
    {
        if (InputManager.instance.GetMouseClick())
            isFirstMouseClicked = true;
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

        if (interactObject.name.Contains("Fish"))
        {
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

                interactObject.transform.position = attachPoint.position;

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
}