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
}
