using System.Collections.Generic;
using UnityEngine;

public enum SfxType
{
 Take,
 Throw,
 LevelCompleted,
 Click
}
public enum MusicType
{
    GameplayMusic
}
public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private List<SfxData> _sfxDatas = new List<SfxData>();
    [SerializeField] private List<MusicData> _musicDatas = new List<MusicData>();
    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this) 
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        if (!_musicSource.isPlaying)
        {
            PlayMusic(MusicType.GameplayMusic);
        }
    }
    public void PlaySfx(SfxType sfx)
    {
        var sfxData = GetSfxData(sfx);
        _sfxSource.PlayOneShot(sfxData.Clip, sfxData.Volume);
    }

    public void PlayMusic(MusicType music)
    {
        var musicData = GetMusicData(music);
        _musicSource.clip = musicData.Clip;
        _musicSource.volume = musicData.Volume;
        _musicSource.Play();
    }
    private SfxData GetSfxData(SfxType type)
    {
        var result = _sfxDatas.Find(sfxData => sfxData.Type == type);
        return result;
    }

    private MusicData GetMusicData(MusicType type)
    {
        var result = _musicDatas.Find(musicData => musicData.Type == type);
        return result;
    }
}

[System.Serializable]
public class SfxData
{
    [SerializeField] private SfxType _type;
    [SerializeField] private AudioClip _clip;
    [SerializeField][Range(0, 1)] private float _volume;
    public SfxType Type => _type;
    public AudioClip Clip => _clip;
    public float Volume => _volume;
}
[System.Serializable]
public class MusicData
{
    [SerializeField] private MusicType _type;
    [SerializeField] private AudioClip _clip;
    [SerializeField][Range(0, 1)] private float _volume;

    public MusicType Type => _type;
    public AudioClip Clip => _clip;
    public float Volume => _volume;
}