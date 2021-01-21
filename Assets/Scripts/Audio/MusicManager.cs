using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

//THIS SCRIPT IS TO BE PLACED DIRECTLY ON THE MUSIC AUDIO SOURCE GAMEOBJECT
public class MusicManager : MonoBehaviour
{
    [SerializeField] private GameValues gameValues;
    [SerializeField] private AudioClip[] songs;
    private AudioSource audioSource;

    private Queue<AudioClip> songQueue;
    private const float defaultVolume = 0.5f;
    private bool stopped = false;

    // Start is called before the first frame update
    void Start() {
        songQueue = new Queue<AudioClip>();
        audioSource = GetComponent<AudioSource>();

        //GAME VALUES MUSIC OVERRIDE
        if (gameValues != null) {
            songs = gameValues.Songs;
        }

        UpdateVolume();
        ShuffleAndAdd();
    }

    void ShuffleAndAdd() {
        AudioClip[] songs = (AudioClip[]) this.songs.Clone();
        
        //Fisher Yates algorithm
        int n = songs.Length;
        while (n > 1) 
        {
            int k = Random.Range(0, n--);

            //swap
            AudioClip temp = songs[n];
            songs[n] = songs[k];
            songs[k] = temp;
        }

        //add to queue
        foreach (AudioClip song in songs) {
            songQueue.Enqueue(song);
        }
    }

    void Update() {
        if (!stopped && !audioSource.isPlaying) {
            PlayMusic();
        }
    }

    public void PlayMusic() {
        if (songQueue.Count > 0) {
            AudioClip clip = songQueue.Dequeue();
            audioSource.clip = clip;
            StartCoroutine(FadeIn(1));
            audioSource.Play();
        }
        else {
            ShuffleAndAdd();
        }
    }

    public IEnumerator FadeOutAndStop(float speed) {
        stopped = true;
        float t = 0;
        float originalVolume = audioSource.volume;
        while (t <= 1) {
            t += speed * Time.deltaTime;
            audioSource.volume = Mathf.Lerp(originalVolume, 0, t);
            yield return null; 
        }
        audioSource.Stop();
        audioSource.clip = null;
        audioSource.volume = originalVolume;
    }
    
    public IEnumerator FadeIn(float speed) {
        float t = 0;
        float originalVolume = audioSource.volume;
        while (t <= 1) {
            t += speed * Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0, originalVolume, t);
            yield return null; 
        }
    }

    public void Pause() {
        stopped = true;
        audioSource.Pause();
    }

    public void Resume() {
        stopped = false;
        if (audioSource.clip != null) {
            audioSource.UnPause();
        }
        else {
            PlayMusic();
        }   
    }

    public void UpdateVolume() {
        audioSource.volume = PlayerPrefs.GetFloat(TagHolder.PREF_MUSIC_VOLUME, defaultVolume);
    }
}
