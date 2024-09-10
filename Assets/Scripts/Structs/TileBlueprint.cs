using UnityEngine;

[System.Serializable]
public struct TileBlueprint {
    public Transform spawnPoint;
    public GameObject selectEffect;
    public bool isPlayerHouse;
    public GameObject replaceEffect;
    public AudioClip buildAudioClip;
    public AudioClip destroyAudioClip;
    public AudioClip farmAudioClip;
    public AudioClip repairAudioClip;
    public AudioClip upgradeAudioClip;
    public TileObjectType type;
    public TileObject startObject;
    public Quaternion objectRotation;
    public TileObject[] randomWood;
    public TileObject[] randomWaste;
}