using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sound/BGM")]
public class BGMData : ScriptableObject
{
    [Header("Data")]
    [Tooltip("The intro part of the track")]
    public AudioClip introAudioClip;

    [Tooltip("The loop part of the track")]
    public AudioClip loopAudioClip;
}
