using Unity.VisualScripting;
using UnityEngine;


public class MomCat : MonoBehaviour
{
    public float moveSpeed = 50f;
    private Rigidbody rb;

    public float mouseMovementSpeedX = 250f;
    public float mouseMovementSpeedY = 250f;
    public Transform playerCamera;
    private Transform playerCameraPivot;
    private Transform catBody;
    public float mouseXRotationLimit = 50f;
    private float xRotation = -50f;
    public float mouseXRotationMinLimit = -80f;
    public float mouseXRotationMaxLimit = 20f;
    private float cameraPivotBaseRotationY = -180.0f;

    public float jumpForce = 5f;
    private bool isGrounded = true;

    public GameObject[] legs;
    private bool isMove = false;
    private bool isPrevMove = false;

    private bool isInteracting = false;
    public bool canInteracting = false;
    public GameObject interactObject = null;

    private bool isFirstMouseClicked = false;
    private bool isAltActive = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //TO-DO : 임시
        GameManager.instance.Init();

        rb = GetComponent<Rigidbody>();
        playerCameraPivot = transform.Find("CameraPivot");
        catBody = transform.Find("Body");

        xRotation = 0f;
        playerCamera.localRotation = Quaternion.Euler(0f, 180f, 0f);
        playerCamera.localPosition = new Vector3(0, 0, 5);

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

        Vector3 forward = catBody.up;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = catBody.forward;
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
            return;
        }

        Vector2 mouse = InputManager.instance.GetMouseInput();
        float mouseX = mouse.x * mouseMovementSpeedX * Time.deltaTime;
        float mouseY = mouse.y * mouseMovementSpeedY * Time.deltaTime;

        // 카메라와 고양이 캐릭터가 같이 회전
        if (InputManager.instance.GetAltInput() == false)
        {
            // 좌우로 회전한 카메라 원상복구
            if(isAltActive == true)
            {
                
                playerCameraPivot.localRotation = Quaternion.Euler(playerCameraPivot.localRotation.x, cameraPivotBaseRotationY, playerCameraPivot.localRotation.z);
                isAltActive = false;
            }

            transform.Rotate(Vector3.up * mouseX);
        }
        // 카메라만 좌우회전
        else
        {
            isAltActive = true;
            float yRotation = playerCameraPivot.eulerAngles.y - mouseX;
            playerCameraPivot.rotation = Quaternion.Euler(playerCameraPivot.eulerAngles.x, yRotation, playerCameraPivot.eulerAngles.z);
        }    
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, mouseXRotationMinLimit, mouseXRotationMaxLimit);

        playerCameraPivot.rotation = Quaternion.Euler(xRotation, playerCameraPivot.eulerAngles.y, playerCameraPivot.eulerAngles.z);
        //playerCamera.rotation = Quaternion.Euler(xRotation, playerCamera.eulerAngles.y, playerCamera.eulerAngles.z);
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
                interactObject.gameObject.tag = "Catch";

                Debug.Log(interactObject.gameObject.tag);
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

                interactObject.gameObject.tag = "Fish";
                Debug.Log(interactObject.gameObject.tag);
            }
        }
    }
}