using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPotion : MonoBehaviour
{
    #region functions
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.transform.CompareTag("Player")) {
            collision.transform.GetComponent<PlayerController>().movespeed *= 2;
            Destroy(gameObject);
        }
    }
    #endregion
}
