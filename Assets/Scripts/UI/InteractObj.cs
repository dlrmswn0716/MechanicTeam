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
        if(other.tag=="Player")
        {
            isShowUI = true;
            Vector3 calcPos = Vector3.Lerp(gameObject.transform.position, other.transform.position, 0.5f);
            testUI.transform.position = calcPos;
            trn = other.transform;
            testUI.SetActive(true);
        }
    }
}
