using System.Collections;
using UnityEngine;

public class SoundManager {
    // Delegates and Events
    public delegate void FloatAction (float _float);
    public event FloatAction SetMusicVolumeEvent;
    public event FloatAction SetSfxVolumeEvent;
    // Properties
    private float volume_music;
	private float volume_sfx;
	public bool IsMusic { get; private set; }
	public bool IsSfx { get; private set; }


    // ----------------------------------------------------------------
    //  Getters
    // ----------------------------------------------------------------
	public float Volume_Music { get { return IsMusic ? volume_music : 0; } }
    public float Volume_Sfx { get { return IsSfx ? volume_sfx : 0; } }


	// ----------------------------------------------------------------
	//  Setters
	// ----------------------------------------------------------------
    public void SetVolume_Music (float _volume) {
        volume_music = _volume;
        SaveStorage.SetFloat (SaveKeys.VOLUME_MUSIC, volume_music); // Save that volume, Bill!
        SetMusicVolumeEvent?.Invoke(Volume_Music);
    }
    public void SetVolume_Sfx (float _volume) {
        volume_sfx = _volume;
        SaveStorage.SetFloat (SaveKeys.VOLUME_SFX, volume_sfx); // Save that volume, Tiffany!
        SetSfxVolumeEvent?.Invoke(Volume_Sfx);
    }
    
    public void ToggleIsSound() {
        bool isSound = !IsSfx;
        SetIsMusic(isSound);
        SetIsSfx(isSound);
    }
 //   public void ToggleIsMusic() {
 //       SetIsMusic(!IsMusic);
 //   }
	//public void ToggleIsSfx() {
	//	SetIsSfx(!IsSfx);
	//}
    private void SetIsMusic(bool _bool) {
        IsMusic = _bool;
        SaveStorage.SetInt(SaveKeys.IsMusic, IsMusic?1:0);
        SetMusicVolumeEvent?.Invoke(Volume_Music);
	}
    private void SetIsSfx(bool _bool) {
        IsSfx = _bool;
        SaveStorage.SetInt(SaveKeys.IsSfx, IsSfx?1:0);
        SetSfxVolumeEvent?.Invoke(Volume_Sfx);
    }



    // ----------------------------------------------------------------
    //  Initialize
    // ----------------------------------------------------------------
    public SoundManager () {
        // Load up volumes!
        volume_music = SaveStorage.GetFloat (SaveKeys.VOLUME_MUSIC, 1);
		volume_sfx = SaveStorage.GetFloat (SaveKeys.VOLUME_SFX, 1);
		IsMusic = SaveStorage.GetInt(SaveKeys.IsMusic, 1) == 1;
		IsSfx = SaveStorage.GetInt(SaveKeys.IsSfx, 1) == 1;
    }


}


