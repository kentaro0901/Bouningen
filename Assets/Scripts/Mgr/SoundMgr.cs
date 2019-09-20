using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMgr : MonoBehaviour {

    AudioSource audioSource;

    [SerializeField] AudioClip limitBreak;

    public enum Clip {
        LimitBreak
    }

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySFX(Clip c) {
        audioSource.Stop();
        switch (c) {
            case Clip.LimitBreak: audioSource.clip = limitBreak; break;
            default: break;
        }
        audioSource.Play();
    }
}
