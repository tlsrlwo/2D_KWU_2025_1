using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelMenu : MonoBehaviour
{
    public Button[] buttons;
    public GameObject levelButtons;

    private void Awake()
    {
        ButtonsArray();

        int unlockedLevels = PlayerPrefs.GetInt("UnlockedLevel", 1);            // 첫번째 레벨은 게임실행부터 잠금해제하기 위해 ReachedLevels 를 1로 설정해둠

        for (int i = 0; i < buttons.Length; i++)                                // 모든 레벨을 처음에 잠금상태로 설정
        {
            buttons[i].interactable = false;
        }
        for (int i = 0; i < unlockedLevels; i++)                                // 위에 설정한 int값 unlockedLevels 만큼 잠금을 해제해줌
        {
            buttons[i].interactable = true;
        }
    }

    public void OpenLevel(int levelNum)
    {
        string levelName = "Level" + levelNum;
        SceneManager.LoadScene(levelName);
    }

    public void ButtonsArray()
    {
        int childCount = levelButtons.transform.childCount;
        buttons = new Button[childCount];
        for (int i = 0; i < childCount; i++)
        {
            buttons[i] = levelButtons.transform.GetChild(i).gameObject.GetComponent<Button>();
        }
    }
}