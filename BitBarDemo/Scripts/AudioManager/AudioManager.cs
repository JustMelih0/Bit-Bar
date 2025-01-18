using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

 public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
   public Sound[] musicSounds,sfxSounds;
   public AudioSource musicSource,sfxSource;
   private void Awake() 
   {
     if (Instance==null)
     {
        Instance=this;
        UpdateSounds();
        DontDestroyOnLoad(gameObject);

     }
     else
     {
        Destroy(gameObject);
     }
   }
   public void UpdateSounds()
   {
      if (PlayerPrefs.GetInt("gameLevel") == 0)
      {
         musicSource.volume = 0.3f;
         sfxSource.volume = 0.5f;
         return;
      }
      musicSource.volume = PlayerPrefs.GetFloat("musicVolume");
      sfxSource.volume = PlayerPrefs.GetFloat("sfxVolume");
   }
   public void PlayMusic(string name)
   {
       Sound s=Array.Find(musicSounds,x=>x.name==name);
       if (s==null)
       {
            Debug.Log("ses yok");
       }
       else
       {
          musicSource.clip=s.clip;
          musicSource.Play();
       }
   }
   public void PlaySFX(string name)
   {
       Sound s=Array.Find(sfxSounds,x=>x.name==name);
       if (s==null)
       {
            Debug.Log("ses yok");
       }
       else
       {
         sfxSource.PlayOneShot(s.clip);
       }
   }
   
  
}
[System.Serializable]
public class Sound 
{
    
    public string name;
    public AudioClip clip;
}

