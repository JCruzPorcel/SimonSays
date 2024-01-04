using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimonSaysController : MonoBehaviour
{
    [SerializeField] private Color[] colors = { Color.red, Color.green, Color.blue, Color.yellow };
    [SerializeField] private Button[] colorButtons;
    [SerializeField] private Button[] difficultyButtons;

    private List<int> computerSequence = new List<int>();
    private List<int> playerSequence = new List<int>();

    private Animator[] lightGo;
    [SerializeField] private TMPro.TextMeshProUGUI messageText;
    private bool canPlay = false;
    public bool CanPlay { get { return canPlay; } }
    private bool isCountdownRunning = false;

    private enum Difficulty { Easy, Normal, Hard }
    private Difficulty currentDifficulty = Difficulty.Normal;

    // Velocidades de secuencia para cada dificultad (ajusta según sea necesario)
    private float easySequenceSpeed = 1.5f;
    private float normalSequenceSpeed = 1f;
    private float hardSequenceSpeed = 0.5f;

    private AudioManager audioManager;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();

        lightGo = new Animator[colors.Length];

        for (int i = 0; i < lightGo.Length; i++)
        {
            lightGo[i] = colorButtons[i].GetComponentInChildren<Animator>();
        }

        messageText.text = "Select difficulty to start";

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

            // Cambiar colores de los botones de dificultad
            UpdateDifficultyButtonColors();

            // Limpiar el mensaje cuando se selecciona una dificultad
            messageText.text = string.Empty;

            // Iniciar el juego cuando se selecciona una dificultad válida
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
        yield return new WaitForSeconds(1);

        messageText.text = "2";
        audioManager.CounterSound(counter);
        yield return new WaitForSeconds(1);

        messageText.text = "1";
        counter++;
        audioManager.CounterSound(counter);
        yield return new WaitForSeconds(1);

        messageText.text = "Go!";
        counter++;
        audioManager.CounterSound(counter);
        yield return new WaitForSeconds(1);

        messageText.text = string.Empty;

        isCountdownRunning = false;

        // Mostrar la secuencia al inicio
        yield return StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        canPlay = false;

        yield return new WaitForSeconds(1);

        messageText.text = string.Empty;

        float sequenceSpeed = GetSequenceSpeed(currentDifficulty);

        for (int i = 0; i < computerSequence.Count; i++)
        {
            ShowColor(computerSequence[i]);
            yield return new WaitForSeconds(sequenceSpeed);
        }

        canPlay = true;
        messageText.text = "Your Turn";
    }

    private void ShowColor(int colorIndex)
    {
        lightGo[colorIndex].SetTrigger(currentDifficulty.ToString());
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
                    canPlay = false;

                    // Reiniciar el juego después de 1.5 segundos
                    StartCoroutine(ResetGameAfterDelay(1.5f));
                    return;
                }
            }

            if (playerSequence.Count == computerSequence.Count)
            {
                // El jugador ha completado la secuencia
                messageText.text = "You won! Next level";
                playerSequence.Clear();
                AddRandomColorToSequence();
                StartCoroutine(PlaySequence());
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
                return easySequenceSpeed;
            case Difficulty.Normal:
                return normalSequenceSpeed;
            case Difficulty.Hard:
                return hardSequenceSpeed;
            default:
                return normalSequenceSpeed;
        }
    }
}
