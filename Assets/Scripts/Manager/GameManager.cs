using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public MomCat PC;
    public bool Achievement;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

    }
    
    public void Init(GameObject PlayerObj)
    {
        PC = PlayerObj.GetComponent<MomCat>();
        Achievement = false;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
