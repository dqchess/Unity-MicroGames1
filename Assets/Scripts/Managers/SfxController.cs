using System.Collections;
using UnityEngine;

/** How to use this class:
 * Add this script to any GameObject.
 * Call "PlaySound" with a reference to me. That's it! :)
 */
public class SfxController : MonoBehaviour {
    // Constants
    private const int NUM_AUDIO_SOURCES = 16;
    // Components
    AudioSource[] allAudioSources; // a list of 16 possible AudioSources. When we want to play a sound, we pick the first one in the list that ISN'T currently playing a sound.
    // References
    [SerializeField] private AudioClip ac_buttonClick=null;

    // Getters
    private SoundManager soundManager { get { return GameManagers.Instance.SoundManager; } }
    private AudioSource GetUnoccupiedAudioSource () {
        for (int i=0; i<allAudioSources.Length; i++) {
            if (!allAudioSources[i].isPlaying) {
                return allAudioSources[i];
            }
        }
        // If we couldn't find any audioSource available, return null. No sound will be played.
        return null;
    }


    // ----------------------------------------------------------------
    //  Initialize
    // ----------------------------------------------------------------
    private void Awake () {
        // Make my AudioSources!
        allAudioSources = new AudioSource[NUM_AUDIO_SOURCES];
        for (int i=0; i<NUM_AUDIO_SOURCES; i++) {
            allAudioSources[i] = this.gameObject.AddComponent<AudioSource>();
        }

        // Add event listeners!
        GameManagers.Instance.EventManager.AnyButtonClickEvent += OnAnyButtonClick;
        //GameManagers.Instance.SoundManager.SetSfxVolumeEvent += OnSetVolumeEvent;
    }
    private void OnDestroy () {
        // Remove event listeners!
        GameManagers.Instance.EventManager.AnyButtonClickEvent -= OnAnyButtonClick;
        //GameManagers.Instance.SoundManager.SetSfxVolumeEvent -= OnSetVolumeEvent;
    }
    
    
    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    private void OnAnyButtonClick() {
        PlaySound(ac_buttonClick);
    }




    // ----------------------------------------------------------------
    //  Play
    // ----------------------------------------------------------------
    //public AudioSource PlaySound (AudioClip clip, float _volume=1f, bool pitchShiftToMatchCurrentSong=false) {
    //    float _pitch = pitchShiftToMatchCurrentSong ? GameManagers.Instance.DataManager.currentSongPitchScaleFromE : 1;
    //    return PlaySound (clip, _volume, 0, _pitch);
    //}
    //public AudioSource PlaySound (AudioClip clip, Vector2 audioPos, float _volume=1f, bool pitchShiftToMatchCurrentSong=false, float _clipStartTime=0) {
    //    float _pan = GetPanFromGlobalPos (audioPos);
    //    float _pitch = pitchShiftToMatchCurrentSong ? GameManagers.Instance.DataManager.currentSongPitchScaleFromE : 1;
    //    return PlaySound (clip, _volume, _pan, _pitch, _clipStartTime);
    //}
    public AudioSource PlaySound (AudioClip clip, float _volume=1f, float _pan=0f, float _pitch=1f, float _clipStartTime=0, bool _bypassAllEffects=false) {
        AudioSource audioSource = GetUnoccupiedAudioSource ();
        if (audioSource!=null) {
            audioSource.loop = false;
            audioSource.clip = clip;
            audioSource.panStereo = _pan;
            audioSource.pitch = _pitch;
            audioSource.time = _clipStartTime;
            audioSource.volume = _volume * soundManager.Volume_Sfx;
            audioSource.bypassEffects = audioSource.bypassListenerEffects = audioSource.bypassReverbZones = _bypassAllEffects;
            audioSource.Play ();
        }
        else {
            Debug.LogWarning ("All " + allAudioSources.Length + " AudioSources occupied; cannot play sfx: " + clip.name);
        }
        return audioSource;
    }


    // ----------------------------------------------------------------
    //  Pause
    // ----------------------------------------------------------------
    //  private void PauseAllSounds() {
    //      AudioSource[] allAudioSources = this.gameObject.GetComponents<AudioSource> ();
    //      for (int i=0; i<allAudioSources.Length; i++) {
    //          allAudioSources[i].Pause();
    //      }
    //  }


    // ----------------------------------------------------------------
    //  Stop
    // ----------------------------------------------------------------
    /** Pass me in the AudioSource you know is playing the clip, and if it's still playing that clip, I'll stop it! */
    public void StopSound (AudioSource _audioSource, AudioClip _audioClip) {
        if (_audioSource.clip == _audioClip) {
            _audioSource.Stop ();
        }
    }


}




