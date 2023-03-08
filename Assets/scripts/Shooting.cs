using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Shooting : MonoBehaviour
{
    public Camera cam;
    public float fire_rate;
    public float dammage;
    private int bullet_in_gun;
    public int magazine_size;
    public int bullet_remaining;
    public bool infinit_ammo;

    private bool can_fire;

    public GameObject bullet;

    public float recoil_strength;
        
    private Vector2 mouse_screen_pos;
    private Vector2 mouse_world_pos;
    private Vector2 player_screen_pos;
    private Rigidbody2D rb2D;

    private Vector3 local_origin_pos;
    public float recoil_lenght;
    private float recoil_factor;
    public float recoil_recover_speed;

    public Shooting_VFX shooting_vfx_script;
    public GameObject muzzle_smoke;

    public float reload_delay;
    private bool is_reloading = false;

    public GameObject Canvas_UI;
    private SliderReload slider_reload_script;


    [SerializeField]  ScreenShake screen_shake_script;
    
    
    //public TextMeshPro text;
    public TextMeshProUGUI text;
    // Start is called before the first frame update

    void Start()
    {

        bullet_in_gun = magazine_size;
        local_origin_pos = transform.localPosition;
        can_fire = true;
        rb2D = gameObject.GetComponentInParent<Rigidbody2D>();
        slider_reload_script = Canvas_UI.GetComponent<SliderReload>();
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.actual_game_state == GameManager.GameState.game){
            
            UpdateRecoil();

            transform.parent.rotation = Quaternion.AngleAxis(GetSignedAngle(),Vector3.forward);
            
            DisplayAmmo();


            if (Input.GetMouseButtonDown(0)){
                Shoot();
            }

            if (Input.GetKeyDown(KeyCode.R)){
                Reload();
            }
        }
        else{
            if(text.gameObject.activeInHierarchy) text.gameObject.SetActive(false);
        }
    }


    // Get the normalized direction between the player and the mouse (world space)
    public Vector2 GetDirection(){
        mouse_screen_pos = Input.mousePosition; // screen space
        mouse_world_pos = cam.ScreenToWorldPoint(new Vector3(mouse_screen_pos.x,mouse_screen_pos.y,0.0f));
        return (mouse_world_pos - rb2D.position).normalized;
    }

    public float GetSignedAngle(){
        return Vector2.SignedAngle(Vector2.right,GetDirection());
    }

    void Shoot(){
        if((bullet_in_gun>=1 ||infinit_ammo) && can_fire==true){
            ApplyRecoil();
            // do the shoot

            // enable bounce
            GetComponentInParent<Movements>().set_player_bounciness();



            // Do the vfx
            shooting_vfx_script.start_vfx();
            shooting_vfx_script.Spawntrails();
            
            Instantiate(muzzle_smoke,rb2D.position,new Quaternion(0.0f,0.0f,0.0f,0.0f));

            //Instantiate(bullet,rb2D.position,new Quaternion(0.0f,0.0f,0.0f,0.0f));
            Instantiate(bullet,rb2D.position,Quaternion.AngleAxis(GetSignedAngle(),Vector3.forward));
            Instantiate(bullet,rb2D.position,Quaternion.AngleAxis(GetSignedAngle()+15f,Vector3.forward));
            Instantiate(bullet,rb2D.position,Quaternion.AngleAxis(GetSignedAngle()-15f,Vector3.forward));

            rb2D.velocity *= 0.1f;
            rb2D.AddForce(-GetDirection()*recoil_strength);
            
            screen_shake_script.StartShake();
            
            bullet_in_gun-=1;
            can_fire =false;
            StartCoroutine(ShootingDelay());

        }
    }

    void Reload(){
        if (bullet_in_gun == magazine_size || bullet_remaining ==0 || is_reloading) return; // the mag is full of we don't have more bullet, we can't reload
        else{
                can_fire = false;
                is_reloading = true;
                slider_reload_script.start_reload_UI_anim(reload_delay);
                Invoke("effectiveReload",reload_delay);
        }
    }

    void effectiveReload(){
        int difference = magazine_size-bullet_in_gun; // what I need to add

        bullet_in_gun = difference >= bullet_remaining ? // do I have enough
            bullet_in_gun+bullet_remaining : // no
            magazine_size; // yes

        bullet_remaining = Mathf.Max(0,bullet_remaining-difference); // update the ammo remaining
        can_fire = true;
        is_reloading = false;
    }

    void ApplyRecoil(){
        //transform.position = -GetDirection()*recoil_lenght;
        //transform.Translate(new Vector3(-recoil_lenght,0f,0f),Space.Self);
        recoil_factor = 1f;
    }
    void UpdateRecoil(){
        //transform.position = Vector3.SmoothDamp(transform.position,transform.parent.position+original_offset_to_parent,ref recoil_velocity_recover,recoil_smooth_recover);
        recoil_factor = Mathf.Max(0f,recoil_factor-Time.deltaTime*recoil_recover_speed);
        //transform.localPosition = offset_position_to_parent;

        //transform.Translate(new Vector3(-recoil_lenght,0f,0f),Space.Self);
        transform.localPosition = Vector3.Lerp(local_origin_pos,new Vector3(-recoil_lenght,0f,0f),recoil_factor);

    }


    public void AddBullet(int n){
        bullet_remaining+=n;
    }

    void DisplayAmmo(){
        if(!text.gameObject.activeInHierarchy) text.gameObject.SetActive(true);

        string value = bullet_in_gun.ToString() + " / " + bullet_remaining.ToString();
        if (bullet_in_gun <=2){
            value =  "<color=red>" + bullet_in_gun.ToString() + "</color>" + " / " + bullet_remaining.ToString();
        }
        text.SetText(value);
    }



    IEnumerator ShootingDelay(){
        yield return new WaitForSeconds(1/fire_rate);
        can_fire = true;
        shooting_vfx_script.end_vfx();
    }
}
