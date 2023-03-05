using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb2D;
    public float bullet_speed;

    private Vector2 direction;

    private  float lifeTime;

    // Start is called before the first frame update
    void Start()
    {
        lifeTime = 5.0f;
        rb2D = GetComponent<Rigidbody2D>();
        //rb2D.AddForce(direction*bullet_speed);
        rb2D.AddForce(transform.right*bullet_speed);

    }

    // Update is called once per frame
    void Update()
    {
        // avoid infinity lifetime bullet
        lifeTime-=Time.deltaTime;
        if (lifeTime<=0) Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision){
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "bullet"){ // avoid the bullet to be destroyed by the player
            return;
        }else{
            Destroy(gameObject);
        }
    }
}
