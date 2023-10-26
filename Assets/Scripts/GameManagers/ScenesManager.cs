using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    string SceneName;
    int sceneID;

    void LoadSelectedSceneByName()
    {
       SceneManager.LoadScene(SceneName);
    }

    void LoadSelectedSceneByID()
    {
        SceneManager.LoadScene(sceneID);
    }

    public void ChangeSceneByName(string name) { SceneName = name; LoadSelectedSceneByName(); }
    public void ChangeSceneByID(int ID) { sceneID = ID; LoadSelectedSceneByID(); }
}
