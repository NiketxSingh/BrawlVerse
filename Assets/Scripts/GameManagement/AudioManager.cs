using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public static AudioManager instance;

    private bool musicPlaying;

    [Header("-------Audio Source-------")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("-------Audio Clip-------")]
    public AudioClip background;
    public AudioClip damage;
    public AudioClip attack;
    public AudioClip click;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else {
            Destroy(gameObject); // Prevent duplicate
        }
    }

    private void Start() {
        if (!musicPlaying && background != null) {
            musicSource.clip = background;
            musicSource.Play();
            musicPlaying = true;
        }
    }

    public void PlaySFX(AudioClip clip) {
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }
}
