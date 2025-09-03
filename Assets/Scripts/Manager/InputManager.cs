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

    // �̵� �Է�
    public Vector2 GetMoveInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        return new Vector2(moveX, moveZ);
    }

    // ���콺 �Է�
    public Vector2 GetMouseInput()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        return new Vector2(mouseX, mouseY);
    }

    // ���� �Է�
    public bool GetJumpInput()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    // ��ȣ�ۿ� �Է�
    public bool GetInteractInput()
    {
        return Input.GetKeyDown(KeyCode.E);
    }

    // ���콺 Ŭ�� (���� ��ư)
    public bool GetMouseClick()
    {
        return Input.GetMouseButtonDown(0);
    }
}
