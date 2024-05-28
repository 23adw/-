using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.Video;

public class TestSet : MonoBehaviour
{
    public Slider[] volumeSlider;
    public AudioSource[] volumeSource;
    //public AudioListener TotalSource;
    void Start()
    {
        volumeSlider[0].onValueChanged.AddListener(BGM);
        volumeSlider[1].onValueChanged.AddListener(Interface);
        volumeSlider[2].onValueChanged.AddListener(Total);
    }

    void BGM(float value)
    {
        // ������Ƶ������
        volumeSource[0].volume=value;//��Ƶͨ������
    }
    void Interface(float value)
    {
        volumeSource[1].volume = value;
    }
    void Total(float value)
    {
        AudioListener.volume = value;
    }
    void Update()
    {
        
    }
}
