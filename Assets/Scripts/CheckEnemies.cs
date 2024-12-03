using UnityEngine;

public class CheckEnemies : MonoBehaviour
{
    
    void Update()
    {
        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            SceneController sceneController = FindObjectOfType<SceneController>();
            sceneController.LoadSceneByName("WinScreen");
        }
    }
}
