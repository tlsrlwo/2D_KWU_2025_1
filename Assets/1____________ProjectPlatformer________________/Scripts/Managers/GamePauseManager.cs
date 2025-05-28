using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePauseManager : MonoBehaviour
{
    [Header("UI컴포넌트")]
    
    [SerializeField] private GameObject pauseUIPrefab;
    private GameObject pauseUI;
    public bool isPaused;
    private bool isAvailable;               // 이거는 메인 화면에서 매니저를 안 사용하기 위한 방법

    public static GamePauseManager Instance { get; private set; }           // 싱글톤으로 플레이어 스크립트에서 참조할 것.

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);    // UI 생성 및 유지

            if (pauseUIPrefab != null)
            {
                pauseUI = Instantiate(pauseUIPrefab);
                pauseUI.name = "pauseUI";
                pauseUI.tag = "PauseUI";
                pauseUI.SetActive(false);
                DontDestroyOnLoad(pauseUI);
            }
            else
            {
                Debug.LogError("[GamePauseManager] Pause UI Prefab이 할당되지 않았습니다!");
            }
        }
        else
        {
            Destroy(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (pauseUI != null)
            Destroy(pauseUI);

    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isPaused = false;
        isAvailable = scene.name != "Title_Scene";

        if (pauseUI != null)
            pauseUI.SetActive(false);

        Time.timeScale = 1f; // 씬 진입 시 타임스케일 초기화
    }

    private void Update()
    {
        if (!isAvailable || pauseUI == null) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    public void PauseGame()
    {
        if (!isAvailable || pauseUI == null) return;

        isPaused = true;
        Time.timeScale = 0f;

        pauseUI.SetActive(true);
    }
    public void ResumeGame()
    {
        if (!isAvailable || pauseUI == null ) return;

        isPaused = false;
        Time.timeScale = 1f;

        pauseUI.SetActive(false);
    }
    public void MainMenuBTN()
    {
        Time.timeScale = 1f;
        if(pauseUI != null)
        {
            Destroy(pauseUI);
            pauseUI = null;
        }
        SceneManager.LoadScene("Title_Scene");
    }
    public void ExitGameBTN()
    {
        Debug.Log("게임을 종료합니다");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;        // 에디터 Play 모드 중지
#else
    Application.Quit();                                         // 빌드된 게임에서는 정상 종료
#endif
    }
}
