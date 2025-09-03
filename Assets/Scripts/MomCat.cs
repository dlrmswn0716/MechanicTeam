using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;


public class MomCat : MonoBehaviour
{
    public float moveSpeed = 50f;
    private Rigidbody rb;

    public float mouseMovementSpeedX = 250f;
    public float mouseMovementSpeedY = 250f;
    public Transform playerCamera;
    private Transform playerCameraPivot;
    private Transform catBody;
    private Transform catMain;
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

    public GameObject MiniCat;
    public LayerMask groundLayer;         // 바닥 레이어 지정

    private BoxCollider boxCollider;

    [Header("Ground Check Settings")]
    [Tooltip("캐릭터의 피봇 위치에서 얼마나 높은 곳에서 땅을 향해 Raycast를 쏠지 결정합니다.")]
    [SerializeField] private float groundCheckYOffset = 0.5f;
    [Tooltip("땅을 감지할 Raycast의 최대 거리입니다.")]
    [SerializeField] private float groundCheckDistance = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //TO-DO : 임시
        GameManager.instance.Init(gameObject);

        rb = GetComponent<Rigidbody>();
        playerCameraPivot = transform.Find("CameraPivot");
        catBody = transform.Find("Cat/Body");
        catMain = transform.Find("Cat");

        xRotation = 0f;
        playerCamera.localRotation = Quaternion.Euler(0f, 180f, 0f);
        playerCamera.localPosition = new Vector3(0, 0, 5);

        boxCollider = GetComponent<BoxCollider>();

        //TO-DO : UI 클릭을 위한 임시 주석
        /*        Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;*/

    }

    // Update is called once per frame
    void Update()
    {
        HandleMove();
        HandleJump();
        HandleInteract();
        HandleMouseLook();
        HandleMouseClick();
        StickToGround();
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
        xRotation += mouseY;
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

    }

    private void OnCollisionExit(Collision collision)
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Achievement"))
            GameManager.instance.Achievement = true;

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Fish") && isInteracting == false)
        {
            interactObject = null;
            canInteracting = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Fish") && isInteracting == false)
        {
            if (interactObject != null)
                return;

            interactObject = other.gameObject;
            canInteracting = true;
        }
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

        if (interactObject == null)
        {
            canInteracting = false;
            return;
        }

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

    void StickToGround()
    {
        // 캐릭터의 위치에서 아래 방향으로 Ray를 쏠 시작점과 방향 정의
        // 인스펙터에서 설정한 groundCheckYOffset 값을 사용하도록 수정
        Vector3 rayOrigin = transform.position + Vector3.up * groundCheckYOffset;
        Vector3 rayDirection = Vector3.down;

        // 인스펙터에서 설정한 groundCheckDistance 값을 사용하도록 수정
        float rayDistance = groundCheckDistance;

        // [디버그 1] 레이캐스트 광선을 Scene 뷰에 빨간색 선으로 표시
        Debug.DrawRay(rayOrigin, rayDirection * rayDistance, Color.red);

        // Raycast가 바닥에 부딪혔는지 확인
        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, rayDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            // [디버그 2] Raycast가 무언가에 부딪혔을 때 콘솔에 로그 출력
            // Debug.Log("레이캐스트 충돌! 충돌한 오브젝트: " + hit.transform.name);

            // 만약 Ray가 자기 자신에게 부딪혔다면 함수를 그냥 종료한다.
            if (hit.transform == this.transform)
            {
                return;
            }

            // [디버그 3] 부딪힌 지점에서 표면의 법선 벡터(Normal)를 파란색 선으로 표시
            Debug.DrawRay(hit.point, hit.normal, Color.blue);

            // [디버그 4] 법선 벡터 값을 콘솔에 출력
            // Debug.Log("경사각도(Normal): " + hit.normal);


            // --- 기존 회전 로직 ---
            Quaternion slopeRotation = Quaternion.FromToRotation(transform.up, hit.normal);
            Quaternion targetRotation = slopeRotation * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
        else
        {
            // [디버그 5] 레이캐스트가 아무것에도 부딪히지 않았을 때 로그 출력
            // Debug.Log("레이캐스트가 아무것에도 충돌하지 않음.");
        }
    }
}