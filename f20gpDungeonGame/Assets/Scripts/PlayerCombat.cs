using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator;

    [Header("Attack Settings")]
    public float attack1Duration = 0.67f;
    public float attack2Duration = 0.75f;

    [Header("Block Settings")]
    public float blockAttackLockout = 0.2f;

    private float attackCooldownTimer = 0f;
    private float blockCooldownTimer = 0f;

    private bool isAttacking = false;
    private bool isInAttack2 = false;
    private bool queuedAttack2 = false;
    private bool isBlocking = false;

    private WeaponHitbox weaponHitbox;
    private Health health;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        weaponHitbox = GetComponentInChildren<WeaponHitbox>();
        health = GetComponent<Health>();
    }

    void Update()
    {
        HandleBlockInput();
        HandleAttackInput();

        UpdateAttackTimer();
        UpdateBlockCooldown();
    }

    void HandleBlockInput()
    {
        if (Input.GetMouseButton(1) && !isAttacking)
        {
            if (!isBlocking)
            {
                isBlocking = true;
                animator.SetBool("isBlock", true);
                if (health != null) health.isBlocking = true;
                Debug.Log("Started Blocking");
            }
        }
        else if (isBlocking)
        {
            isBlocking = false;
            blockCooldownTimer = blockAttackLockout;
            animator.SetBool("isBlock", false);
            if (health != null) health.isBlocking = false;
            Debug.Log("Stopped Blocking, starting lockout");
        }
    }

    void HandleAttackInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isBlocking || blockCooldownTimer > 0f)
            {
                Debug.Log("Attack blocked due to active block or cooldown");
                return;
            }

            if (!isAttacking && attackCooldownTimer <= 0f)
            {
                TriggerAttack1();
            }
            else if (isAttacking && !isInAttack2)
            {
                queuedAttack2 = true;
                Debug.Log("Attack2 queued");
            }
        }
    }

    void UpdateAttackTimer()
    {
        if (!isAttacking) return;

        attackCooldownTimer -= Time.deltaTime;

        if (attackCooldownTimer <= 0f)
        {
            if (queuedAttack2)
            {
                TriggerAttack2();
            }
            else
            {
                ResetAttackState();
            }
        }
    }

    void UpdateBlockCooldown()
    {
        if (blockCooldownTimer > 0f)
        {
            blockCooldownTimer -= Time.deltaTime;
        }
    }

    void TriggerAttack1()
    {
        animator.SetTrigger("Attack1Trigger");
        isAttacking = true;
        isInAttack2 = false;
        queuedAttack2 = false;
        attackCooldownTimer = attack1Duration;

        if (weaponHitbox != null) weaponHitbox.EnableHitbox();

        Debug.Log("Attack1 triggered");
    }

    void TriggerAttack2()
    {
        animator.SetTrigger("Attack2Trigger");
        isInAttack2 = true;
        queuedAttack2 = false;
        attackCooldownTimer = attack2Duration;

        if (weaponHitbox != null) weaponHitbox.EnableHitbox();

        Debug.Log("Attack2 triggered");
    }

    void ResetAttackState()
    {
        isAttacking = false;
        isInAttack2 = false;
        queuedAttack2 = false;
        attackCooldownTimer = 0f;

        if (weaponHitbox != null) weaponHitbox.DisableHitbox();

        Debug.Log("Attack reset");
    }
}
