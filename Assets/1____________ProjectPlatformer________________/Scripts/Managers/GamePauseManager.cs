using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePauseManager : MonoBehaviour
{
    [Header("UI������Ʈ")]
    
    [SerializeField] private GameObject pauseUIPrefab;
    private GameObject pauseUI;
    public bool isPaused;
    private bool isAvailable;               // �̰Ŵ� ���� ȭ�鿡�� �Ŵ����� �� ����ϱ� ���� ���

    public static GamePauseManager Instance { get; private set; }           // �̱������� �÷��̾� ��ũ��Ʈ���� ������ ��.

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);    // UI ���� �� ����

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
                Debug.LogError("[GamePauseManager] Pause UI Prefab�� �Ҵ���� �ʾҽ��ϴ�!");
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

        Time.timeScale = 1f; // �� ���� �� Ÿ�ӽ����� �ʱ�ȭ
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
        Debug.Log("������ �����մϴ�");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;        // ������ Play ��� ����
#else
    Application.Quit();                                         // ����� ���ӿ����� ���� ����
#endif
    }
}
