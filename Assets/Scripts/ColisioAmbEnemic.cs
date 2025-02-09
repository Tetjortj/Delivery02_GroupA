using UnityEngine;
using UnityEngine.SceneManagement;
public class ColisioAmbEnemic : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Enemy")
        {
            gameOver();
        }
    }
    public void gameOver()
    {
        SceneManager.LoadSceneAsync("Ending");
    }
}
