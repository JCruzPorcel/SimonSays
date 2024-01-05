using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource sfxButtonAudioSource;
    [SerializeField] private AudioClip[] difficultyClips;
    [SerializeField] private AudioClip[] counterClips;
    [SerializeField] private AudioClip[] buttonClips;
    [SerializeField] private AudioClip[] resultClips;

    private bool isSfxMuted = false;
    private bool isMusicMuted = false;

    private void PlayAudioClip(AudioClip audioClip, AudioSource audioSource)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    public void PlayButtonSound(int soundIndex)
    {
        AudioClip audioClip = buttonClips[soundIndex];
        PlayAudioClip(audioClip, sfxButtonAudioSource);
    }

    public void PlayDifficultySound(int soundIndex)
    {
        AudioClip audioClip = (soundIndex >= 0 && soundIndex < difficultyClips.Length) ? difficultyClips[soundIndex] : difficultyClips[0];
        PlayAudioClip(audioClip, sfxAudioSource);
    }

    public void PlayCounterSound(int soundIndex)
    {
        AudioClip audioClip = (soundIndex >= 0 && soundIndex < counterClips.Length) ? counterClips[soundIndex] : counterClips[0];
        PlayAudioClip(audioClip, sfxAudioSource);
    }

    public void PlayResultSound(bool isVictory)
    {
        AudioClip audioClip = isVictory ? resultClips[0] : resultClips[1];
        PlayAudioClip(audioClip, sfxAudioSource);
    }

    public void ToggleSfxMute()
    {
        isSfxMuted = !isSfxMuted;
        sfxAudioSource.mute = isSfxMuted;
        sfxButtonAudioSource.mute = isSfxMuted;
    }

    public void ToggleMusicMute()
    {
        isMusicMuted = !isMusicMuted;
        musicAudioSource.mute = isMusicMuted;
    }
}
 