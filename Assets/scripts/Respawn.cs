using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Respawn : MonoBehaviour
{
    private Rigidbody2D rb2D;
    private Vector2 respawn_position;
    private Vector2 start_position;
    // Start is called before the first frame update
    void Start()
    {
        rb2D = gameObject.GetComponent<Rigidbody2D>();
        start_position = rb2D.position;
        respawn_position = rb2D.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.X)) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        if(Input.GetKeyDown(KeyCode.C)) rb2D.position = respawn_position;

    }

    void OnTriggerEnter2D(Collider2D collision){
        if (collision.gameObject.tag == "Checkpoint"){
            respawn_position = collision.gameObject.transform.position;
        }
    }
}
