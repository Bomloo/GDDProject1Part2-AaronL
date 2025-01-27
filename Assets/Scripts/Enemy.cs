﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    #region movement_variables
    public float movespeed;
    #endregion

    #region physics_components
    Rigidbody2D enemyRB;
    #endregion

    #region health_variables
    public float maxHealth;
    float currHealth;
    #endregion

    #region targeting_variables
    public Transform player;
    #endregion

    #region attack_variables
    public float explosionDamage;
    public float explosionRadius;
    public GameObject explosionObj;
    #endregion

    #region Unity_functions
    private void Awake() {
        enemyRB = GetComponent<Rigidbody2D>();

        currHealth = maxHealth;
    }

    private void Update() {
        if (player == null)
            return;

        Move();
    }
    #endregion

    #region movement_functions
    private void Move() {
        Vector2 direction = player.position - transform.position;

        enemyRB.velocity = direction.normalized * movespeed;
    }
    #endregion

    #region attack_functions
    private void Explode() {
        FindObjectOfType<AudioManager>().Play("Explosion");

        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, explosionRadius, Vector2.zero);
        foreach (RaycastHit2D hit in hits) {
            if (hit.transform.CompareTag("Player")) {
                hit.transform.GetComponent<PlayerController>().TakeDamage(explosionDamage);

                Instantiate(explosionObj, transform.position, transform.rotation);
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.transform.CompareTag("Player")) {
            Explode();
        }
    }
    #endregion

    #region health_functions
    public void TakeDamage(float value) {
        FindObjectOfType<AudioManager>().Play("BatHurt");

        currHealth -= value;

        if (currHealth <= 0) {
            Die();
        }
    }

    private void Die() {
        Destroy(gameObject);
    }
    #endregion
}
