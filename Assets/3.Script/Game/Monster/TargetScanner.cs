using UnityEngine;
using UnityEngine.AI;

public class TargetScanner : MonoBehaviour
{

    public PlayerController player;           // �÷��̾��� ��ġ�� �޾ƿ� ����
    public float detectionRange = 10f; // ���Ͱ� �÷��̾ �ν��� ����
    public float moveSpeed = 3.5f;     // ���� �̵� �ӵ�

    private NavMeshAgent agent;        // ������ NavMeshAgent
    private bool isChasing = false;    // ���Ͱ� �÷��̾ ���� ������ Ȯ���ϴ� ����

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
        }
        else
        {
            // �÷��̾ ���� ���� �ۿ� ���� ��
            StopChasingPlayer();
        }
    }
    private void LateUpdate()
    {
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    void ChasePlayer()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsTag("Die")) return;

        isChasing = true;
        agent.isStopped = false;       // NavMeshAgent�� �̵��� �� �ֵ��� ����
        agent.SetDestination(player.transform.position); // �÷��̾��� ��ġ�� �̵�
    }

    void StopChasingPlayer()
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
