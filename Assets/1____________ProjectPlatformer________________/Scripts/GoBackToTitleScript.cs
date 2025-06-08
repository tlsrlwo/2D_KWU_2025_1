using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoBackToTitleScript : MonoBehaviour
{
   public void OpenScene()
    {
        SceneManager.LoadScene("Title_Scene");
    }

    public void TestSceneFunc()
    {
        SceneManager.LoadScene("LogicTestingScene");
    }

    public void ResetLevelsValue()
    {
        PlayerPrefs.SetInt("UnlockedLevel", 1);
        PlayerPrefs.SetInt("ReachedIndex", 1);
        PlayerPrefs.Save();

        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);

    }

    public void ExitGame()
    {
        Debug.Log("������ �����մϴ�");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;        // ������ Play ��� ����
#else
    Application.Quit();                                         // ����� ���ӿ����� ���� ����
#endif
    }
}
