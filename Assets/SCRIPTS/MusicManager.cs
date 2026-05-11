using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    // drag all music tracks here
    public AudioClip[] musicTracks;

    // overall music volume
    public float volume = 0.5f;

    // how long the crossfade takes (in seconds)
    public float fadeDuration = 5f;

    // drag the Music mixer group here so volume slider still works
    public AudioMixerGroup musicMixerGroup;

    private AudioSource sourceA;
    private AudioSource sourceB;
    private AudioSource activeSource; // whichever source is currently playing

    private List<int> trackOrder;
    private int currentTrackIndex = 0;
    private bool isFading = false;

    private void Awake()
    {
        // create two AudioSources in code — one fades out while the other fades in
        sourceA = gameObject.AddComponent<AudioSource>();
        sourceB = gameObject.AddComponent<AudioSource>();

        // route both through the Music mixer group so the volume slider controls them
        sourceA.outputAudioMixerGroup = musicMixerGroup;
        sourceB.outputAudioMixerGroup = musicMixerGroup;

        sourceA.playOnAwake = false;
        sourceB.playOnAwake = false;
        sourceA.loop = false;
        sourceB.loop = false;

        activeSource = sourceA;
    }

    private void Start()
    {
        Time.timeScale = 1f; // make sure time is reset when scene loads
        AudioListener.volume = 1f; // make sure volume is restored when scene loads

        float savedVolume = PlayerPrefs.GetFloat("SavedMusicVolume", 0.5f);
        sourceA.volume = savedVolume;
        sourceB.volume = 0f; // inactive source starts silent

        ShuffleTracks();
        PlayCurrentTrack();
    }

    private void Update()
    {
        if (activeSource.isPlaying && !isFading)
        {
            float timeRemaining = activeSource.clip.length - activeSource.time;

            // start crossfading when close to the end of the track
            if (timeRemaining <= fadeDuration)
            {
                StartCoroutine(CrossFadeToNext());
            }
        }
    }

    private IEnumerator CrossFadeToNext()
    {
        isFading = true;

        // pick whichever source isn't currently active
        AudioSource nextSource = (activeSource == sourceA) ? sourceB : sourceA;

        // move to the next track in the shuffled order
        currentTrackIndex++;
        if (currentTrackIndex >= trackOrder.Count)
        {
            ShuffleTracks();
            currentTrackIndex = 0;
        }

        float savedVolume = PlayerPrefs.GetFloat("SavedMusicVolume", 0.5f);

        // load and start the next track immediately at volume 0 — no gap
        nextSource.clip = musicTracks[trackOrder[currentTrackIndex]];
        nextSource.volume = 0f;
        nextSource.Play();

        float elapsed = 0f;
        float startVolume = activeSource.volume;

        // fade old track out and new track in at the same time
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            activeSource.volume = Mathf.Lerp(startVolume, 0f, t);
            nextSource.volume = Mathf.Lerp(0f, savedVolume, t);

            yield return null;
        }

        activeSource.Stop();
        activeSource.volume = 0f;

        // next source is now the active one
        activeSource = nextSource;

        isFading = false;
    }

    public void SetVolume(float value)
    {
        // update whichever source is currently active
        if (activeSource != null)
            activeSource.volume = value;
    }

    private void ShuffleTracks()
    {
        // create a list of tracks and shuffle them randomly
        trackOrder = new List<int>();

        for (int i = 0; i < musicTracks.Length; i++)
        {
            trackOrder.Add(i);
        }

        // goes through the list and randomly swaps each element
        for (int i = trackOrder.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            int temp = trackOrder[i];
            trackOrder[i] = trackOrder[randomIndex];
            trackOrder[randomIndex] = temp;
        }
    }

    private void PlayCurrentTrack()
    {
        if (musicTracks.Length == 0) return;

        float savedVolume = PlayerPrefs.GetFloat("SavedMusicVolume", 0.5f);
        activeSource.clip = musicTracks[trackOrder[currentTrackIndex]];
        activeSource.volume = savedVolume;
        activeSource.Play();
    }
}