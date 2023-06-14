using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class check_future_collision : MonoBehaviour
{
    // Start is called before the first frame update
    public bool close_from_a_collision = false;
    public bool can_check_if_close = false; // we want to check if we can bounce, otherwise, we don't want to see (NO NEED ??)

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.tag == "Plateform") close_from_a_collision = true;
    }

    void OnCollisionExit2D(Collision2D collision){
        if(collision.gameObject.tag == "Plateform" && close_from_a_collision == true) close_from_a_collision = false;
    }
}
