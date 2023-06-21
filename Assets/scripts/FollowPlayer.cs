// Smooth towards the target

// for the trail effect, thanks the Gabriel Aguiar Prod. Youtube Channel
// https://www.youtube.com/watch?v=7vvycc2iX6E&t=777s&ab_channel=GabrielAguiarProd.



using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour
{
    public Rigidbody2D target_rb2D;
    private Transform target;
    public Vector3 offset;
    public float smoothTime = 0.3F;
    private Vector3 velocity = Vector3.zero;
    private Camera cam;
    public float status_size;
    public float max_size;
    public AnimationCurve curve_speed_factor;
    public float orthographicSize_when_still = 10f;
    private float scale_size_factor = 0f;
    public float speed_fallof_factor;

    public float delay_size_cam_recover;
    private float val = 0f;

    public float max_distance_to_player;

    [Header("Start level animation")]
    public float speed_to_rech_player = 1f;
    public float delay_before_reach_player = 0.5f;
    private float reach_time = 0f;

    public AnimationCurve speed_animation;

    private bool is_reaching_player_at_start;

    void Start(){
        target = target_rb2D.transform;
        cam = GetComponent<Camera>();
    }
    void Update()
    {

        if(GameManager.Instance.actual_game_state == GameManager.GameState.game){
            UpdateScaleSizeFactor();

            cam.orthographicSize = orthographicSize_when_still*curve_speed_factor.Evaluate(scale_size_factor); // update camera size
            Vector3 targetPosition = target.TransformPoint(offset);

            // Smoothly move the camera towards that target position
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

            clamp_distance_to_the_target();
        }

        else if(GameManager.Instance.actual_game_state == GameManager.GameState.startLevel){
            delay_before_reach_player = delay_before_reach_player == 0f ? 0f : Mathf.Max(0f,delay_before_reach_player-Time.deltaTime);
            if(delay_before_reach_player == 0f){
                move_toward_player();
            }
            if(Vector3.Distance(transform.position, target.position) <= 0.001f) GameManager.Instance.actual_game_state = GameManager.GameState.game;
        }
    }

    private void UpdateScaleSizeFactor(){
        float distance = Vector3.Distance(target.position,transform.position);
        if (scale_size_factor <= distance) // scale_size_factor increases with the distance and is equal to it
        {
            if (val <= delay_size_cam_recover) val = delay_size_cam_recover;
            scale_size_factor = distance;
        }
        else{ // it decreases slower than the distance as a fix speed
            if (val >=0.05) { // check if the delay is over
                val -= Time.deltaTime;
                }
            else scale_size_factor-=speed_fallof_factor*Time.deltaTime; // apply the diminution
        }
    }

    private void move_toward_player(){
        float avancement = speed_animation.Evaluate(reach_time);
        transform.position = Vector3.Lerp(transform.position, target.position,avancement); // step to the player
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, orthographicSize_when_still,avancement); // step to the size view
        reach_time+=Time.deltaTime*speed_to_rech_player;
    }

    private void clamp_distance_to_the_target(){
        Vector3 delta_pos = transform.position-target.position;

        if(Vector3.Magnitude(delta_pos) > max_distance_to_player+0.002){
            transform.position = target.position + delta_pos.normalized*max_distance_to_player;
        }    
    }
}

