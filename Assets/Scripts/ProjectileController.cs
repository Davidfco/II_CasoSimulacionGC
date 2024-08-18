using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{

    [SerializeField]
    float speed;

    private Rigidbody2D _rigidbody;

    private float _damage;
    private bool _isPercentage;


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();    
    }

    public void Go(float damage, bool isPercentage, Vector2 direction)
    {
        _damage = damage;
        _isPercentage = isPercentage; 
        gameObject.SetActive(true);

        _rigidbody.velocity = direction * speed * Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Zombie"))
        {
            EnemyController2D Zombie = collision.gameObject.GetComponent<EnemyController2D>();
            if (Zombie != null)
            {
                Zombie.TakeDamage(_damage);
            }
            Destroy(gameObject);
        }
    }
}
