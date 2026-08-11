using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class NewEmptyCSharpScript1 : MonoBehaviour 
{
    public Slider VolumeSilider;

    private void Start()
    {
        if (PlayerPrefs.HasKey("soundVolume"))
            LoadVolume();
        else
        {
            PlayerPrefs.SetFloat("soundVolume", 1);
            LoadVolume();
        }
    }

    public void SetVolume()
    {
        AudioListener.volume = VolumeSilider.value;
        SaveVolume();
    }

    public void SaveVolume()
    {
        PlayerPrefs.SetFloat("Sound volume", VolumeSilider.value);
    }

    public void LoadVolume()
    {
        VolumeSilider.value = PlayerPrefs.GetFloat("sound volume");
    }
}
