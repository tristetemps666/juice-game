using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxAmmo : MonoBehaviour
{
    private Shooting shooting_script;
    public int amount_of_ammo;
    public float oscillation_amplitude;
    public float oscilaltion_frequency;
    public Vector3 originPosition;

    // Start is called before the first frame update
    void Start()
    {
        originPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //float angle = 360f*(2*Mathf.PI*oscilaltion_frequency*Time.deltaTime)/(2*Mathf.PI);
        Vector3 oscillation = new Vector3(0f,oscillation_amplitude*Mathf.Sin(2*Mathf.PI*oscilaltion_frequency*Time.time),0f);
        transform.position = originPosition+oscillation;

    }
}
