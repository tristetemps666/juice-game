using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Diagnostics;
using UnityEngine.Rendering.Universal;



public enum GrapplingStates {
    can_shoot,
    is_thrown, 
    is_hooked,
    is_retrieving
}


public class ShootingGrappling : MonoBehaviour
{
    public Camera cam;
    public float length = 10;
    public float throwing_speed = 10f;

    public Color rope_color;

    private bool is_shoot_not_delayed;
    private GameObject go_player;

    private bool can_fire;

    private LineRenderer lineRenderer;
        
    private Vector2 mouse_screen_pos;
    private Vector2 mouse_world_pos;
    private Vector2 player_screen_pos;
    private Rigidbody2D rb2D;

    private Vector3 local_origin_pos;

    private bool is_reloading = false;




    private bool is_thrown = false;

    private bool will_be_hooked = false;


    private float throwing_avancement = 0f;


    private Vector2 endPosition = Vector2.zero;

    
    private GrapplingStates grapplingStates;
    // Start is called before the first frame update

    void Start()
    {

        local_origin_pos = transform.localPosition;
        is_shoot_not_delayed = true;
        rb2D = gameObject.GetComponentInParent<Rigidbody2D>();
        go_player = gameObject.GetComponentInParent<Movements>().gameObject;

        SetupLineRenderer();

    }

    // Update is called once per frame
    void Update()
    {
        can_fire =  is_shoot_not_delayed==true && !is_reloading && !is_thrown;


        if(GameManager.Instance.actual_game_state == GameManager.GameState.game){

            switch(grapplingStates){
                case GrapplingStates.can_shoot:
                    if(lineRenderer.enabled){
                        lineRenderer.enabled = false;
                    }

                    if (Input.GetMouseButtonDown(1)){
                        startThrowGrappling();
                    }
                    break;

                case GrapplingStates.is_thrown:
                    updateGrapplingPositions();
                    throwing_avancement+=Time.deltaTime*throwing_speed;
                    throwing_avancement = Mathf.Min(throwing_avancement,1);

                    if(throwing_avancement >= 0.99){
                        throwing_avancement = 1f;
                        if(will_be_hooked) {
                            grapplingStates = GrapplingStates.is_hooked;
                            DistanceJoint2D joint2D = go_player.GetComponent<DistanceJoint2D>();
                            if(joint2D == null)
                            joint2D = go_player.AddComponent<DistanceJoint2D>();
                            else joint2D.enabled = true;
                            joint2D.connectedAnchor= endPosition;
                            joint2D.distance = Vector2.Distance(endPosition,go_player.transform.position);
                        }
                        else grapplingStates = GrapplingStates.is_retrieving;
                    }
                    break;
                
                case GrapplingStates.is_hooked:
                    updateGrapplingPositions();
                    if(Input.GetMouseButtonUp(1) || !Input.GetMouseButton(1)){
                        grapplingStates = GrapplingStates.is_retrieving;
                        go_player.GetComponent<DistanceJoint2D>().enabled = false;
                    }
                    break;

                case GrapplingStates.is_retrieving:
                    will_be_hooked = false;
                    updateGrapplingPositions();

                    throwing_avancement-=Time.deltaTime*throwing_speed;
                    throwing_avancement = Mathf.Max(throwing_avancement,0);

                    if(throwing_avancement <= 0.01){
                        throwing_avancement = 0f;
                        grapplingStates = GrapplingStates.can_shoot;
                    }
                    break;



            }
        }
        else{
            // if(text.gameObject.activeInHierarchy) text.gameObject.SetActive(false);
        }
    }


    // Get the normalized direction between the player and the mouse (world space)

    public void SetupLineRenderer(){
        lineRenderer = GetComponent<LineRenderer>();
        if(lineRenderer == null){
            gameObject.AddComponent<LineRenderer>();
            lineRenderer = GetComponent<LineRenderer>();

            lineRenderer.positionCount = 2;
            lineRenderer.startColor =rope_color;
            lineRenderer.endColor =rope_color;
            lineRenderer.widthMultiplier = 0.1f;

            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        }else{ UnityEngine.Debug.Log("merdouille line renderer");}
    }


    public Vector2 GetDirection(){
        mouse_screen_pos = Input.mousePosition; // screen space
        mouse_world_pos = cam.ScreenToWorldPoint(new Vector3(mouse_screen_pos.x,mouse_screen_pos.y,0.0f));
        return (mouse_world_pos - rb2D.position).normalized;
    }

    public float GetSignedAngle(){
        return Vector2.SignedAngle(Vector2.right,GetDirection());
    }

    void startThrowGrappling(){
        is_shoot_not_delayed =false;
        grapplingStates = GrapplingStates.is_thrown;

        lineRenderer.enabled = true;

        UnityEngine.Debug.Log("position de depart :"+ rb2D.position + " // position d'arrivee : " + rb2D.position + GetDirection()*length);
        endPosition = rb2D.position + GetDirection()*length;

        int layerMask = 1<<7;
        UnityEngine.Debug.DrawLine(rb2D.position, endPosition, Color.magenta,0.5f);
        RaycastHit2D hit = Physics2D.Raycast(rb2D.position,GetDirection(),length+2,layerMask);

        if (hit.collider != null){
            UnityEngine.Debug.Log("position du hit :"+ hit.point);
            will_be_hooked = true;
            endPosition = hit.point;
        }
    }

    void updateGrapplingPositions(){
        lineRenderer.SetPosition(0,rb2D.position);
        lineRenderer.SetPosition(1,Vector2.Lerp(rb2D.position,endPosition,throwing_avancement));

    }

}