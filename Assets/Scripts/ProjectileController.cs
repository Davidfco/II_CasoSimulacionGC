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

    private void OnEnable()
    {
        _rigidbody.velocity = Vector2.right * speed * Time.deltaTime;    
    }

    public void Go(float damage, bool isPercentage)
    {
        _damage = damage;
        _isPercentage = isPercentage; 
        gameObject.SetActive(true);
    }
    
}
