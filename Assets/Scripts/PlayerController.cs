using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    #region movement_variables
    public float movespeed;
    float x_input;
    float y_input;
    #endregion

    #region attack_variables
    public GameObject bulletObj;
    public float damage;
    public float attackSpeed;
    float attackTimer;
    public float hitboxTiming;
    public float endAnimationTiming;
    bool isAttacking;
    Vector2 currDirection;
    #endregion

    #region physics_components
    Rigidbody2D playerRB;
    #endregion

    #region animation_components
    Animator anim;
    #endregion

    #region health_variables
    public float maxHealth;
    float currHealth;
    public Slider hpSlider;
    #endregion

    #region Unity_functions
    private void Awake() {
        playerRB = GetComponent<Rigidbody2D>();

        anim = GetComponent<Animator>();

        currHealth = maxHealth;
        hpSlider.value = currHealth / maxHealth;

        attackTimer = 0;
    }

    private void Update() {
        if (isAttacking) {
            return;
        }

        x_input = Input.GetAxisRaw("Horizontal");
        y_input = Input.GetAxisRaw("Vertical");

        Move();

        if (Input.GetKeyDown(KeyCode.J) && attackTimer <= 0) {
            Attack();
        } else {
            attackTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.L)) {
            Interact();
        }

        if (Input.GetKeyDown(KeyCode.Space) && attackTimer <= 0) {
            Shoot();
        }
    }
    #endregion

    #region movement_functions
    private void Move() {
        anim.SetBool("Moving", true);

        if (x_input > 0) {
            playerRB.velocity = Vector2.right * movespeed;
            currDirection = Vector2.right;
        } else if (x_input < 0) {
            playerRB.velocity = Vector2.left * movespeed;
            currDirection = Vector2.left;
        } else if (y_input > 0) {
            playerRB.velocity = Vector2.up * movespeed;
            currDirection = Vector2.up;
        } else if (y_input < 0) {
            playerRB.velocity = Vector2.down * movespeed;
            currDirection = Vector2.down;
        } else {
            playerRB.velocity = Vector2.zero;
            anim.SetBool("Moving", false);
        }

        anim.SetFloat("DirX", currDirection.x);
        anim.SetFloat("DirY", currDirection.y);
    }
    #endregion

    #region attack_functions
    private void Attack() {
        StartCoroutine(AttackRoutine());
        attackTimer = attackSpeed;
    }

    IEnumerator AttackRoutine() {
        isAttacking = true;
        playerRB.velocity = Vector2.zero;

        anim.SetTrigger("Attack");

        FindObjectOfType<AudioManager>().Play("PlayerAttack");

        yield return new WaitForSeconds(hitboxTiming);

        RaycastHit2D[] hits = Physics2D.BoxCastAll(playerRB.position + currDirection, Vector2.one, 0f, Vector2.zero, 0);
        foreach(RaycastHit2D hit in hits) {
            if (hit.transform.CompareTag("Enemy")) {
                hit.transform.GetComponent<Enemy>().TakeDamage(damage);
            }
        }

        yield return new WaitForSeconds(endAnimationTiming);

        isAttacking = false;
    }

    private void Shoot() {
        playerRB.velocity = Vector2.zero;
        attackTimer = attackSpeed;
        GameObject bullet = Instantiate(bulletObj, transform.position, transform.rotation);
        bullet.GetComponent<Bullet>().direction = currDirection;
        Debug.Log("shot");
    }
    #endregion

    #region health_functions
    public void TakeDamage(float value) {
        currHealth -= value;

        hpSlider.value = currHealth / maxHealth;

        FindObjectOfType<AudioManager>().Play("PlayerHurt");

        if (currHealth <= 0) {
            Die();
        }
    }

    public void Heal(float value) {
        currHealth += value;
        currHealth = Mathf.Min(currHealth, maxHealth);

        hpSlider.value = currHealth / maxHealth;
    }

    private void Die() {
        FindObjectOfType<AudioManager>().Play("PlayerDeath");

        GameObject gm = GameObject.FindWithTag("GameController");
        gm.GetComponent<GameManager>().LoseGame();

        Destroy(gameObject);
    }
    #endregion

    #region interact_function
    void Interact() {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(playerRB.position + currDirection, new Vector2(.5f, .5f), 0f, Vector2.zero, 0);
        foreach(RaycastHit2D hit in hits) {
            if (hit.transform.CompareTag("Chest")) {
                hit.transform.GetComponent<Chest>().Interact();
            }
        }
    }
    #endregion
}
