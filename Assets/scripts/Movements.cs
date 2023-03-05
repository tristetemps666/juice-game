using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Movements : MonoBehaviour

{

    private Rigidbody2D rb2D;
    private PhysicsMaterial2D pm2D;

    private Shooting shooting_script;
    public float horizontal_speed;
    public float vertical_speed;

    public AnimationCurve horizontal_speedGrowCurve;
    private float horizontal_input;
    public float horizontal_acceleration_step;
    private float horizontal_acceleration_increase;
    private float acceleration_factor;

    public float fall_speed_factor;
    private float original_gravity;
    private float vertical_input;

    private bool is_jump;
    public float air_drag;
    public float ground_drag;
    public float air_dash_factor;
    public float delay_jump;
    private float can_jump;

    public float rebound_factor_after_shoot;

    public GameObject jump_effect;
    private Transform position_to_spawn_jump_effect;

    // Start is called before the first frame update
    void Start()
    {
        shooting_script = GetComponentInChildren<Shooting>();
        rb2D = GetComponent<Rigidbody2D>();
        pm2D = GetComponent<PhysicsMaterial2D>();

        is_jump = true;
        can_jump = 0f;
        horizontal_acceleration_increase = 0.0f;
        acceleration_factor = 0.0f;
        original_gravity = rb2D.gravityScale;

    }

    // Update is called once per frame

    void Update(){
        // pm2D.bounciness = shooting_script.GetHasShootAndNotCollide() ?
        //     rebound_factor_after_shoot :
        //     0f;

        horizontal_input = Input.GetAxisRaw("Horizontal");
        vertical_input = Input.GetAxisRaw("Vertical");

    }
    void FixedUpdate()
    {
        ////////////////  Vertical movement/////////////////////////
        if(is_jump) can_jump = Mathf.Max(can_jump-Time.deltaTime,0f);
        if(vertical_input>0.1f && can_jump>0f){
            is_jump = true;
            can_jump= 0f;

            rb2D.velocity*= new Vector2(1f,0.1f);
            Vector2 Yspeed = new Vector2(0.0f,vertical_input*vertical_speed*Time.deltaTime);
            rb2D.AddForce(Yspeed,ForceMode2D.Impulse);
        }

        if (rb2D.velocity.y < -0.1f){
            rb2D.gravityScale=fall_speed_factor;
        }else{
            rb2D.gravityScale = original_gravity;
        }



        ////////////////  horizontal movement/////////////////////////

        acceleration_factor = horizontal_speedGrowCurve.Evaluate(horizontal_acceleration_increase);
        if (Mathf.Abs(horizontal_input) < 0.1f) {
            horizontal_acceleration_increase  = Mathf.Max(horizontal_acceleration_increase-horizontal_acceleration_step,0.0f);
        }

        // When the player is pressing left or right
        else{
            float air_factor = is_jump ? air_dash_factor : 1f;
            horizontal_acceleration_increase  = Mathf.Clamp(horizontal_acceleration_increase+horizontal_acceleration_step,0.0f,1.0f);
            Vector2 Xspeed = new Vector2(acceleration_factor*horizontal_speed*horizontal_input*Time.deltaTime,0.0f);
            rb2D.AddForce(air_factor*Xspeed,ForceMode2D.Impulse);
        }

    }


    void OnTriggerEnter2D(Collider2D collider){
        if (collider.gameObject.tag == "AmmoBox"){
            Debug.Log("box");
            shooting_script.AddBullet(collider.gameObject.GetComponent<BoxAmmo>().amount_of_ammo);
            Destroy(collider.gameObject);
        }

        if (collider.gameObject.tag == "BoundBox"){
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }



    }

    // if the player collides with something
    void OnCollisionEnter2D(Collision2D collision){
    // is it from below ?
        

        if (collision.gameObject.tag == "Plateform"){

            is_jump = false;
            can_jump = delay_jump;
            rb2D.drag = ground_drag;

            // play landing VFX
            position_to_spawn_jump_effect = GetComponentsInChildren<Transform>()[1]; // works only if it is in first place


            float strenght = Mathf.Clamp(collision.relativeVelocity.sqrMagnitude/100f,0.3f,15f);
            ParticleSystem part = jump_effect.GetComponent<ParticleSystem>();

            part.startColor = collision.gameObject.GetComponent<SpriteRenderer>().color;

            part.emission.SetBurst(0,new ParticleSystem.Burst(0f,(int)(2f*strenght)));

            Instantiate(jump_effect,position_to_spawn_jump_effect.position,new Quaternion(0f,0f,0f,0f));
            

        }

        // if (collision.gameObject.tag == "AmmoBox"){
        //     Debug.Log("box");
        //     shooting_script.AddBullet(collision.gameObject.GetComponent<BoxAmmo>().amount_of_ammo);
        //     Destroy(collision.gameObject);
        // }


        if (collision.gameObject.tag == "BoundBox"){
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }



        void OnCollisionExit2D(Collision2D collision){
    // is it from below ?
        if (collision.gameObject.tag == "Plateform"){
            is_jump = true;
            rb2D.drag = air_drag;

            //if(shooting_script.GetHasShootAndNotCollide()) shooting_script.SetHasShootAndNotCollide(false); REBOUND MAYBE
        }

    }

}
