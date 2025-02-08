using UnityEngine;
using UnityEngine.SceneManagement;

public class IniciJoc : MonoBehaviour
{
    // canvi d'escena per el boto
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("SampleScene");
    }
   
}
