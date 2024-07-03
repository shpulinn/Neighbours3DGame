using System.Collections;
using System.Collections.Generic;
using _Neighbours.Scripts;
using UnityEngine;

public class BootstrapEntryPoint : MonoBehaviour
{
    void Awake() {
        InitializeSystems();
    }

    void Start() {
        StartGame();
    }

    private void InitializeSystems() {
        if (SceneManager.Instance == null) {
            GameObject sceneManager = new GameObject("SceneManager");
            sceneManager.AddComponent<SceneManager>();
        }
        Debug.Log("Systems Initialized");
    }

    private void StartGame() {
        SceneManager.Instance.LoadScene(Scenes.MAIN_MENU);
        Debug.Log("Menu Started");
    }
}
