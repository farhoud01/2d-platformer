using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wallJumpBehavior : MonoBehaviour
{
    input_manager input;
    Rigidbody2D rb;
    movementBehavior movementRef;

    void Start()
    {
        input = input_manager.Instance;
        movementRef = GetComponent<movementBehavior>();
        rb = GetComponent<Rigidbody2D>();
    }

    
    void Update()
    {
    
    }

  

}
