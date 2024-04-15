using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestScoreText;

    private int currentScore = 0;
    private int bestScore = 0;

    void Start()
    {
        // Inicializar el puntaje y el mejor puntaje desde PlayerPrefs
        currentScore = 0;
        bestScore = PlayerPrefs.GetInt("BestScore", 0);

        // Actualizar los textos iniciales
        UpdateScoreText();
        UpdateBestScoreText();
    }

    // Función para sumar puntaje desde otros scripts
    public void AddScore(int amount)
    {
        currentScore += amount;

        // Actualizar el texto del puntaje actual
        UpdateScoreText();
    }

    // Función para actualizar el texto del puntaje
    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {currentScore}";
        }
    }

    // Función para actualizar el texto del mejor puntaje
    private void UpdateBestScoreText()
    {
        if (bestScoreText != null)
        {
            if (bestScore > 0)
            {
                bestScoreText.text = $"Best Score: {bestScore}";
            }
            else
            {
                bestScoreText.text = $"Best Score: -";
            }

            // Guardar el mejor puntaje en PlayerPrefs
            PlayerPrefs.SetInt("BestScore", bestScore);
        }
    }

    public void CheckBestScoreAndSave()
    {
        // Actualizar el mejor puntaje si es necesario
        if (currentScore > bestScore)
        {
            bestScore = currentScore;
            UpdateBestScoreText();
        }
    }
    public void ResetScores()
    {
        currentScore = 0;
        UpdateScoreText();
    }

    private void DeleteScore()
    {
        PlayerPrefs.DeleteKey("CurrentScore");
        PlayerPrefs.DeleteKey("BestScore");
    }
}
