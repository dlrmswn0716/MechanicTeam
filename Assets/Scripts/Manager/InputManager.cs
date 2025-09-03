using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 이동 입력
    public Vector2 GetMoveInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        return new Vector2(moveX, moveZ);
    }

    // 마우스 입력
    public Vector2 GetMouseInput()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        return new Vector2(mouseX, mouseY);
    }

    // 점프 입력
    public bool GetJumpInput()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    // 상호작용 입력
    public bool GetInteractInput()
    {
        return Input.GetKeyDown(KeyCode.E);
    }

    // 마우스 클릭 (왼쪽 버튼)
    public bool GetMouseClick()
    {
        return Input.GetMouseButtonDown(0);
    }

    public bool GetAltInput()
    {
        return Input.GetKey(KeyCode.LeftAlt);
    }
}
