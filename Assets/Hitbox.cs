using Unity.VisualScripting;
using UnityEngine;

public class Hitbox : MonoBehaviour{
    public GameObject hitEffect;
    public Vector3 knockback;
    public float damage;
    private int lifespan;
    public int hitstun;
    public PlayerController player;
    public void SetLifespan(int l){
        lifespan = l;
    }
    // void OnTriggerEnter(Collider other){
    //     print("Triggerred");
    //     if(other.gameObject.tag == "Player"){
    //         print("Hit");
    //         // Hitbox hits
    //         var p = other.gameObject.GetComponent<PlayerController>();
    //         p.Hit(knockback, damage, hitstun);
    //         var hit = Instantiate(hitEffect);
    //         hit.transform.position = this.transform.position;
    //         // Remember to destroy the hitbox after a hit
    //         Destroy(gameObject);
    //     }
    // }
    void FixedUpdate(){
        lifespan -= 1;
        if(lifespan == 0){
            Destroy(this.gameObject);
        }
    }

}