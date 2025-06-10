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

        int unlockedLevels = PlayerPrefs.GetInt("UnlockedLevel", 1);            // ù��° ������ ���ӽ������ ��������ϱ� ���� ReachedLevels �� 1�� �����ص�

        for (int i = 0; i < buttons.Length; i++)                                // ��� ������ ó���� ��ݻ��·� ����
        {
            buttons[i].interactable = false;
        }
        for (int i = 0; i < unlockedLevels; i++)                                // ���� ������ int�� unlockedLevels ��ŭ ����� ��������
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