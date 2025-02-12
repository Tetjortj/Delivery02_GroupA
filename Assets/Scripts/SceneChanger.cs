using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public string sceneName;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            LoadScene();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitGame();
        }
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }


    private void ExitGame()
    {
        Application.Quit();
    }
}
