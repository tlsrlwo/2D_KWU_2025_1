using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform startTransform;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject finishGoal;

   public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
    }

    private void InvokeMethod()
    {
        Debug.Log("플레이어가 죽음");
    }

}
