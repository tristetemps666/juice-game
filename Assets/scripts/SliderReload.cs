using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SliderReload : MonoBehaviour
{
    private Slider slider;

    private float reload_progress = 0f;
    private float reload_time = 1f;

    public Color color_value_0;
    public Color color_value_1;

    public bool transition_color = false;


    void Start()
    {
        slider = GetComponentInChildren<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (reload_progress <=0.01f) slider.gameObject.SetActive(false);

        else{
            if(!slider.gameObject.activeSelf) slider.gameObject.SetActive(true);
            reload_progress-= Time.deltaTime;
            slider.value = reload_progress/reload_time;
            
            if (transition_color){
                ColorBlock cb = slider.colors;
                cb.disabledColor = Color.Lerp(color_value_0,color_value_1,slider.value);
                slider.colors = cb;
            }
            
        }
        
    }

    public void start_reload_UI_anim(float delay){
        reload_progress = delay;
        reload_time = delay;
    }
}
