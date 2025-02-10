using UnityEngine;
using TMPro;

public class timer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] TextMeshProUGUI temporitzadorText;
    float temps;
    // Update is called once per frame
    void Update()
    {
        transform.position = Camera.main.transform.position + Camera.main.transform.forward * 5;
        transform.rotation = Camera.main.transform.rotation;
        temps += Time.deltaTime;
        int minutes = Mathf.FloorToInt(temps/60);
        int seconds = Mathf.FloorToInt(temps%60);
        temporitzadorText.text = string.Format("{0:00}:{1:00}", minutes,seconds);
    }
}
