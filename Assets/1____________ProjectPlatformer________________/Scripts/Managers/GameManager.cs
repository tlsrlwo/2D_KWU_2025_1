using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //[SerializeField] private Transform startTransform;
    [SerializeField] private GameObject player;
    //[SerializeField] private GameObject finishGoal;

    [SerializeField] private FinishFlag finishFlag;
    [SerializeField] private GameObject finishUiPrefab;
    public GameObject finishUI;
    private bool isAvailable;

   public static GameManager Instance { get; private set; }
     

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (finishUiPrefab != null)
            {
                finishUI = Instantiate(finishUiPrefab);
                finishUI.name = "finishUI";
                //finishUI.tag = "finishUI";
                finishUI.SetActive(false);
                DontDestroyOnLoad(finishUI);
            }
            else
            {
                Debug.LogError("[GamePauseManager] finish UI Prefab이 할당되지 않았습니다!");
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

        if(finishUI != null)
            Destroy(finishUI);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        player = GameObject.FindGameObjectWithTag("Player");

        isAvailable = scene.name != ("Title_Scene");

        if(finishUI != null)
        {
            finishUI.SetActive(false);
        }
      
        //finishGoal = GameObject.FindGameObjectWithTag("FinishGoal");
        
    }

    private void Update()
    {        
        if (player != null)
        {
            if (player.transform.position.y <= -20f)
            {
                GamePauseManager.Instance.PauseGame();
            }
        }

        if (isAvailable && finishFlag != null && finishFlag.isFinished)
        {
            finishUI.SetActive(true);
        }
    }

    public void RegisterFinishFlag(FinishFlag flag)
    {
        finishFlag = flag;
    }

    private void InvokeMethod()
    {
        Debug.Log("플레이어가 죽음");
    }

}
