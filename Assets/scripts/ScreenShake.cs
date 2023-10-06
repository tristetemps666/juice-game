using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    // Start is called before the first frame update
    public float shake_strenght;
    public float shake_fade;

    private float shake_amount;
    void Start()
    {
        
    }

    public void StartShake(float strenght_factor){
        shake_amount = shake_strenght*strenght_factor;
    }
    // Update is called once per frame
    void Update() // TODO (pb c que ça se fait à la meme vitesse meme pendant le slowmo 
    {
        shake_amount = Mathf.Max(shake_amount-shake_fade*Time.deltaTime,0f);
        Vector2 displacement = new Vector2(Random.Range(-shake_amount,shake_amount),Random.Range(-shake_amount,shake_amount));
        transform.Translate(displacement);
    }
}
