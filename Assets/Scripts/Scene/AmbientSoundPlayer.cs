using UnityEngine;

public class AmbientSoundPlayer : MonoBehaviour {
    
    public AudioClip[] ambientClips;
    public float minDelay = 10f;
    public float maxDelay = 60f;
    public AudioSource audioSource;
    
    private float _nextPlayTime;

    
    #region Unity Methods
    
    private void Start() {
        ScheduleNextPlay();
    }

    private void Update() {
        
        if (Time.time >= _nextPlayTime) {
            PlayAmbientSound();
            ScheduleNextPlay();
        }
    }
    
    #endregion
    
    
    #region Private Class Methods

    private void ScheduleNextPlay() {
        _nextPlayTime = Time.time + Random.Range(minDelay, maxDelay);
    }

    private void PlayAmbientSound() {
        
        if (ambientClips.Length > 0) {
            AudioClip clip = ambientClips[Random.Range(0, ambientClips.Length)];
            audioSource.PlayOneShot(clip);
        }
    }
    
    #endregion
}