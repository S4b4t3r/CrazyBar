using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Jukebox");

        if (objs.Length > 1)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
    public AudioSource mainMusic;
    
    // Start is called before the first frame update
    void Start()
    {
        mainMusic.Play();
    }

    public void SetMusicPitch(float pitch)
    {
        mainMusic.pitch = pitch;
    }
}
