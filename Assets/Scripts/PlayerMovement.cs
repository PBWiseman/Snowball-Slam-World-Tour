using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private const float UPDATEDELAY = 0.05f; //The delay between direction facing updates
    private const double DOUBLETAPDELAY = 0.5; //The amount of delay allowed between taps
    private const int MOVEMENTSPEED = 7; //Placeholder values. Will change after testing

    private const int SLIDINGSPEED = 20; //Placeholder values. Will change after testing
    private int slowingDownSpeed = 0; //The speed the player is at during the slowing down process

    public int CurrentSpeed //Returns the movement speed, sliding speed, or 0 depending on the player's state
    {
        get
        {
            if (IsSlowingDown)
            {
                //Slowing down speed
                return slowingDownSpeed;
            }
            if (IsSliding)
            {
                //Sliding speed
                return SLIDINGSPEED;
            }
            if (IsMoving)
            {
                //Moving Speed
                return MOVEMENTSPEED;
            }
            else
            {
                //Idle Speed
                return 0;
            }
        }
    }

    private bool isMoving = false;
    public bool IsMoving //Property to set the IsMoving bool and update the animator
    {
        get
        {
            return isMoving;
        }
        private set
        {
            isMoving = value;
            animator.SetBool("IsMoving", value);
            if (!value) //If the player is not moving then they are not sliding either
            {
                IsSliding = value;
            }
        }
    }
    
    private bool isSliding = false;
    public bool IsSliding //Property to set the IsSliding bool and update the animator
    {
        get
        {
            return isSliding;
        }
        private set
        {
            if (value && slowingDown) //If it gets set to true and the player was in the process of slowing down
            {
                slowingDown = false;
            }
            if (!value && !slowingDown && isSliding) //If it gets set to false and the player was not slowing down and was sliding
            {
                IsSlowingDown = true; //The player gets set to slowing down
            }
            else
            {
                isSliding = value;
                animator.SetBool("IsSliding", value);
            }
        }
    }
    private bool isSlowingDown = false;

    public bool IsSlowingDown //Property when slowing down. This will slow the player down rapidly and then set the player to not sliding
    {
        get
        {
            return isSlowingDown;
        }
        private set
        {
            isSlowingDown = value;
            if (value && !slowingDown) //If the player need to slow down and is not already slowing down
            {
                StartCoroutine(SlowDown());
            }
        }
    }

    private Rigidbody rb; //The player's Rigidbody component
    private Animator animator; //The player's Animator component
    private Vector2 moveInput; //The player's vector 2 move input
    private Vector2 lastInput; //The previous input
    private Vector2 secondLastInput; //The input before the last input
    private double lastTime; //The time of the last input
    private double secondLastTime; //The time of the input before the last input
    private bool slowingDown = false; //If the coroutine for slowing the player down is running
    private Vector3 moveDirection; //Move input converted to a Vector3
    private float lastUpdate; //The time since the last direction update
    private IEnumerator coroutine; //The coroutine for slowing down

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        if (rb == null)
        {
            Debug.LogWarning("No Rigidbody component found.");
        }
        if (animator == null)
        {
            Debug.LogWarning("No Animator component found.");
        }
        lastInput = Vector2.zero;
    }

    void FixedUpdate()
    {
        if (slowingDown) //If the player is slowing down from sliding
        {
            //If the second to last input was diagonal and the last input was straight and the time between the two inputs is very short
            //then the user likely did not mean to go straight and they were just releasing the two keys
            if ((secondLastInput.x != 0 && secondLastInput.y != 0) //If the second to last input was diagonal
                && (lastInput.x == 0 || lastInput.y == 0) //If the last input was straight
                && secondLastTime - lastTime < 0.1) //If the time between the two inputs is very short
            {
                //The player is moved along the second to last move direction instead of the last move direction
                moveDirection = new Vector3(secondLastInput.x, 0, secondLastInput.y);
            }
            else
            {
                //The player is moved along the last move direction instead of the current (0,0) move direction
                moveDirection = new Vector3(lastInput.x, 0, lastInput.y);
            }
        }
        rb.velocity = new Vector3(moveDirection.x * CurrentSpeed, 0, moveDirection.z * CurrentSpeed); //Move the player based on the move direction and speed
        lastUpdate += Time.fixedDeltaTime;
        if (lastUpdate >= UPDATEDELAY //I have added a delay between changing the facing direction otherwise it is almost impossible to end up facing diagonally
            && moveDirection != transform.forward //If the player is not already facing the move direction
            && moveDirection != Vector3.zero) //If the player is not standing still
        {
            transform.forward = moveDirection;
            lastUpdate = 0;
        }
    }

    private IEnumerator SlowDown()
    {
        slowingDown = true;
        slowingDownSpeed = SLIDINGSPEED;
        while (slowingDownSpeed > MOVEMENTSPEED && IsSliding && slowingDown)
        {
            slowingDownSpeed--;
            yield return new WaitForFixedUpdate();
        }
        IsSliding = false;
        IsSlowingDown = false;
        slowingDown = false;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        IsMoving = moveInput != Vector2.zero; //If the move input is not zero then the player is moving
        //If there was an action performed
        if (context.performed)
        {
            //If the time between the last input and the current input is less than the double tap delay
            //And the move input is the same as the last input
            if (context.time - lastTime < DOUBLETAPDELAY && moveInput == lastInput)
            {
                IsSliding = true;
            }
            //If the player is slowing down and the move input is the exact opposite of the last input
            else if (slowingDown && moveInput == -lastInput)
            {
                IsSliding = true;
            }
            secondLastInput = lastInput;
            secondLastTime = lastTime;
            lastInput = moveInput;
            lastTime = context.time;
        }
    }
}
