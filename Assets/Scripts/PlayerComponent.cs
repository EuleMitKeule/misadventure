using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

public class PlayerComponent : MonoBehaviour
{
    [Header("Physics")]
    [SerializeField] float speed;
    [SerializeField] float moveDamping;
    [Header("Input")]
    [SerializeField] InputAction walkAction;

    Vector2 InputValue { get; set; }

    Rigidbody2D Rigidbody { get; set; }

    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        walkAction.Enable();
    }

    void Update()
    {
        ProcessInput();
    }

    void FixedUpdate()
    {
        Rigidbody.velocity = speed * InputValue;
    }

    void ProcessInput()
    {
        var inputValue = walkAction.ReadValue<Vector2>();
        InputValue = Vector2.MoveTowards(InputValue, inputValue, 1 / moveDamping);
    }
}
