using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    public IntVariableSO score;
    public TextMeshProUGUI scoreText;

    void OnEnable() { if (score && score.OnChanged) score.OnChanged.OnEvent += UpdateUI; }
    void OnDisable() { if (score && score.OnChanged) score.OnChanged.OnEvent -= UpdateUI; }

    void Start() { UpdateUI(score ? score.value : 0); }
    void UpdateUI(int v) { if (scoreText) scoreText.text = "Score: " + v; }
}
