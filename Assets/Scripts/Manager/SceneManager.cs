using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    public static SceneManager instance;
    public SceneType _sceneType { get; private set; }

    public enum SceneType
    {
        Lobby,
        Stage1,
        Stage2
    } 
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
            Destroy(gameObject);
    }

    public void NextScene()
    {
        // 마지막 씬
        int lastScene = System.Enum.GetValues(typeof(SceneType)).Cast<int>().Max();
        _sceneType += 1;
        if ((int)_sceneType > lastScene)
            _sceneType = SceneType.Lobby;
        string sceneName = _sceneType.ToString();
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void ReLoadScene()
    {
        string sceneName = _sceneType.ToString();
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // 새 씬이 로드될 때마다 버튼 자동 연결
        StartCoroutine(ConnectButtonsAfterDelay());
    }

    IEnumerator ConnectButtonsAfterDelay()
    {
        // UI가 완전히 로드될 때까지 잠시 대기
        yield return new WaitForSeconds(0.1f);

        ConnectSceneButtons();
    }

    void ConnectSceneButtons()
    {
        // "NextButton" 이름의 버튼 찾기
        Button[] children = UIManager.Instance.gameObject.GetComponentsInChildren<Button>(true);
        foreach(var item in children)
        {
            if (item.name == "StartBtn")
            {
                item.onClick.RemoveAllListeners();
                item.onClick.AddListener(NextScene);
                Debug.Log($"'{item.name}' 버튼에 NextScene 연결됨");
                return;
            }
        }
        Debug.Log("버튼을 찾을 수 없습니다 : StartBtn");

    }
}
