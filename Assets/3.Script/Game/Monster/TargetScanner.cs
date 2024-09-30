using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class TargetScanner : MonoBehaviour
{

    public PlayerController player;           // �÷��̾��� ��ġ�� �޾ƿ� ����
    public float detectionRange = 10f; // ���Ͱ� �÷��̾ �ν��� ����
    public float moveSpeed = 3.5f;     // ���� �̵� �ӵ�

    private NavMeshAgent agent;        // ������ NavMeshAgent
    private bool isChasing = false;    // ���Ͱ� �÷��̾ ���� ������ Ȯ���ϴ� ����

    [Header("Attack Settings")] 
    public float attackRange = 1f;          // ���� ���� ����
    public float minAttackInterval = 1.5f;    // �ּ� ���� ����
    public float maxAttackInterval = 3f;    // �ִ� ���� ����
    public int damage = 10;                 // ���� ������

    private bool isAttacking = false;       // ���� ����

    private Animator animator;

    void Start()
    {
        TryGetComponent(out animator);
        agent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<PlayerController>();
        agent.speed = moveSpeed;      // ������ �̵� �ӵ� ����
    }

    void Update()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);

        if (distance <= detectionRange)
        {
            // �÷��̾ ���� ���� ���� ���� ��
            ChasePlayer();

            if (distance <= attackRange && !isAttacking)
            {
                // ���� ���� ���� �ְ� ���� ���� �ƴ� ��
                StartCoroutine(AttackRoutine());
            }
        }
        else
        {
            // �÷��̾ ���� ���� �ۿ� ���� ��
            StopChasingPlayer();
        }
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
                //switch (Random.Range(0, 2))
                //{
                //    case 0:
                //        animator.SetTrigger("Punch");
                //        sec = 0.4f;
                //        break;
                //    case 1:
                //        animator.SetTrigger("Swiping");
                //        sec = 1.5f;
                //        break;
                //}
            }

            // ������ ���� (��: �÷��̾� ��ũ��Ʈ�� �������� ������ �޼��� ȣ��)
            PlayerStatus status = player.GetComponent<PlayerStatus>();
            if (status != null)
            {
                status.TakeDamage(damage, sec);
            }

            // �α� ��� (����� �뵵)
            Debug.Log("���Ͱ� �÷��̾ �����߽��ϴ�!");

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
        }
    }


#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        // Gizmos�� ���� ���� (��: ������)
        Gizmos.color = Color.red;

        // ������ ���� ������ ��ü�� �׸���
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

#endif

}
