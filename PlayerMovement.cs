using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Vector2 pos_iniziale;

    private float horizontal;
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float runSpeed = 8f;
    private float speed;
    public bool isFacingRight = true;
    private int nJumps = 0;
    private float previousDirection = 0;
    private bool rollPressed = false;
    private float startBoxColliderSizeX;
    private float startBoxColliderSizeY;
    private float startBoxColliderOffsetX;
    private float startBoxColliderOffsetY;
    private LayerMask startBoxColliderLayerMask;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField, Tooltip("This parameter allows you to set the power of the player's jump. So it can be used with the gravity scale to change the speed " +
        "of the jump")] private float jumpingPower = 6f;
    [SerializeField] private BoxCollider2D collisionWithoutEnemy;

    //Variabili che permettono di modificare lo spazio che occupa il BoxCollider. Da usare durante la rotolata (roll).
    [Header("BoxCollider changes during roll animation:")] //Header: permette di creare una raccolta di richieste presenti nel suddetto componente una volta presente nell'inspector, aiutando, quindi, ad organizzare meglio le richieste di tipo [SerializeField] quando vengono visualizzate.
    [SerializeField] private float changeBoxColliderSizeInrollX = 0.2f;
    [SerializeField] private float changeBoxColliderSizeInrollY = 0.1f;
    [SerializeField] private float changeBoxColliderOffsetInrollX = -0.15f;
    [SerializeField] private float changeBoxColliderOffsetInrollY = -0.14f;

    private Rigidbody2D player;
    private Animator animator;
    private PlayerAttack playerAttack;
    private Health health;
    //[SerializeField] private Transform groundCheck;

    void Start()
    {
        player = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        playerAttack = gameObject.GetComponent<PlayerAttack>();
        health = GetComponent<Health>();
        pos_iniziale = transform.position;
        speed = walkSpeed;
        startBoxColliderSizeX = boxCollider.size.x;
        startBoxColliderSizeY = boxCollider.size.y;
        startBoxColliderOffsetX = boxCollider.offset.x;
        startBoxColliderOffsetY = boxCollider.offset.y;
        startBoxColliderLayerMask = boxCollider.forceSendLayers;
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        animator.SetFloat("Speed", Mathf.Abs(horizontal * speed));

        // Resetta la posizione del PG se cade dallo schermo
        if (transform.position.y < -5.6f) transform.position = pos_iniziale;
        
        movement();
        Flip();
    }

    private void FixedUpdate()
    {
        if (playerAttack.getCanAttack() && !checkBottomEnemy() && health.getHealth() > 0) { player.velocity = new Vector2(horizontal * speed, player.velocity.y); }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log(collision.gameObject.name + ", tag: "+ collision.collider.tag + ", Layer: " + collision.gameObject.layer);
        if (collision.collider.tag == "Terrain")
        {
            if (nJumps > 0 && playerAttack.getMainAttack())
            {
                playerAttack.attackFinished();
            }
            nJumps = 0;
        } else if (collision.collider.tag == "Enemy" && rollPressed) {
            boxCollider.forceSendLayers = collisionWithoutEnemy.forceSendLayers;
            boxCollider.forceReceiveLayers = collisionWithoutEnemy.forceReceiveLayers;
        }
    }

    private void OnDrawGizmos() //Il rettangolo disegnato di giallo ai piedi del giocatore potrebbe essere usato in seguito per verificare il terreno e modificare così il contatore del salto solo quando il terreno si trova a contatto con i piedi del giocatore
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector3(boxCollider.bounds.center.x, -1 * (boxCollider.bounds.size.y/2) + boxCollider.bounds.center.y, boxCollider.bounds.center.z),
            new Vector3(boxCollider.bounds.size.x, 0.1f, boxCollider.bounds.size.z));
    }

    private bool checkBottomEnemy()
    {
        if (!rollPressed)
        {
            RaycastHit2D hit = Physics2D.BoxCast(new Vector3(boxCollider.bounds.center.x, -1 * (boxCollider.bounds.size.y / 2) + boxCollider.bounds.center.y, boxCollider.bounds.center.z),
                new Vector3(boxCollider.bounds.size.x, 0.1f, boxCollider.bounds.size.z),
                    0, Vector2.right, 0, enemyLayer);
            if (hit.collider != null)
            {
                if (hit.collider.tag == "Enemy")
                {
                    if (previousDirection == 0 && nJumps > 0)
                    {
                        previousDirection = transform.localScale.x;
                        player.velocity = new Vector2(previousDirection * 10, player.velocity.y);
                    }
                    player.velocity = new Vector2(previousDirection * 10, player.velocity.y);
                    return true;
                }
                else
                {
                    previousDirection = 0;
                }
            }
            else
            {
                previousDirection = 0;
            }
        }
        return false;
    }

    //************ Codice per far si che salti una sola volta usando una variabile booleana ************
    /*void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.name == "Terrain" && collision.gameObject.layer == 6)
        {
            canJump = false;
        }
    }*/

    /*private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundLayer);
    }*/

    private void Flip() //Codice per cambiare la direzione dell'immagine del personaggio
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    public void movement() {
        jumpChecker();
        rollChecker();
        runChecker();
    }

    private void jumpChecker()
    {
        if (Input.GetButtonDown("Jump") && nJumps < 2 && !rollPressed)
        {
            nJumps++;
            player.velocity = new Vector2(player.velocity.x, jumpingPower);
        }
        animator.SetInteger("nJump", nJumps);
    }

    private void runChecker() {
        if (Input.GetKey(KeyCode.LeftControl) && !rollPressed)
        {
            speed = runSpeed;
        }
        else
        {
            if (speed > walkSpeed && rollPressed)
            {
                speed -= 0.05f;
            } else
            {
                speed = walkSpeed;
            }
        }
    }

    private void rollChecker()
    {
        if ((horizontal >= 1 || horizontal <= -1) && Input.GetKeyDown(KeyCode.LeftShift)) {
            rollPressed = !rollPressed;
            speed = runSpeed;
            if (rollPressed == false)
            {
                animationRollFinished();
            }
        } else if (horizontal == 0 && rollPressed)
        {
            animationRollFinished();
        }
        animator.SetBool("strike", rollPressed);
    }

    private void rotateColliderSizeInRoll() //Usato in animazione
    {
        boxCollider.forceSendLayers = collisionWithoutEnemy.forceSendLayers;
        boxCollider.forceReceiveLayers = collisionWithoutEnemy.forceReceiveLayers;
        boxCollider.size = new Vector2(startBoxColliderSizeY + changeBoxColliderSizeInrollX, startBoxColliderSizeX + changeBoxColliderSizeInrollY);
        boxCollider.offset = new Vector2(startBoxColliderOffsetX + changeBoxColliderOffsetInrollX, startBoxColliderOffsetY + changeBoxColliderOffsetInrollY);
    }

    private void animationRollFinished() //Usato anche in animazione
    {
        rollPressed = false;
        boxCollider.offset = new Vector2(startBoxColliderOffsetX, startBoxColliderOffsetY);
        boxCollider.size = new Vector2(startBoxColliderSizeX, startBoxColliderSizeY);
        boxCollider.forceSendLayers = startBoxColliderLayerMask;
        boxCollider.forceReceiveLayers = startBoxColliderLayerMask;
    }

    public void knockbackDirection(float direction) {
        player.velocity = new Vector2((direction * speed), player.velocity.y);
    }

    public bool getRollPressed() { return rollPressed; }

}