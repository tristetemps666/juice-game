using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SliderSlowMo : MonoBehaviour
{
    public Slider slider;

    public bool can_use_slow_motion = true;

    public float stamina = 1f;
    public float max_stamina = 1f;

    public Color enabled_color;
    public Color disable_color;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
        slider.value = stamina/max_stamina;
        ColorBlock cb = slider.colors;
        cb.disabledColor = can_use_slow_motion ? enabled_color : disable_color;
        slider.colors = cb;
        
    }

}