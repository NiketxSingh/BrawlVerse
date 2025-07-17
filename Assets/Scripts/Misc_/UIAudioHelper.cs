using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAudioHelper : MonoBehaviour {

    public void PlayClickSound() {
        AudioManager.instance.PlaySFX(AudioManager.instance.click);
    }
}

