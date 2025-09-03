using UnityEngine;

public class InteractObj : MonoBehaviour
{
    private bool isShowUI = false;
    public GameObject testUI;
    private Transform trn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trn = null;
    }

    // Update is called once per frame
    void Update()
    {
        if(isShowUI)
        {
            testUI.transform.LookAt(Camera.main.transform);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag=="Player"&&!GameManager.instance.PC.canInteracting)
        {
            isShowUI = true;
            testUI.transform.position = new Vector3(transform.position.x,transform.position.y+2f,transform.position.z);
            trn = other.transform;
            testUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            isShowUI = false;
            testUI.SetActive(false);
        }
    }
}
