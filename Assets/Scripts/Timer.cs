using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] TextMeshProUGUI temporitzadorText;
    float temps;
    // Update is called once per frame
    void Update()
    {
        temps += Time.deltaTime;
        int minutes = Mathf.FloorToInt(temps/60);
        int seconds = Mathf.FloorToInt(temps%60);
        temporitzadorText.text = "Timer: " + string.Format("{0:00}:{1:00}", minutes,seconds);
    }
}
