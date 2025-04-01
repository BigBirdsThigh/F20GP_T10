using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Animator animator;
    public float comboWindowStart = 0.6f;
    public float comboWindowEnd = 0.95f;

    public float attackDelay = 0.2f;
    private float attackCooldown = 0f;

    private bool isAttacking = false;
    private bool inComboWindow = false;
    private bool queuedAttack = false;
    private int currentAttack = 0;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleBlocking();
        HandleAttacking();
        attackCooldown -= Time.deltaTime;
    }

    void HandleBlocking()
    {
        bool blocking = Input.GetMouseButton(1) && !isAttacking;
        animator.SetBool("isBlock", blocking);
    }

    void HandleAttacking()
    {
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        // ✅ While in Attack1
        if (state.IsName("Attack1"))
        {
            isAttacking = true;
            animator.SetBool("isAttacking", true);

            float t = state.normalizedTime;
            inComboWindow = t >= comboWindowStart && t <= comboWindowEnd;

            if (inComboWindow && queuedAttack)
            {
                animator.SetTrigger("Attack2Trigger");
                queuedAttack = false;
                currentAttack = 2;
            }

            if (t > 1f && !queuedAttack)
                ResetAttack();
        }

        // ✅ While in Attack2
        else if (state.IsName("Attack2"))
        {
            isAttacking = true;
            animator.SetBool("isAttacking", true);

            if (state.normalizedTime > 1f)
                ResetAttack();
        }

        // ✅ Not in any attack state
        else if (!queuedAttack)
        {
            isAttacking = false;
            animator.SetBool("isAttacking", false);
            currentAttack = 0;
        }

        // ✅ Input: Mouse 1
        if (Input.GetMouseButtonDown(0))
        {
            if (!isAttacking && attackCooldown <= 0f)
            {
                animator.SetTrigger("Attack1Trigger");
                animator.SetBool("isAttacking", true);
                isAttacking = true;
                currentAttack = 1;
                attackCooldown = attackDelay;
            }
            else if (currentAttack == 1 && inComboWindow)
            {
                queuedAttack = true;
            }
        }
    }

    void ResetAttack()
    {
        isAttacking = false;
        queuedAttack = false;
        inComboWindow = false;
        currentAttack = 0;
        attackCooldown = attackDelay;

        animator.SetBool("isAttacking", false);
    }
}
