using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI temporitzadorText;
    [SerializeField] private float tempsInicial = 60f;
    private float temps;

    void Start()
    {
        temps = tempsInicial;
    }

    void Update()
    {
        if (temps > 0)
        {
            temps -= Time.deltaTime;

            int minutes = Mathf.FloorToInt(temps / 60);
            int seconds = Mathf.FloorToInt(temps % 60);
            temporitzadorText.text = "Timer: " + string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        else
        {
            temps = 0;
            temporitzadorText.text = "Timer: 00:00";
            SceneManager.LoadScene("Ending");
        }
    }
}
