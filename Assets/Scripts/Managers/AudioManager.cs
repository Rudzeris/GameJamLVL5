using System.Collections;
using UnityEngine;

[AddComponentMenu("Managers/Audio")]
public class AudioManager : MonoBehaviour, IGameManager
{
    [SerializeField] private AudioSource soundSource;
    [SerializeField] private AudioSource music1Source;
    [SerializeField] private AudioSource music2Source;

    private AudioSource _activeMusic;
    private AudioSource _inactiveMusic;

    [SerializeField][Range(0, 1)] private float startMusicVolume = 0.5f;
    [SerializeField][Range(0, 1)] private float startSoundVolume = 1f;

    public float crossFadeRate = 1.5f;
    private bool _crossFading;

    public ManagerStatus Status { get; private set; }

    private float _musicVolume;

    public float MusicVolume
    {
        get { return _musicVolume; }
        set
        {
            _musicVolume = value;
            if (music1Source != null && music2Source != null)
            {
                music1Source.volume = value;
                music2Source.volume = value;
            }
        }
    }
    public bool MusicMute
    {
        get { return music1Source.mute; }
        set
        {
            if (music1Source != null && music2Source != null)
            {
                music1Source.mute = value;
                music2Source.mute = value;
            }
        }
    }

    public float SoundVolume
    {
        get { return AudioListener.volume; }
        set { AudioListener.volume = value; }
    }

    public bool SoundMute
    {
        get { return AudioListener.pause; }
        set { AudioListener.pause = value; }
    }

    private void PlayMusic(AudioClip clip)
    {
        if (_crossFading) return;
        StartCoroutine(CrossFadeMusic(clip));
    }
    public void StopMusic()
    {
        music1Source.Stop();
    }

    public void Startup()
    {
        Debug.Log("Audio Manager starting...");

        music1Source.ignoreListenerPause = true; // Игнорировать паузу слушателя, чтобы музыка продолжала играть при паузе игры
        music1Source.ignoreListenerVolume = true; // Игнорировать уровень громкости слушателя, чтобы музыка могла иметь свой собственный уровень громкости
        music2Source.ignoreListenerPause = true;
        music2Source.ignoreListenerVolume = true;

        SoundVolume = startSoundVolume; // Начальный уровень громкости
        MusicVolume = startMusicVolume; // Начальный уровень громкости музыки

        _activeMusic = music1Source;
        _inactiveMusic = music2Source;

        Status = ManagerStatus.Started;
    }

    public void PlaySound(AudioClip clip)
    {
        soundSource.PlayOneShot(clip);
    }

    private IEnumerator CrossFadeMusic(AudioClip clip)
    {
        _crossFading = true;

        _inactiveMusic.clip = clip;
        _inactiveMusic.volume = 0;
        _inactiveMusic.Play();

        float scaleRate = crossFadeRate * _musicVolume;

        while (_activeMusic.volume > 0)
        {
            _activeMusic.volume -= scaleRate * Time.deltaTime;
            _inactiveMusic.volume += scaleRate * Time.deltaTime;
            yield return null;
        }

        AudioSource temp = _activeMusic;

        _activeMusic = _inactiveMusic;
        _activeMusic.volume = _musicVolume;

        _inactiveMusic = temp;
        _inactiveMusic.Stop();

        _crossFading = false;
    }
}
