using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Tilemaps;
using UnityEditor.UIElements;
using UnityEngine;

public enum TourelleState{
    waiting,
    aiming,
};
public class Tourelle : MonoBehaviour
{
    public string enemy_tag = "player";

    public float bullet_speed = 2f;
    public float fire_rate = 1f;

    public float predictiveness = 1f;

    public float rotate_speed = 20f;
    public float rotation_max  = 80f;

    public GameObject bullet;

    private TourelleState tourelle_state = TourelleState.waiting;

    public float rotation_amount = 0f;
    private int rotation_sign = 1;

    private Transform circle_transform;

    private Transform player_transform;
    // Start is called before the first frame update
    void Start()
    {
        // GET CIRCLE TRANSFORM
        var transforms = GetComponentsInChildren<Transform>();
        foreach(Transform transform in transforms){
            if(transform.gameObject.name == "Circle") circle_transform = transform;
            if(transform.gameObject.name == "bullet") bullet = transform.gameObject;
        }

    }

    // Update is called once per frame
    void Update()
    {
        switch (tourelle_state){
            case TourelleState.waiting:
            
                update_waiting_rotation();

                break;

            case TourelleState.aiming:
                // circle_transform.LookAt(player_transform);
                look_at_player();

                break;
        }
    }



    void update_waiting_rotation(){
        float angle_value = circle_transform.rotation.eulerAngles.z > 180 ? 
            circle_transform.rotation.eulerAngles.z-360f 
            : circle_transform.rotation.eulerAngles.z ;

        circle_transform.Rotate(Vector3.forward*rotation_amount);

        if(angle_value >= rotation_max && rotation_sign == 1){
            rotation_sign = -1;
        }
        if(angle_value <= -rotation_max && rotation_sign == -1){
            rotation_sign = 1;
        }
        rotation_amount = rotate_speed*Time.deltaTime*rotation_sign;

    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == enemy_tag && tourelle_state == TourelleState.waiting){
            Debug.Log("J AI CAPTE LE JOUEUR");
            tourelle_state = TourelleState.aiming;

            player_transform = other.transform;

            InvokeRepeating("shoot",fire_rate,fire_rate);

        }
    }


    private void OnTriggerExit2D(Collider2D other) {
        if(other.tag == enemy_tag &&  tourelle_state == TourelleState.aiming){
            Debug.Log("ORVOIR LE JOUEUR");
            tourelle_state = TourelleState.waiting;

            CancelInvoke("shoot");

        }
    }

    private void shoot(){
        Debug.Log("PAN !!");
        var go_bullet = Instantiate<GameObject>(bullet);
        go_bullet.SetActive(true);

        go_bullet.transform.rotation = circle_transform.transform.rotation;
        go_bullet.transform.position =  circle_transform.transform.position -2f*circle_transform.transform.up;

        if(go_bullet.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb)){
            rb.AddForce(-circle_transform.transform.up*bullet_speed);
        }
    }

    private void look_at_player(){
        Vector3 diff = player_transform.position-circle_transform.position;

        Vector2 forward_to_player = new Vector2(diff.x,diff.y);
        forward_to_player.Normalize();


        Vector2 circle_up_world_vec2 = new Vector2 (circle_transform.up.x, circle_transform.up.y);

        Debug.DrawRay(circle_transform.position, circle_transform.up);

        float angle = Vector2.SignedAngle(forward_to_player,-circle_up_world_vec2);

        circle_transform.Rotate(-angle*Vector3.forward);

    }
}