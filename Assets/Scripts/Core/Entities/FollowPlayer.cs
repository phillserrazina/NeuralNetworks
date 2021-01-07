using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] [Range(0f, 30f)] private float speed;
    private Vector3 initialPosition;

    private float restartTimer = 7f;

    private void Awake() {
        initialPosition = transform.position;
    }

    private void Update() {
        transform.position = Vector3.MoveTowards(transform.position, player.position, Time.deltaTime * speed);
        
        if (restartTimer > 0) {
            restartTimer -= Time.deltaTime;
            return;
        }

        transform.position = initialPosition;
        restartTimer = 7f;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            transform.position = initialPosition;
        }
    }
}
