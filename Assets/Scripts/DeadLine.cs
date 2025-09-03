using UnityEngine;

public class DeadLine : MonoBehaviour
{

    //떨어질 시 게임오버
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            UIManager.Instance.OverUI();
            Time.timeScale = 0f;
        }
    }
}
