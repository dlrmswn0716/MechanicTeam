using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    //TO-DO :임시 PUBLIC 생성
    public TextMeshProUGUI time;
    public GameObject clear;
    public GameObject over;
    public string playerName;
    public Image interactionIcon;
    public GameObject AchieveObj;

    public GameObject InteractUI;
    public GameObject OverModel;

    public  GameObject testObj;
    public bool isInter = false;
    public GameObject Fade;

    private bool timeOff = false;
    private float _time = 0.0f;
    private bool isAchieve = false;

    // singleton 처리
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(SceneManager.instance._sceneType != SceneManager.SceneType.Lobby)
        {
            MouseLock(true);
        }
    }

    private void MouseLock(bool isLock)
    {
        if (isLock)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (time == null)
            return;
        if(!timeOff)
            timeCalc();

    }

    //시간 계산
    private void timeCalc()
    {
        _time += Time.deltaTime;
        int min = Mathf.FloorToInt(_time / 60f);
        int sec = Mathf.FloorToInt(_time % 60f);
        time.text = string.Format("{0:00}:{1:00}", min, sec);
    }

    //TO-DO : 털뭉치 획득 시 아이콘 활성화
    public void GetAchieve()
    {
        AchieveObj.GetComponent<Image>().color = Color.red;
        isAchieve= true;
    }

    // 클리어 UI SHOW
    public void ClearUI()
    {
        MouseLock(false);
        timeOff = true;
        clear.SetActive(true);
        //TO-DO : 추후에 스크립터블 오브젝트를 사용해서 조건 확인
        int checkCnt = 1;
        if (_time <= 1.0f)
            checkCnt++;
        //TO - DO : 털뭉치 획득 여부 확인
        if (isAchieve)
            checkCnt++;
        Image[] images = clear.transform.GetChild(1).GetComponentsInChildren<Image>();
        for(int i = 0; i < checkCnt; i++)
        {
            images[i].color = Color.yellow;
        }

    }

    // 게임오버 UI SHOW
    public void OverUI()
    {
        if(OverModel!= null)
        {
            StartCoroutine(OverCor());
        }
        else
        {
            MouseLock(false);
            timeOff = true;
            over.SetActive(true);
        }

    }
    IEnumerator OverCor()
    {
        yield return new WaitForSeconds(0.3f);
        if (Fade != null)
            Fade.SetActive(true);
        MouseLock(false);
        timeOff = true;
        over.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }
}
