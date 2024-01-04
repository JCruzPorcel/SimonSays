using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private SimonSaysController controller;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] audioClips;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        controller = FindObjectOfType<SimonSaysController>();
    }

    private void PlaySound(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    public void ColorButtonSound(AudioClip audioClip)
    {
        bool canPlay = controller.CanPlay;

        if (canPlay)
        {
            PlaySound(audioClip);
        }
    }

    public void CounterSound(int timer)
    {
        AudioClip audioClip;

        switch (timer)
        {
            case 0:
                audioClip = audioClips[0];
                break;
            case 1:
                audioClip = audioClips[1];
                break;
            case 2:
                audioClip = audioClips[2];
                break;
            default:
                audioClip = audioClips[0];
                break;
        }

        PlaySound(audioClip);
    }
}
