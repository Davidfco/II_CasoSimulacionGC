using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour
{
    [SerializeField]
    float chargeTimeout;

    private CharacterController2D _character;

    private float _chargeTime;
    private bool _isCharging;

    private void Awake()
    {
        _character = GetComponent<CharacterController2D>();    
    }

    private void Update()
    {
        if (Input.GetButtonUp("Fire2"))
        {
            if (_chargeTime >= chargeTimeout)
            {
                _character.Shot();
            }
            _isCharging = false;
            _chargeTime = 0.0F;
        }

        if (Input.GetButton("Fire2"))
        {
            if (!_isCharging)
            {
                _isCharging = true;
                _chargeTime = 0.0F;
            }

            else if (_isCharging)
            {
                _chargeTime += Time.deltaTime;
            }
        }
    }
}
