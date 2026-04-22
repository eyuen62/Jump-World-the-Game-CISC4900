using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    // drag all music tracks here
    public AudioClip[] musicTracks;

    // overall music volume
    public float volume = 0.05f;

    // how long the music crossfades (in seconds) before switching to the next track
    public float fadeDuration = 5f;

    private AudioSource audioSource; // reference to the AudioSource component
    private List<int> trackOrder; // stores the shuffled order of tracks
    private int currentTrackIndex = 0; // tracks which song in the shuffled order is currently playing
    private bool isFading = false; // prevents multiple crossfades from triggering at the same time

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>(); // get the AudioSource component
    }

    private void Start()
    {
        AudioListener.volume = 1f; // make sure volume is restored when scene loads
        ShuffleTracks(); // shuffle the track order when the game starts
        PlayCurrentTrack(); // play the first track in the shuffled order
    }

    private void Update()
    {
        if (audioSource.isPlaying && !isFading) // only check if a track is playing and not already fading
        {
            float timeRemaining = audioSource.clip.length - audioSource.time; // how much time is left in the track

            if (timeRemaining <= fadeDuration) // if within the fade window, start fading the music
            {
                StartCoroutine(FadeAndNext()); // start fading out and then play the next track
            }
        }
    }

    private IEnumerator FadeAndNext()
    {
        isFading = true; // mark that a fade is currently happening

        float startVolume = audioSource.volume; // remember the starting volume
        float elapsed = 0f; // tracks how much time has passed during the fade

        while (elapsed < fadeDuration) // gradually reduce volume over the fade duration
        {
            elapsed += Time.deltaTime; // count up time
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeDuration); // smoothly lower volume to 0
            yield return null; // wait one frame before continuing
        }

        audioSource.Stop(); // stop the current track once fully faded out
        NextTrack(); // move to the next track
        isFading = false; // reset the fading flag
    }

    private void ShuffleTracks()
    {
        // create a list of tracks and shuffle them randomly
        trackOrder = new List<int>();

        for (int i = 0; i < musicTracks.Length; i++)
        {
            trackOrder.Add(i); // add each track index to the list
        }

        // goes through the list and randomly swaps each element
        for (int i = trackOrder.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1); // pick a random index
            int temp = trackOrder[i]; // store the current value
            trackOrder[i] = trackOrder[randomIndex]; // swap
            trackOrder[randomIndex] = temp; // complete the swap
        }
    }

    private void PlayCurrentTrack()
    {
        if (musicTracks.Length == 0) return; // don't play if no tracks are assigned

        audioSource.clip = musicTracks[trackOrder[currentTrackIndex]]; // set the current track
        audioSource.volume = volume; // restore volume to full before playing
        audioSource.Play(); // play the track
    }

    private void NextTrack()
    {
        currentTrackIndex++; // move to the next track in the shuffled order

        if (currentTrackIndex >= trackOrder.Count) // if all tracks have been played
        {
            ShuffleTracks(); // reshuffle for a new random order
            currentTrackIndex = 0; // start from the beginning of the new shuffle
        }

        PlayCurrentTrack(); // play the next track
    }
}