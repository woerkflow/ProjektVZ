using UnityEngine;

public class AmbientSoundPlayer : MonoBehaviour {
    
    [SerializeField] private AudioClip[] ambientClips;
    [SerializeField] private float minDelay = 10f;
    [SerializeField] private float maxDelay = 60f;
    [SerializeField] private AudioSource audioSource;
    
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