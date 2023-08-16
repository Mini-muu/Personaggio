using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KillContact : MonoBehaviour
{
    public float speed;
    public float distance;
    public Transform detection;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate() {
        transform.Translate(Vector2.left*speed);
        RaycastHit2D groundCheck =Physics2D.Raycast(transform.position,Vector2.down, distance);
        if(groundCheck.collider == false){
            transform.Rotate(0f,180f,0f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.CompareTag("Player")){
            SceneManager.LoadScene("LoseScreen");
        }
        
    }
}
