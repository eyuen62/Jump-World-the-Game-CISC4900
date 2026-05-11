using UnityEngine;
using UnityEngine.UI;

public class SoundSettings : MonoBehaviour
{
    [SerializeField] Slider soundSlider;

    private void Start()
    {
        // load saved volume or default to 50% on first play
        float savedVolume = PlayerPrefs.GetFloat("SavedMusicVolume", 0.5f);
        soundSlider.value = savedVolume;
        ApplyVolume(savedVolume);
    }

    public void SetVolumeFromSlider()
    {
        // save and apply the volume whenever the slider moves
        float value = soundSlider.value;
        PlayerPrefs.SetFloat("SavedMusicVolume", value);
        ApplyVolume(value);
    }

    private void ApplyVolume(float value)
    {
        // find MusicManager and tell it to update volume directly
        MusicManager musicManager = GameObject.Find("Music").GetComponent<MusicManager>();
        if (musicManager != null)
            musicManager.SetVolume(value);
    }
}