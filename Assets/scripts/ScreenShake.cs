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

    public void StartShake(){
        shake_amount = shake_strenght;
    }
    // Update is called once per frame
    void Update()
    {
        shake_amount = Mathf.Max(shake_amount-shake_fade*Time.deltaTime,0f);
        Vector2 displacement = new Vector2(Random.Range(-shake_amount,shake_amount),Random.Range(-shake_amount,shake_amount));
        transform.Translate(displacement);
    }
}
