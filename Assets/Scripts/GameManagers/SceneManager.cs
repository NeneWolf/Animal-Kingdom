using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneManager : MonoBehaviour
{
    string SceneName;

    void LoadSelectedScene()
    {
       // SceneManager.
    }

    public void ChangeScene(string name) { SceneName = name; LoadSelectedScene(); }  
}
