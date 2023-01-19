using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{

    public void SwitchScenes(string sceneName)
    {
        if (sceneName == null)
        {
            Debug.LogError("Assign Scene Name");
            return;
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}
