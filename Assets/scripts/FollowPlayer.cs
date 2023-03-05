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
    private float orthographicSize_when_still;
    private float scale_size_factor = 0f;
    public float speed_fallof_factor;

    public float delay_size_cam_recover;
    public bool slowtime;
    private float val = 0f;


    void Start(){
        target = target_rb2D.transform;
        cam = GetComponent<Camera>();
        orthographicSize_when_still = cam.orthographicSize;
    }
    void Update()
    {

        if (slowtime) Time.timeScale=0.3f;
        else Time.timeScale = 1f;
        
        UpdateScaleSizeFactor();

        cam.orthographicSize = orthographicSize_when_still*curve_speed_factor.Evaluate(scale_size_factor);
        Vector3 targetPosition = target.TransformPoint(offset);

        // Smoothly move the camera towards that target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    void UpdateScaleSizeFactor(){
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
}
