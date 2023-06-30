using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class Movements : MonoBehaviour

{

    public float max_speed;
    private Rigidbody2D rb2D;
    private PhysicsMaterial2D pm2D;
    public const  float bounciness_after_shoot = 1.5f;
    public float bounce_time;
    public float time_to_bounce = 0f;

    private bool can_bounce = false;


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

    public float dash_gravity;
    public float air_dash_factor;
    public float delay_jump;
    private float can_jump;

    public GameObject jump_effect;
    private Transform position_to_spawn_jump_effect;


    public float water_gravity = -10f;
    public float water_height_point_enter = 0f;
    public bool enter_water = false;

    // Start is called before the first frame update
    void Start()
    {
        shooting_script = GetComponentInChildren<Shooting>();
        rb2D = GetComponent<Rigidbody2D>();
        pm2D = rb2D.sharedMaterial;
        pm2D.bounciness = 1f;
        rb2D.sharedMaterial = pm2D;

        is_jump = true;
        can_jump = 0f;
        horizontal_acceleration_increase = 0.0f;
        acceleration_factor = 0.0f;
        original_gravity = rb2D.gravityScale;

    }

    // Update is called once per frame

    void Update(){


    }

    void FixedUpdate()
    {
        if (GameManager.Instance.actual_game_state ==GameManager.GameState.game){
            update_bounce();

            horizontal_input = Input.GetAxisRaw("Horizontal");
            vertical_input = Input.GetAxisRaw("Vertical");
        }
        if (GameManager.Instance.actual_game_state ==GameManager.GameState.game){
            set_gravity_along_jump();
            set_drag_along_jump();
            handle_water_interraction();
            vertical_movement();
            horizontal_movement();
            rb2D.velocity = Vector2.ClampMagnitude(rb2D.velocity, max_speed);
        }

    }

    private void update_bounce(){
        time_to_bounce = Mathf.Max(time_to_bounce-Time.deltaTime,0f);
        set_player_bounciness(bounciness_after_shoot*(Mathf.Max(time_to_bounce/bounce_time,0.5f)));
        if (time_to_bounce <= 0.001f &&rb2D.sharedMaterial.bounciness >=0f) set_player_bounciness(0f);
    }

// MOVEMENTS
    private void set_gravity_along_jump(){
        rb2D.gravityScale = time_to_bounce > 0.7 *bounce_time ? dash_gravity:
                            (rb2D.velocity.y < -0.1f) ? fall_speed_factor:
                            original_gravity;
    }


    private void set_drag_along_jump(){
        rb2D.drag = time_to_bounce > 0.7*bounce_time ? rb2D.drag:
                            is_jump ? air_drag :
                                      ground_drag;
    }


    private void vertical_movement(){
            ////////////////  Vertical movement/////////////////////////
        if(is_jump) can_jump = Mathf.Max(can_jump-Time.deltaTime,0f);
        if(vertical_input>0.1f && can_jump>0f){
            is_jump = true;
            can_jump= 0f;

            rb2D.velocity*= new Vector2(1f,0.1f);
            Vector2 Yspeed = new Vector2(0.0f,vertical_input*vertical_speed*Time.deltaTime);
            rb2D.AddForce(Yspeed,ForceMode2D.Impulse);
        }    
    }

    private void horizontal_movement(){
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

    public void set_player_bounciness(float bounce = bounciness_after_shoot){
        PhysicsMaterial2D pm = new PhysicsMaterial2D();
        pm.bounciness = bounce;
        pm.friction = 0f;

        rb2D.sharedMaterial = pm;
    }
    public void reset_time_to_bounce(){
        time_to_bounce = bounce_time;
    }












// TRIGGERS COLLIDERS AND COLLISION

    void OnTriggerEnter2D(Collider2D collider){
        if (collider.gameObject.tag == "AmmoBox"){
            shooting_script.AddBullet(collider.gameObject.GetComponent<BoxAmmo>().amount_of_ammo);
            Destroy(collider.gameObject);
        }

        if (collider.gameObject.tag == "AmmoInstantBox"){
            if(shooting_script.FillCap())
                Destroy(collider.gameObject);
        }

        if (collider.gameObject.tag == "BoundBox"){
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }



    }

    // if the player collides with something
    void OnCollisionEnter2D(Collision2D collision){
    // is it from below ?
        

        if (collision.gameObject.tag == "Plateform" || collision.gameObject.tag == "PlateformAmmo"){

            is_jump = false;
            can_jump = delay_jump;
            rb2D.drag = ground_drag;

            // play landing VFX
            position_to_spawn_jump_effect = GetComponentsInChildren<Transform>()[1]; // works only if it is in first place


            float strenght = Mathf.Clamp(collision.relativeVelocity.sqrMagnitude/100f,0.3f,15f);
        
            ParticleSystem part = jump_effect.GetComponent<ParticleSystem>();


    
            part.startColor = color_from_collision(collision);

            part.emission.SetBurst(0,new ParticleSystem.Burst(0f,(int)(2f*strenght)));

            Instantiate(jump_effect,position_to_spawn_jump_effect.position,new Quaternion(0f,0f,0f,0f));



            if(collision.gameObject.tag == "PlateformAmmo"){
                shooting_script.FillCap();

            }
            

        }

        if (collision.gameObject.tag == "BoundBox"){
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }



        void OnCollisionExit2D(Collision2D collision){
    // is it from below ?
        if (collision.gameObject.tag == "Plateform" || collision.gameObject.tag == "PlateformAmmo"){
            is_jump = true;
            rb2D.drag = air_drag;
            set_player_bounciness(0f);

            //if(shooting_script.GetHasShootAndNotCollide()) shooting_script.SetHasShootAndNotCollide(false); REBOUND MAYBE
        }

    }

    public bool is_in_water(Vector3 position){
        int layer_water = 11; // CAN Change, check in unity Water layer
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position,0f);
        Debug.Log(colliders.Length);
        if(colliders.Length >0){
            foreach(Collider2D collider2D in colliders){
                Debug.Log(collider2D.tag + "\n");
                if(collider2D.tag == "Water") return true;
            }
        }
        return false;
    }

    public void handle_water_interraction(){
        if(is_in_water(transform.position)){
            if(!enter_water) {
                Vector3 top_water = transform.position;
                while(is_in_water(top_water)){
                    top_water+=Vector3.up*0.5f;
                }
                water_height_point_enter = top_water.y;
                enter_water = true;
            }
            float factor = Mathf.Abs(transform.position.y-water_height_point_enter);
            factor = (rb2D.velocity.y<0) ? factor*=0.3f : factor*= 0.4f;
            factor = Mathf.Clamp(factor,0f,3f);
            rb2D.gravityScale = water_gravity*(factor*factor);
            // rb2D.AddForce(factor*factor*10*Vector2.up,ForceMode2D.Force);
            Debug.Log("water_height_point_enter : "  + water_height_point_enter + "factor : "  + factor + "\n\n" + "enter water :  " + enter_water + "\n\n");
        }else{
            enter_water = false;
            set_gravity_along_jump(); // reset as in normal
        }
    }
    
    Color color_from_collision(Collision2D collision){
        Tilemap tilemap = collision.gameObject.GetComponent<Tilemap>();

        
        if(tilemap != null){ // je collide avec une collision

                // get the tile hited
                Vector3 hitPosition = collision.GetContact(0).point;
                Vector3Int cellPosition = tilemap.WorldToCell(hitPosition);
                TileBase tile  = tilemap.GetTile(cellPosition);
                // return tilemap.GetColor(cellPosition);


                Sprite sprite = tilemap.GetSprite(cellPosition);
                if(sprite == null) return Color.black;
                Vector3 spritePosition = hitPosition - tilemap.CellToWorld(cellPosition);
                Vector2 textureCoord = new Vector2(spritePosition.x / sprite.rect.width, spritePosition.y / sprite.rect.height);

                return  sprite.texture.GetPixelBilinear(textureCoord.x, textureCoord.y);


                
            }

        else{
            SpriteRenderer Sprite_render_collision = collision.gameObject.GetComponent<SpriteRenderer>();  
            return Color.blue; 
        }

    }


}
