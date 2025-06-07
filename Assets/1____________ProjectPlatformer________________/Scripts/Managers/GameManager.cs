using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform startTransform;
    [SerializeField] private GameObject player;
    //[SerializeField] private GameObject finishGoal;

    [SerializeField] private FinishFlag finishFlag;
    public GameObject finishUI;

   public static GameManager Instance { get; private set; }

 

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (finishUI != null)
            {
                finishUI = Instantiate(finishUI);
                finishUI.name = "finishUI";
                
                finishUI.SetActive(false);
                DontDestroyOnLoad(finishUI);
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
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        player = GameObject.FindGameObjectWithTag("Player");
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

        if (finishFlag != null && finishFlag.isFinished)
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
