using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeController : MonoBehaviour
{
    [SerializeField]
    float maxTime;

    [SerializeField]
    Image timer;

    private CharacterController2D _player;

    private float _currentTime;

    private void Awake()
    {
        _player = FindObjectOfType<CharacterController2D>();
    }
    private void Start()
    {
        _currentTime = maxTime;
    }

    private void Update()
    {
        _currentTime -= Time.deltaTime;
        if (_currentTime <= 0.0F)
        {
            _player.Die();
            enabled = false;
            return;
        }
        timer.fillAmount = _currentTime / maxTime;
    }
}
