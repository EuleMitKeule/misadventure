using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

public class PlayerComponent : MonoBehaviour
{
    [Header("Physics")]
    [SerializeField] float speed;
    [Header("Input")]
    [SerializeField] InputAction walkAction;

    Rigidbody2D Rigidbody { get; set; }

    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        
    }

    void Update()
    {
    }
}
