using UnityEngine;

public class FXManager : MonoBehaviour {
    
    [Header("Audio Source")]
    [SerializeField] private AudioSource soundFXObject;

    
    #region Audio Management
    
    public void PlaySound(AudioClip audioClip, Vector3 position, float volume) {
        AudioSource audioSource = Instantiate(soundFXObject, position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.spatialBlend = 1f;
        audioSource.priority = 200;
        audioSource.Play();

        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }
    
    public void PlayRandomSound(AudioClip[] audioClips, Vector3 position, float volume) {
        int random = Random.Range(0, audioClips.Length);

        AudioSource audioSource = Instantiate(soundFXObject, position, Quaternion.identity);
        audioSource.clip = audioClips[random];
        audioSource.volume = volume;
        audioSource.Play();

        float duration = audioSource.clip.length;
        Destroy(audioSource.gameObject, duration);
    }
    
    #endregion
    
    
    #region Effect Management
    
    public void PlayEffect(GameObject effect, Vector3 position, Quaternion rotation, Transform parent = default) {
        ParticleSystem[] particleSystems = effect.GetComponentsInChildren<ParticleSystem>();
        float duration = CalculateMaxDuration(particleSystems);
        
        GameObject effectObject = Instantiate(effect, position, rotation, parent);
        Destroy(effectObject, duration);
    }
    
    public void PlayRandomEffect(GameObject[] effects, Vector3 position, Quaternion rotation) {
        int random = Random.Range(0, effects.Length);
        GameObject effect = effects[random];
        ParticleSystem[] particleSystems = effect.GetComponentsInChildren<ParticleSystem>();
        float duration = CalculateMaxDuration(particleSystems);
        
        GameObject effectObject = Instantiate(effect, position, rotation);
        Destroy(effectObject, duration);
    }
    
    private float CalculateMaxDuration(ParticleSystem[] particleSystems) {
        float maxDuration = 0f;

        foreach (var ps in particleSystems) {
            var mainModule = ps.main;
            float duration = mainModule.duration + mainModule.startLifetime.constantMax;

            if (duration > maxDuration) {
                maxDuration = duration;
            }
        }
        return maxDuration;
    }
    
    #endregion
}
