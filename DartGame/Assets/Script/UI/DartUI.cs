using UnityEngine;
using TMPro;

public class DartUI : MonoBehaviour
{
    [SerializeField]
    private GameObject dartImage;
    [SerializeField]
    private GameObject scoreObject;
    [SerializeField]
    private TextMeshProUGUI scoreText;

    public void SetScore(int score)
    {
        dartImage.SetActive(false);
        scoreObject.SetActive(true);

        if (score == 0)
            scoreText.text = "X";
        else
            scoreText.text = $"{score}";
    }

    public void Clear()
    {
        dartImage.SetActive(true);
        scoreObject.SetActive(false);
        scoreText.text = "";
    }
}
