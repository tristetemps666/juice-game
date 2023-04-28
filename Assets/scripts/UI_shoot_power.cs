using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_shoot_power : MonoBehaviour
{
    // Start is called before the first frame update
    private Transform scale_vertical_transform;
    private SpriteRenderer level_1_renderer;
    private SpriteRenderer level_2_renderer; 

    private float power_factor = 0f;
    void Start()
    {
        scale_vertical_transform = transform.Find("bottom");
        level_1_renderer = transform.Find("level_1").gameObject.GetComponent<SpriteRenderer>();
        level_2_renderer = transform.Find("level_2").gameObject.GetComponent<SpriteRenderer>();
    }


    void Update()
    {
        update_ui(power_factor);

        Color alpha_increas_level_1 = new Color(1f,1f,1f,Mathf.Clamp(power_factor,0f,1f));
        Color alpha_increas_level_2 = new Color(1f,1f,1f,Mathf.Clamp(power_factor-1f,0f,1f));

        Color color_level_changing_1_passed = power_factor >= 1f ? Color.yellow : Color.white;
        Color color_level_changing_2_passed = power_factor >= 2f ? Color.yellow : Color.white;

        level_1_renderer.color = alpha_increas_level_1*color_level_changing_1_passed;
        level_2_renderer.color = alpha_increas_level_2*color_level_changing_2_passed;

    }
    public void ui_is_not_actve(){
        gameObject.SetActive(false);
    }

    public void set_power_factor(float factor){
        power_factor = factor;
    }



    public void update_ui(float power_factor){
        scale_vertical_transform.localScale= new Vector3(1f,power_factor,0f);
    }
}
