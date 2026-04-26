using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerControl : MonoBehaviour
{

    private Rigidbody player;
    private float movementX;
    private float movementY;
    public float speed = 100;

    public float dashForce = 2000f;
    private bool dashRequest = false;

    void Start()
    {
        player = GetComponent<Rigidbody>();
    }
    private void OnDash(InputValue value)
    {
        if (value.isPressed)
        {
            dashRequest = true;
            print("Dash");
        }
    }

    private void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementY = -movementVector.x;
        movementX = movementVector.y;
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        player.AddForce(movement * speed);

        player.AddForce(movement * speed);

        if (dashRequest)
        {
            Vector3 dashDirection = new Vector3(movementX, 0.0f, movementY).normalized;

            if (dashDirection.magnitude > 0)
            {
                player.AddForce(dashDirection * dashForce, ForceMode.Impulse);
            }

            dashRequest = false;
        }
    }
}