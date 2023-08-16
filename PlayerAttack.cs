using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private bool doDamage = false;
    private bool bladeDown = false;
    private float mainAttackDamage = 1;
    private bool mainAttack = false;
    private float superAttackDamage = 3;
    private bool secondaryAttack = false;
    [SerializeField] private float mainKnockback = 0.66f; //Rinculo
    //private float superKnockback = 1; //Rinculo dell'attacco più potente
    private bool canAttack = true;
    private float newPositionX;
    private Animator animator;
    private PlayerMovement playerMovement;
    //private EnemyHealth enemyHealth;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask enemyLayer;
    [Header("Attack box parameters:")] //Header: permette di creare una raccolta di richieste presenti nel suddetto componente una volta presente nell'inspector, aiutando, quindi, ad organizzare meglio le richieste di tipo [SerializeField] quando vengono visualizzate.
    [SerializeField, Tooltip("Distance of the 'attack' box from the BoxCollider2D")] private float attackRange;
    [SerializeField, Tooltip("Width of the 'attack' box")] private float attackAreaWidth;
    // Start is called before the first frame update
    void Start()
    {
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        animator = gameObject.GetComponent<Animator>();
        newPositionX = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("mainAttack", mainAttack);
        animator.SetBool("secondaryAttack", secondaryAttack);
        mouseInteraction();
    }

    //Importante: notare bene che il metodo sottostante è sempre attivo nell'editor del gioco e non solo durante la simulazione.
    private void OnDrawGizmos()
    {
        float range = (attackAreaWidth / 2f) + attackRange;
        float attackCubeCenter = boxCollider.bounds.center.x + transform.localScale.x * range;

        //Disegno l'area d'attacco della spada.
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(attackCubeCenter, boxCollider.bounds.center.y, boxCollider.bounds.center.z),
            new Vector3(attackAreaWidth, boxCollider.bounds.size.y, boxCollider.bounds.size.z));
    }
    private void mouseInteraction()
    {
        if (Input.GetMouseButtonDown(0) && canAttack) //Click sinistro mouse
        {
            newPositionX = transform.position.x + -transform.localPosition.x * getMainKnockback();
            canAttack = false;
            mainAttack = true;
        }
        else if (Input.GetMouseButtonDown(1) && canAttack) //Click destro mouse
        {
            Debug.Log("Right click");
            newPositionX = transform.position.x + -transform.localPosition.x * getMainKnockback();
            canAttack = false;
            secondaryAttack = true;
        }

        if (bladeDown) //Nel caso in cui il personaggio abbia affondato la spada ricevi il rinculo
        {
            if (playerMovement.isFacingRight)
            {
                if (transform.position.x >= newPositionX)
                {
                    playerMovement.knockbackDirection(-mainKnockback);
                }
            }
            else
            {
                if (transform.position.x <= newPositionX)
                {
                    playerMovement.knockbackDirection(mainKnockback);
                }
            }
        }

        if (doDamage)
        {
            float range = (attackAreaWidth / 2f) + attackRange;
            float attackCubeCenter = boxCollider.bounds.center.x + transform.localScale.x * range;
            RaycastHit2D hit = Physics2D.BoxCast(new Vector3(attackCubeCenter, boxCollider.bounds.center.y, boxCollider.bounds.center.z),
                new Vector3(attackAreaWidth, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
                0, Vector2.right, 0, enemyLayer);

            if (hit.collider != null && doDamage)
            {
                //Debug.Log("Something");
                if (hit.collider.tag == "Enemy") {
                    //enemyHealth = hit.transform.GetComponent<EnemyHealth>();
                    //enemyHealth.reduceHealth(mainAttackDamage);
                    //Debug.Log("Enemy");
                    doDamage = false;
                } else
                {
                    doDamage = false;
                }
            } else
            {
                doDamage = false;
            }
        }
    }

    public void attackFinished() { //Usato in animazione
        canAttack = true;
        mainAttack = false;
        secondaryAttack = false;
        bladeDown = false;
    }

    private void bladeDownKnockback() //Usato in animazione
    {
        bladeDown = true;
    }

    private void doDamageMethod()
    {
        doDamage = true;
    }

    //Metodi setter
    public void setMainAttack(bool mainAttack) { this.mainAttack = mainAttack; }

    //Metodi getter
    public bool getMainAttack() { return mainAttack; }
    public float getmainAttackDamage() { return mainAttackDamage; }
    public float getMainKnockback() { return mainKnockback; }
    public float getSuperAttackDamage() { return superAttackDamage; }
    public bool getCanAttack() { return canAttack; }

    //Metodi reducer
    public void reduceMainAttackDamage(float reduceMainAttackDamage)
    {
        mainAttackDamage -= reduceMainAttackDamage;
    }
    public void reduceMainKnockback(float reduceKnockback)
    {
        mainKnockback -= reduceKnockback;
    }
    public void reduceSuperAttackDamage(float reduceSuperAttackDamage)
    {
        mainAttackDamage -= reduceSuperAttackDamage;
    }
}
