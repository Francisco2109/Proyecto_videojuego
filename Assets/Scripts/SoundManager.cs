using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Mixer Groups")]
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;

    [Header("Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource loopingSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Clips")]
    [SerializeField] private AudioClip backgroundMusicClip;
    [SerializeField] private AudioClip walkLoopClip;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip victoryClip;
    [SerializeField] private AudioClip loseClip;

    private readonly HashSet<int> walkingSourceIds = new HashSet<int>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicSource == null)
            musicSource = GetOrAddAudioSource("Music Audio Source");

        if (loopingSource == null)
            loopingSource = GetOrAddAudioSource("Looping Audio Source");

        if (sfxSource == null)
            sfxSource = GetOrAddAudioSource("SFX Audio Source");

        if (musicMixerGroup != null)
            musicSource.outputAudioMixerGroup = musicMixerGroup;

        if (sfxMixerGroup != null)
        {
            loopingSource.outputAudioMixerGroup = sfxMixerGroup;
            sfxSource.outputAudioMixerGroup = sfxMixerGroup;
        }

        PlayBackgroundMusic();
    }

    private AudioSource GetOrAddAudioSource(string sourceName)
    {
        Transform child = transform.Find(sourceName);

        if (child == null)
        {
            GameObject sourceObject = new GameObject(sourceName);
            sourceObject.transform.SetParent(transform, false);
            child = sourceObject.transform;
        }

        AudioSource audioSource = child.GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = child.gameObject.AddComponent<AudioSource>();

        return audioSource;
    }

    public void SetWalking(GameObject walker, bool isWalking)
    {
        if (walker == null || walkLoopClip == null || loopingSource == null)
            return;

        int walkerId = walker.GetInstanceID();

        if (isWalking)
            walkingSourceIds.Add(walkerId);
        else
            walkingSourceIds.Remove(walkerId);

        if (walkingSourceIds.Count > 0)
        {
            if (loopingSource.clip != walkLoopClip)
            {
                loopingSource.clip = walkLoopClip;
                loopingSource.loop = true;
            }

            if (!loopingSource.isPlaying)
                loopingSource.Play();
        }
        else if (loopingSource.isPlaying)
        {
            loopingSource.Stop();
        }
    }

    public void PlayJump()
    {
        PlayOneShot(jumpClip);
    }

    public void PlayVictory()
    {
        // Detener loops y música de fondo, pero permitir reproducir el stinger
        StopWalking();
        if (musicSource != null && musicSource.isPlaying)
            musicSource.Stop();

        PlayOneShot(victoryClip);
    }

    public void PlayLose()
    {
        // Detener loops y música de fondo, pero permitir reproducir el stinger
        StopWalking();
        if (musicSource != null && musicSource.isPlaying)
            musicSource.Stop();

        PlayOneShot(loseClip);
    }

    public void StopWalking()
    {
        if (loopingSource != null && loopingSource.isPlaying)
            loopingSource.Stop();
    }

    public void StopAllSounds()
    {
        walkingSourceIds.Clear();
        StopWalking();

        if (musicSource != null && musicSource.isPlaying)
            musicSource.Stop();

        if (sfxSource != null)
            sfxSource.Stop();
    }

    public void PlayBackgroundMusic()
    {
        if (backgroundMusicClip == null || musicSource == null)
            return;

        if (musicSource.clip != backgroundMusicClip)
        {
            musicSource.clip = backgroundMusicClip;
            musicSource.loop = true;
        }

        if (!musicSource.isPlaying)
            musicSource.Play();
    }

    private void PlayOneShot(AudioClip clip)
    {
        if (clip == null || sfxSource == null)
            return;

        sfxSource.PlayOneShot(clip);
    }
}