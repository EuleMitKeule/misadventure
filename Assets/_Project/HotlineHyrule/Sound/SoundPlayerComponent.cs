using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayerComponent : MonoBehaviour
{
    public IEnumerator DestroyWhenFinished(AudioSource source)
    {
        yield return new WaitUntil(() => source.isPlaying == false);
        Destroy(source.gameObject);
    }
}
