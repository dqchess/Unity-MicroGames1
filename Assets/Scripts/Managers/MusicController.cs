using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicController : MonoBehaviour {
    // Components
    AudioSource audioSource;
    //// References
    //private AudioClip ac_music;

    // Getters
    private SoundManager soundManager { get { return GameManagers.Instance.SoundManager; } }


    // ----------------------------------------------------------------
    //  Initialize
    // ----------------------------------------------------------------
    private void Awake () {
        audioSource = GetComponent<AudioSource>();
        
        ApplyMusicVolume();
        
        // Add event listeners!
        GameManagers.Instance.SoundManager.SetMusicVolumeEvent += OnSetMusicVolume;
    }
    private void OnDestroy () {
        // Remove event listeners!
        GameManagers.Instance.SoundManager.SetMusicVolumeEvent -= OnSetMusicVolume;
    }
    
    
    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    private void OnSetMusicVolume(float vol) {
        ApplyMusicVolume();
    }
    
    private void ApplyMusicVolume() {
        audioSource.volume = soundManager.Volume_Music;
    }




}




