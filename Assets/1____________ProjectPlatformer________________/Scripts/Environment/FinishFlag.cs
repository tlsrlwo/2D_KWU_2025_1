using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishFlag : MonoBehaviour
{
    
    public bool isFinished;

   // public static FinishFlag Instance {  get; private set; }

    private void Awake()
    {
   /*     if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }*/
    }

    private void Start()
    {
        GameManager.Instance?.RegisterFinishFlag(this);
        GamePauseManager.Instance?.RegisterFinishFlag(this);
    }

    public void UnlockNewLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex >= PlayerPrefs.GetInt("ReachedIndex"))
        {
            PlayerPrefs.SetInt("ReachedIndex", SceneManager.GetActiveScene().buildIndex + 1);           // 현재 씬의 빌드 인덱스 보다 +1 만큼 ReachedIndex 를 설정
            Debug.Log("현재 도달한 레벨 : " + PlayerPrefs.GetInt("ReachedIndex"));
            PlayerPrefs.SetInt("UnlockedLevel", PlayerPrefs.GetInt("UnlockedLevel", 1) + 1);
            Debug.Log("현재 도달한 레벨 : " + PlayerPrefs.GetInt("UnlockedLevel"));
            PlayerPrefs.Save();
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            GamePauseManager.Instance.FinishLevel();
            TimerManager.Instance.StopTimer();
            isFinished = true;
        }
    }

}
