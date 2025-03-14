using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SimonSaysController : MonoBehaviour
{
    [SerializeField] private Color[] colors = { Color.red, Color.green, Color.blue, Color.yellow };
    [SerializeField] private Button[] colorButtons;
    [SerializeField] private Button[] difficultyButtons;

    private List<int> computerSequence = new List<int>();
    private List<int> playerSequence = new List<int>();

    private Animator[] lightAnim;
    [SerializeField] private TextMeshProUGUI messageText;
    private bool canPlay = false;
    public bool CanPlay { get { return canPlay; } }
    private bool isCountdownRunning = false;

    private Color lastColor;
    private int colorCount;

    [SerializeField] private TextMeshProUGUI countTextPrefab;
    [SerializeField] private Transform canvasTransform;

    private enum Difficulty { Easy, Normal, Hard }
    private Difficulty currentDifficulty = Difficulty.Normal;

    // Velocidades de secuencia para cada dificultad (ajusta seg�n sea necesario)
    private float easySequenceSpeed = 1f;
    private float normalSequenceSpeed = .75f;
    private float hardSequenceSpeed = 0.5f;

    private float maxSequenceSpeed = 0.2f; // L�mite m�ximo para la velocidad de la secuencia
    private float speedIncreaseFactor = 0.05f; // Factor de aumento de velocidad por acierto
    private float currentSequenceSpeed;

    private int scoreValue = 0;

    private AudioManager audioManager;
    private ScoreManager scoreManager;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        scoreManager = FindObjectOfType<ScoreManager>();

        lightAnim = new Animator[colors.Length];

        lastColor = Color.clear;
        colorCount = 0;

        for (int i = 0; i < lightAnim.Length; i++)
        {
            lightAnim[i] = colorButtons[i].GetComponentInChildren<Animator>();
        }

        messageText.text = "Select difficulty to start";

        currentSequenceSpeed = GetSequenceSpeed(currentDifficulty);

        canPlay = false;
    }

    private void StartGame()
    {
        GenerateSequence();

        if (!isCountdownRunning)
        {
            StartCoroutine(CountdownAndPlaySequence());
        }
        else
        {
            StartCoroutine(PlaySequence());
        }
    }

    private void GenerateSequence()
    {
        computerSequence.Clear();
        AddRandomColorToSequence();
    }

    private void AddRandomColorToSequence()
    {
        int randomColorIndex = Random.Range(0, colors.Length);
        computerSequence.Add(randomColorIndex);
    }

    public void ChangeDifficultyButton(int mode)
    {
        if (System.Enum.IsDefined(typeof(Difficulty), mode))
        {
            StopAllCoroutines();

            isCountdownRunning = false;

            currentDifficulty = (Difficulty)mode;

            audioManager.PlayDifficultySound(0);

            currentSequenceSpeed = GetSequenceSpeed(currentDifficulty);

            // Cambiar colores de los botones de dificultad
            UpdateDifficultyButtonColors();

            // Limpiar el mensaje cuando se selecciona una dificultad
            messageText.text = string.Empty;

            // Iniciar el juego cuando se selecciona una dificultad v�lida
            ResetGame();
        }
        else
        {
            Debug.LogWarning("Invalid difficulty mode: " + mode);
        }
    }

    private void UpdateDifficultyButtonColors()
    {
        ColorBlock easyColors = difficultyButtons[0].colors;
        ColorBlock normalColors = difficultyButtons[1].colors;
        ColorBlock hardColors = difficultyButtons[2].colors;

        easyColors.normalColor = (currentDifficulty == Difficulty.Easy) ? new Color(1f, 1f, 0.62f) : new Color(1f, 1f, 0.62f, 0.1f);
        normalColors.normalColor = (currentDifficulty == Difficulty.Normal) ? new Color(1f, 0.79f, 0.62f) : new Color(1f, 0.79f, 0.62f, 0.1f);
        hardColors.normalColor = (currentDifficulty == Difficulty.Hard) ? new Color(1f, 0.62f, 0.65f) : new Color(1f, 0.62f, 0.65f, 0.1f);

        difficultyButtons[0].colors = easyColors;
        difficultyButtons[1].colors = normalColors;
        difficultyButtons[2].colors = hardColors;
    }

    private IEnumerator CountdownAndPlaySequence()
    {
        canPlay = false;
        isCountdownRunning = true;
        int counter = 0;

        yield return new WaitForSeconds(.2f);

        messageText.text = "3";
        yield return new WaitForSeconds(1f);

        messageText.text = "2";
        audioManager.PlayCounterSound(counter);
        yield return new WaitForSeconds(1f);

        messageText.text = "1";
        counter++;
        audioManager.PlayCounterSound(counter);
        yield return new WaitForSeconds(1f);

        messageText.text = "Go!";
        counter++;
        audioManager.PlayCounterSound(counter);
        yield return new WaitForSeconds(1f);

        messageText.text = string.Empty;

        isCountdownRunning = false;

        // Mostrar la secuencia al inicio
        yield return StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        canPlay = false;
        lastColor = Color.clear;

        yield return new WaitForSeconds(1);

        messageText.text = string.Empty;

        for (int i = 0; i < computerSequence.Count; i++)
        {
            ShowColor(computerSequence[i]);
            yield return new WaitForSeconds(currentSequenceSpeed);
        }

        canPlay = true;
        lastColor = Color.clear;
        messageText.text = "Your Turn";
    }

    private void ShowColor(int colorIndex)
    {
        audioManager.PlayButtonSound(colorIndex);

        // Incrementar la cuenta si el color es igual al anterior
        if (colors[colorIndex] == lastColor)
        {
            colorCount++;
        }
        else
        {
            colorCount = 1;  // Reiniciar la cuenta si el color ha cambiado
        }

        // Guardar el color actual
        lastColor = colors[colorIndex];

        if (colorCount > 1)
        {
            // Obtener la posici�n del bot�n
            Vector3 buttonPosition = colorButtons[colorIndex].transform.position;

            // Instanciar el prefab de TextMeshProUGUI
            TextMeshProUGUI countText = Instantiate(countTextPrefab, canvasTransform);
            countText.text = $"x{colorCount}";

            // Establecer la posici�n del texto en relaci�n con el bot�n
            countText.transform.position = buttonPosition;

            // Destruir el objeto de texto despu�s de un tiempo
            //Destroy(countText.gameObject, 1f);
        }

        // Reiniciar las animaciones de los botones
        lightAnim[colorIndex].Rebind();
        lightAnim[colorIndex].SetTrigger(currentDifficulty.ToString());
    }

    public void OnColorButtonClick(int buttonIndex)
    {
        if (canPlay)
        {
            playerSequence.Add(buttonIndex);
            ShowColor(buttonIndex);
            CheckPlayerSequence();
        }
    }

    private void CheckPlayerSequence()
    {
        if (playerSequence.Count > 0)
        {
            for (int i = 0; i < playerSequence.Count; i++)
            {

                if (i >= computerSequence.Count || playerSequence[i] != computerSequence[i])
                {
                    // El jugador ha perdido
                    messageText.text = "You lost! Try again";
                    audioManager.PlayResultSound(false);
                    canPlay = false;

                    scoreManager.CheckBestScoreAndSave();

                    // Reiniciar el juego despu�s de 1.5 segundos
                    StartCoroutine(ResetGameAfterDelay(1.5f));
                    return;
                }
            }

            if (playerSequence.Count == computerSequence.Count)
            {
                // El jugador ha completado la secuencia
                messageText.text = "You won! Next level";
                audioManager.PlayResultSound(true);

                scoreManager.AddScore(scoreValue);

                playerSequence.Clear();
                AddRandomColorToSequence();
                StartCoroutine(PlaySequence());

                // Incrementar la velocidad de la secuencia si el l�mite no se ha alcanzado
                if (currentSequenceSpeed > maxSequenceSpeed)
                {
                    IncreaseSequenceSpeed();
                }
            }
        }
    }


    private IEnumerator ResetGameAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        ResetGame();
    }

    private void ResetGame()
    {
        // Reiniciar el juego
        StopAllCoroutines();
        scoreManager.CheckBestScoreAndSave();
        scoreManager.ResetScores();
        isCountdownRunning = false;
        computerSequence.Clear();
        playerSequence.Clear();
        StartGame();
    }

    private float GetSequenceSpeed(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                scoreValue = 5;
                return easySequenceSpeed;
            case Difficulty.Normal:
                scoreValue = 10;
                return normalSequenceSpeed;
            case Difficulty.Hard:
                scoreValue = 15;
                return hardSequenceSpeed;
            default:
                scoreValue = 10;
                return normalSequenceSpeed;
        }
    }

    private void IncreaseSequenceSpeed()
    {
        // Aumentar la velocidad de la secuencia seg�n el factor de aumento
        currentSequenceSpeed -= speedIncreaseFactor;
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
