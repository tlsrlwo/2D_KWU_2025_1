using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private int             maxHp = 1;
    private int             currentHp;

    private float           minYdieRange = -10f;
    private Transform       respawnPoint;
    private bool            isDead = false;

    

    private void Start()
    {
        currentHp = maxHp;
    }
    private void Update()
    {
        if(transform.position.y < minYdieRange)
        {
            //Dead();
        }
    }

    void Dead()
    {
        isDead = true;
        currentHp = 0;

        //UiManager.Instance.ShowRetryUI();
    }

}
