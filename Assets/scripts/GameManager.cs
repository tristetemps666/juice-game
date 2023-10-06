using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Unity.Mathematics;
using UnityEngine.UIElements;


public class SlowMotionHandler{
    public float slow_mo_time_scale = 0.05f;

    public float normal_time_scale = 5f;
    public float actual_time_scale = 1f;

    public bool can_use_slow_motion  = false;
    public bool is_in_slow_motion = false;

    public float speed_slow_mo = 10f;

    public float stamina_slow_motion;
    public float max_duration_slow_motion = 5f;

    public float speed_to_recover_stamina = 0.5f;


    public void update(){        
        if(is_in_slow_motion){
            actual_time_scale = Mathf.Max(actual_time_scale-Time.deltaTime*speed_slow_mo,slow_mo_time_scale);
            stamina_slow_motion = math.max(stamina_slow_motion-Time.deltaTime/actual_time_scale,0f); // is not affected by time scale
        }
        else{
            actual_time_scale = Mathf.Min(actual_time_scale+Time.deltaTime*speed_slow_mo,1f);
            stamina_slow_motion=math.min(stamina_slow_motion+Time.deltaTime*speed_to_recover_stamina,max_duration_slow_motion);
        }

        if (stamina_slow_motion == 0){
            is_in_slow_motion = false;
            can_use_slow_motion = false;
        }

        if(can_use_slow_motion == false){
            stamina_slow_motion=math.min(stamina_slow_motion+Time.deltaTime*speed_to_recover_stamina,max_duration_slow_motion);
            if(stamina_slow_motion == max_duration_slow_motion) can_use_slow_motion = true;
        }
    }

}



public class GameManager : MonoBehaviour
{
    public  static GameManager Instance;

    public GameState actual_game_state;

    public Volume volume;
    private LensDistortion lensDistortion;
    private ChromaticAberration chromaticAberration;

    public Canvas canvas;

    public enum GameState {
        pause,
        start,
        startLevel,
        game,
        win,
        loose, 
        overview
    }

    public SlowMotionHandler slow_motion = new SlowMotionHandler();

    void Awake(){
        Instance = this;
    }

    void UpdateGameState(){
        switch(actual_game_state){
            case GameState.pause:
                break;

            case GameState.game:
                break;

            case GameState.start:
                break;

            case GameState.startLevel:
                break;

            case GameState.win:
                break;

            case GameState.loose:
                break;
            
            case GameState.overview:
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        actual_game_state = GameState.startLevel;


        slow_motion.stamina_slow_motion = slow_motion.max_duration_slow_motion;
        Time.timeScale = slow_motion.slow_mo_time_scale;

        LensDistortion LDtmp;
        if(volume.profile.TryGet<LensDistortion>( out LDtmp ) )
        {
            lensDistortion = LDtmp;
        }
        ChromaticAberration CHtmp;
        if(volume.profile.TryGet<ChromaticAberration>( out CHtmp ) )
        {
            chromaticAberration = CHtmp;
        }
    }

 
    // Update is called once per frame
    void Update()
    {
        Time.timeScale = slow_motion.actual_time_scale;

        // Debug.Log(" // peut utiliser slowmo ? : " + slow_motion.can_use_slow_motion + " // est en slow mo ? : " + slow_motion.is_in_slow_motion + " // time scale : " + slow_motion.actual_time_scale + " // stamina : " +  slow_motion.stamina_slow_motion);
        if(Input.GetKeyDown(KeyCode.LeftAlt)){
            slow_motion.is_in_slow_motion = (!slow_motion.is_in_slow_motion) && slow_motion.can_use_slow_motion;
        }
        
        slow_motion.update();

        if(lensDistortion != null && chromaticAberration != null){
            float intensity = math.remap(slow_motion.slow_mo_time_scale,1f,0.5f,0f,slow_motion.actual_time_scale);

            lensDistortion.intensity.value = 0.5f*intensity;
            chromaticAberration.intensity.value = intensity;
        }

        SliderSlowMo sliderSlowMo = canvas.GetComponentInChildren<SliderSlowMo>();
        sliderSlowMo.can_use_slow_motion = slow_motion.can_use_slow_motion;
        sliderSlowMo.stamina = slow_motion.stamina_slow_motion;
        sliderSlowMo.max_stamina = slow_motion.max_duration_slow_motion;
    }


}
