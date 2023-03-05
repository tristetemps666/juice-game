using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Shooting_VFX : MonoBehaviour
{
    public Material gunMat;
    private float t;
    private float speed;
    private bool has_just_shoot;
    // Start is called before the first frame update



    /// PLAYER trail EFFECT

    public float activeTime = 1f;
    private bool isTrailActive;

    public float meshRefreshRate = 0.1f;

    public Transform player_trans;
    public SpriteRenderer player_spr_rend;

    public Gradient trail_gradient;
    public float meshtimetodie = 0.2f;

    public int count = 10;

    void Start()
    {
        speed = gunMat.GetFloat("_speed");
        has_just_shoot = false;
        t = 0.29f;
        update_material_t();
        isTrailActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        update_t();
        update_material_t();

        // if(isTrailActive) StartCoroutine(ActivateTrail(activeTime));
    }

    void update_t(){
        if(has_just_shoot){ // if i just shooted i update
            t += Time.deltaTime*speed;
            t = Mathf.Clamp(t,0.29f,1.1f);
        }else {
            t=0.1f; // else i do nothing
        }
    }

    public void update_material_t(){
        gunMat.SetFloat("_t",t);
    }

    public void set_has_shoot(bool val){
        has_just_shoot = val;
    }

    public void start_vfx(){
        set_has_shoot(true);
    }
    public void end_vfx(){
        set_has_shoot(false);
    }

    GameObject GetParentPlayerGameObject(){
        GameObject[] listGO = GetComponentsInParent<GameObject>();
        foreach(GameObject go in listGO){
            if (go.name == "Player") return go;
        }
        return null;
    }


    public void Spawntrails(){
        isTrailActive = true;
        StartCoroutine(ActivateTrail(count));
    }

    IEnumerator ActivateTrail(int number){
        while(number >0f){

            number-=1;
            // do the render here
            GameObject go = new GameObject();
            go.transform.SetPositionAndRotation(player_trans.position,player_trans.rotation);
            go.AddComponent<SpriteRenderer>();
            SpriteRenderer sprRend =  go.GetComponent<SpriteRenderer>();
            // sprRend = player_spr_rend;
            sprRend.sprite = player_spr_rend.sprite;
            sprRend.sortingLayerName = "Default"; // dash layer wich is before the player


            sprRend.color = trail_gradient.Evaluate(1f*number/count);
            Destroy(go,meshtimetodie);

            // Instantiate(go,player_trans);
            yield return new WaitForSeconds(meshRefreshRate);
        }
        isTrailActive = false;
    }

}