using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using UnityEngine;

public class FinishFlag : MonoBehaviour
{
    
    public bool isFinished;

    public static FinishFlag Instance {  get; private set; }

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

    private void Start()
    {
        GameManager.Instance?.RegisterFinishFlag(this);
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
