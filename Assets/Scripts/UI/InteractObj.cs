using UnityEngine;

public class InteractObj : MonoBehaviour
{
    private bool isShowUI = false;
    public GameObject testUI;
    private Transform trn;
    public e_ItemType ItemType;

    
    public enum e_ItemType
    {
        Fish,
        Achievement,
        Goal
    }
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
        if (other.gameObject.GetComponent<chaseFish>() != null)
            return;
        if (ItemType == e_ItemType.Achievement && other.CompareTag("Player"))
        {
            GameManager.instance.Achievement = true;
            UIManager.Instance.GetAchieve();
            Destroy(gameObject);
        }
        else if (ItemType != e_ItemType.Fish)
            return;
        else if (other.tag == "Player" && !GameManager.instance.PC.canInteracting)
        {
            isShowUI = true;
            testUI.transform.position = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
            trn = other.transform;
            testUI.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (ItemType != e_ItemType.Goal)
            return;
        // Physics.OverlapBox로 한 번에 체크
        Collider[] colliders = Physics.OverlapBox(transform.position,GetComponent<BoxCollider>().size / 2);

        bool hasPlayer = false;
        bool hasMini = false;
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Player"))
            {
                if (col.gameObject.GetComponent<chaseFish>() != null)
                    hasMini = true;
                if(col.gameObject.GetComponent<MomCat>() != null)
                    hasPlayer = true;
            }
        }

        if (hasPlayer&&hasMini)
        {
            Debug.Log(" 골!");
            UIManager.Instance.ClearUI();
            Time.timeScale = 0f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<chaseFish>() != null)
        {
            return;
        }

        if (other.tag == "Player" && ItemType == e_ItemType.Fish)
        {
            HideUI();
        }
    }

    public void HideUI()
    {
        isShowUI = false;
        testUI.SetActive(false);

    }
}
