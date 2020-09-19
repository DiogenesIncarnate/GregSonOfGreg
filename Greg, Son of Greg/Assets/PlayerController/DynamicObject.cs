using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Controller2D))]
public class DynamicObject : MonoBehaviour
{
    // Object stats
    public float maxHP;
    public float currentHP;

    // Object physics data
    protected float gravity;
    protected Vector3 velocity;
    protected float velocityXSmoothing;

    protected Vector2 directionalInput;

    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    public float timeToJumpApex = .6f;

    protected float accelerationTimeAirborne = .2f;
    protected float accelerationTimeGrounded = .1f;
    protected float moveSpeed = 4;

    // Object animation/collision references
    public Animator animator;
    protected Controller2D controller;


    protected virtual void Start()
    {
        currentHP = maxHP;

        controller = GetComponent<Controller2D>();

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
    }

    protected virtual void Update()
    {
        CalculateVelocity();

        controller.Move(velocity * Time.deltaTime, directionalInput);

        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }
    }
    protected virtual void CalculateVelocity()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
    }

    public virtual void SetDirectionalInput(Vector2 input)
    {
        if (input.x != 0 && Mathf.Sign(input.x) != Mathf.Sign(transform.localScale.x))
        {
            FlipSprite();
        }

        directionalInput = input;

        animator.SetFloat("DirectionalInputX", Mathf.Abs(directionalInput.x));
    }

    protected virtual void FlipSprite()
    {
        Vector3 tempScale = transform.localScale;
        tempScale.x *= -1;
        transform.localScale = tempScale;
    }
}
