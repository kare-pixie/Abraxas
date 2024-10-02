using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class TargetScanner : MonoBehaviour
{

    public PlayerController player;           // �÷��̾��� ��ġ�� �޾ƿ� ����
    public float detectionRange = 10f; // ���Ͱ� �÷��̾ �ν��� ����
    public float moveSpeed = 3.5f;     // ���� �̵� �ӵ�
    public float fieldOfView = 45f;    // ������ �þ߰� (45�� ����)

    private NavMeshAgent agent;        // ������ NavMeshAgent
    private bool isChasing = false;    // ���Ͱ� �÷��̾ ���� ������ Ȯ���ϴ� ����

    [Header("Attack Settings")] 
    public float attackRange = 2f;          // ���� ���� ����
    public float minAttackInterval = 1.5f;    // �ּ� ���� ����
    public float maxAttackInterval = 3f;    // �ִ� ���� ����
    public int damage = 10;                 // ���� ������

    private bool isAttacking = false;       // ���� ����
    private Animator animator;
    public LayerMask obstacleLayer;    // ��ֹ� ���̾�

    private Coroutine attackCoroutine;

    void Start()
    {
        TryGetComponent(out animator);
        agent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<PlayerController>();
        agent.speed = moveSpeed;      // ������ �̵� �ӵ� ����
    }

    void Update()
    {
        if (UIManager.instance != null && UIManager.instance.isGameOver)
        {
            StopChasingPlayer();
            return;
        }

        float distance = Vector3.Distance(player.transform.position, transform.position);

        // �÷��̾ �þ߰� ���� �ְ� �þ߿� ������ �� ������ ����
        if (IsPlayerInFieldOfView() && IsPlayerVisible())
        {
            isChasing = true;  // ���� ����
        }

        // ���� ���� ��
        if (isChasing)
        {

            // ���� ���� �ȿ� ���� �� ��� ����
            if (distance <= detectionRange)
            {
                ChasePlayer();

                // ���� ���� ���� ������ ����
                if (distance <= attackRange && !isAttacking)
                {
                    if (attackCoroutine != null)
                        StopCoroutine(attackCoroutine);
                    attackCoroutine = StartCoroutine(AttackRoutine());
                }
            }
            else
            {
                // ���� ������ ����� ���� ����
                StopChasingPlayer();
            }
        }
    }

    private bool IsPlayerVisible()
    {
        // �÷��̾���� ���� ���
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // Raycast�� ����Ͽ� �÷��̾���� �þ߿� ��ֹ��� �ִ��� Ȯ��
        if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleLayer))
        {
            return true;  // ��ֹ��� ������ �÷��̾ �þ߿� �ִ� ��
        }
        return false;      // ��ֹ��� ������ �÷��̾ �þ߿� ����
    }

    private bool IsPlayerInFieldOfView()
    {
        // �÷��̾���� ���� ����
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;

        // ���Ͱ� ���� �ִ� ���� (����)
        Vector3 forward = transform.forward;

        // ���� ����� �÷��̾� ���� ������ ���� ���
        float angleToPlayer = Vector3.Angle(forward, directionToPlayer);

        // ������ �þ߰����� ������ �þ߿� �ִ� ��
        return angleToPlayer < fieldOfView / 2f;
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        float sec = 0.4f;

        while (Vector3.Distance(player.transform.position, transform.position) <= attackRange)
        {
            // �÷��̾ ���� ȸ��
            Vector3 direction = (player.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

            // ���� �ִϸ��̼� ��� (�ִϸ����Ͱ� �ִ� ���)
            Animator animator = GetComponent<Animator>();
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsTag("BlockMovement")) //
                break;

            if (animator != null)
            {
                animator.SetTrigger("Punch");
                sec = 0.4f;
            }

            // ������ ���� (��: �÷��̾� ��ũ��Ʈ�� �������� ������ �޼��� ȣ��)
            PlayerStatus status = player.GetComponent<PlayerStatus>();
            if (status != null)
            {
                status.TakeDamage(damage, sec);
            }

            // ���� ���� ���
            float attackInterval = Random.Range(minAttackInterval, maxAttackInterval);
            yield return new WaitForSeconds(attackInterval);
        }

        isAttacking = false;
    }
    private void LateUpdate()
    {
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    private void ChasePlayer()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsTag("Die")) return;

        isChasing = true;
        agent.isStopped = false;       // NavMeshAgent�� �̵��� �� �ֵ��� ����
        agent.SetDestination(player.transform.position); // �÷��̾��� ��ġ�� �̵�
    }

    private void StopChasingPlayer()
    {
        if (isChasing)
        {
            isChasing = false;
            agent.isStopped = true;    // ���� ����

            if (attackCoroutine != null)
                StopCoroutine(attackCoroutine);
        }
    }


#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        // Gizmos�� ���� ����
        Gizmos.color = Color.red;

        // ������ ���� ����
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // �þ߰� ǥ��
        Gizmos.color = Color.yellow;
        Vector3 forward = transform.forward * detectionRange;
        Vector3 leftBoundary = Quaternion.Euler(0, -fieldOfView / 2, 0) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, fieldOfView / 2, 0) * forward;

        Gizmos.DrawRay(transform.position, leftBoundary);
        Gizmos.DrawRay(transform.position, rightBoundary);
    }

#endif

}
