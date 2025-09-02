using System.Linq;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public static SceneManager instance;
    [SerializeField] private SceneType _sceneType;    
    
   enum SceneType
    {
        Lobby,
        Stage1,
        Stage2,
        Stage3
    } 
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
