using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Controller2D))]
public class DynamicCharacter : MonoBehaviour
{
    // Object stats
    [Header("Character Settings")]
    public float maxHP;
    public int might = 10;
    public int charm = 10;
    public int wit = 10;

    [HideInInspector]
    public float currentHP;
    protected bool isDead = false;

    // Object physics data
    [HideInInspector]
    protected float gravity;
    protected float velocityXSmoothing;
    protected Vector2 directionalInput;
    protected float accelerationTimeAirborne;
    protected float accelerationTimeGrounded;
    protected float nextAttackTime = 0;
    protected Vector3 velocity;
    protected float maxJumpHeight;
    protected float minJumpHeight;
    protected float timeToJumpApex;
    protected float timeToClimb = 0.5f;
    protected float climbTimer = 0f;
    protected float moveSpeed;
    protected float attackRate;
    protected float critChance;

    // Object animation/collision references
    public Animator animator;
    protected Controller2D controller;
    public Transform attackPoint;
    public float attackRange;
    public float attackDelay = 0;
    public LayerMask hittableLayers;

    // UI Data
    public TextMeshProUGUI currentHPDisplay;


    protected virtual void Start()
    {
        SetPhysicsFromAbilities();
        currentHP = maxHP;
        UpdateUI();

        controller = GetComponent<Controller2D>();

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
    }

    protected virtual void Update()
    {

            if (climbTimer >= timeToClimb)
            {
                controller.FinishLedgeClimb();
                climbTimer = 0;
            }
            else
            {
                climbTimer += Time.deltaTime;
            }

        CalculateVelocity();

        if (!controller.canClimbLedge)
        {
            controller.Move(velocity * Time.deltaTime, directionalInput);
        }

        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

        animator.SetBool("CollisionBelow", controller.collisions.below);

        HandleAnimation();
    }
    protected virtual void CalculateVelocity()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
    }

    public void SetDirectionalInput(Vector2 input)
    {
        GetComponent<SpriteRenderer>().flipX = (input.x > 0) ? false : (input.x < 0) ? true : GetComponent<SpriteRenderer>().flipX;
        foreach(Transform t in GetComponentsInChildren<Transform>())
        {
            if (t.gameObject.CompareTag("Hitbox"))
            {
                Vector3 tPNew = t.localPosition;
                tPNew.x = (input.x > 0) ? Mathf.Abs(tPNew.x) : (input.x < 0) ? Mathf.Abs(tPNew.x) * -1 : tPNew.x;
                t.localPosition = tPNew;
            }
        }

        directionalInput = input;

        if (controller.collisions.below)
        {
            animator.SetFloat("DirectionalInputX", Mathf.Abs(directionalInput.x));
        }
        else
            animator.SetFloat("DirectionalInputX", 0);
    }

    public void OnAttackInput()
    {
        if (Time.time >= nextAttackTime && attackDelay <= 0)
        {
            Attack();
            nextAttackTime = Time.time + 1f / attackRate;
        }
        else if(attackDelay > 0)
        {
            attackDelay -= Time.deltaTime;
        }
    }

    protected void Attack()
    {
        animator.SetTrigger("Attack");

        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, hittableLayers);

        foreach (Collider2D obj in hitObjects)
        {
            Debug.Log("We hit " + obj.name);

            DynamicCharacter source = obj.gameObject.GetComponent<DynamicCharacter>();

            if(source)
            {
                obj.gameObject.GetComponent<DynamicCharacter>().Hit(source);
            }
        }
    }

    public void Hit(DynamicCharacter source)
    {
        Vector2 hitDir = new Vector2(Mathf.Sign(transform.position.x - source.transform.position.x) * 5.0f, 5.0f);
        float damage = 5 + ((source.might - 10) / 2);
        if(Random.Range(1, 100) > source.critChance)
        {
            damage *= 1.2f;
        }

        Damage(damage);
        velocity += new Vector3(hitDir.x, hitDir.y, 0);
        animator.SetTrigger("Hit");
    }

    void Damage(float dmg)
    {
        currentHP -= dmg;
        if (currentHP <= 0)
        {
            isDead = true;
            animator.SetBool("IsDead", isDead);
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }

        UpdateUI();
    }

    protected void HandleAnimation()
    {
        animator.SetFloat("VelocityY", velocity.y);
    }

    protected virtual void SetPhysicsFromAbilities()
    {
        maxJumpHeight = 0.25f * might;
        minJumpHeight = 1;
        timeToJumpApex = .5f;

        accelerationTimeAirborne = .2f;
        accelerationTimeGrounded = 1f / might;
        moveSpeed = 0.2f * wit;

        attackRate = 0.2f * charm;
        attackRange = 0.05f * wit;
    }

    protected void UpdateUI()
    {
        if (currentHPDisplay)
        {
            currentHPDisplay.text = ((int)(currentHP / maxHP * 100f)).ToString() + "%";
        }
    }
}