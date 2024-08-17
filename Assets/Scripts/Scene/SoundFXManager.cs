using UnityEngine;

public class SoundFXManager : MonoBehaviour {
    
    [SerializeField]
    private AudioSource soundFXObject;

    public void PlaySoundFXClip(AudioClip audioClip, Vector3 position, float volume) {

        AudioSource audioSource = Instantiate(soundFXObject, position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);
    }
    
    public void PlayRandomSoundFXClip(AudioClip[] audioClips, Vector3 position, float volume) {
        
        int random = Random.Range(0, audioClips.Length);

        AudioSource audioSource = Instantiate(soundFXObject, position, Quaternion.identity);
        audioSource.clip = audioClips[random];
        audioSource.volume = volume;
        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);
    }
}
