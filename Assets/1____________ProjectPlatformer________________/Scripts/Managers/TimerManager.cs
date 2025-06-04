using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimerManager : MonoBehaviour
{
   public static TimerManager Instance { get; private set; }

    //[SerializeField] private TextMeshProUGUI timerText;
    
    [SerializeField]private float timer;
    private bool isPlaying;
    private bool hasStarted;

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
    }

    private void Update()
    {
        if(isPlaying && !GamePauseManager.Instance.isPaused)
        {
            timer += Time.deltaTime;
        }
        //if (timerText != null)
        //{
        //    timerText.text = timer.ToString("F2") + "sec";
        //}
    }

    // 최초 타이머 시작
    public void StartTimer()
    {
      if(!hasStarted)
        {
            timer = 0f;
            isPlaying = true;
            hasStarted = true;
        }
    }

    // 클리어 or 죽었을 때
    public void StopTimer()
    {
        isPlaying = false;
        Debug.Log($"Clear! Time: {timer:F2}초");
    }

    public void ResetGame()
    {
        timer = 0f;
        isPlaying = false;
        hasStarted = false;
        // 플레이어 위치 리셋 로직 등 필요
    }


    public float GetTime() => timer;
}
