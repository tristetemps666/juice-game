using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class EndLevel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void LoadNextLevel(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }

    void OnTriggerEnter2D(Collider2D collision){
        if (collision.gameObject.tag == "Player"){
            Invoke("LoadNextLevel",1f);
            //LoadNextLevel();
        }
    }
}
