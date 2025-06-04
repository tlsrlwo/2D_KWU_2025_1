using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform startTransform;
    [SerializeField] private GameObject player;

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
    }

    private void Start()
    {
        if(startTransform == null)
        {
            //startTransform = 2D ���� ����� �����ؼ� �ٽ� �ϱ�
        }

        if(player == null)
        {
            player = Instantiate(player);
            player.name = "Player";
            player.tag = "Player";
            player.transform.position = startTransform.transform.position;
        }
    }


}
