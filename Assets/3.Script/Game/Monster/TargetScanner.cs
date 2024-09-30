using UnityEngine;
using UnityEngine.AI;

public class TargetScanner : MonoBehaviour
{

    public PlayerController player;           // 플레이어의 위치를 받아올 변수
    public float detectionRange = 10f; // 몬스터가 플레이어를 인식할 범위
    public float moveSpeed = 3.5f;     // 몬스터 이동 속도

    private NavMeshAgent agent;        // 몬스터의 NavMeshAgent
    private bool isChasing = false;    // 몬스터가 플레이어를 추적 중인지 확인하는 변수

    private Animator animator;

    void Start()
    {
        TryGetComponent(out animator);
        agent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<PlayerController>();
        agent.speed = moveSpeed;      // 몬스터의 이동 속도 설정
    }

    void Update()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);

        if (distance <= detectionRange)
        {
            // 플레이어가 감지 범위 내에 있을 때
            ChasePlayer();
        }
        else
        {
            // 플레이어가 감지 범위 밖에 있을 때
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
        agent.isStopped = false;       // NavMeshAgent가 이동할 수 있도록 설정
        agent.SetDestination(player.transform.position); // 플레이어의 위치로 이동
    }

    void StopChasingPlayer()
    {
        if (isChasing)
        {
            isChasing = false;
            agent.isStopped = true;    // 추적 중지
        }
    }


#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        // Gizmos의 색상 설정 (예: 빨간색)
        Gizmos.color = Color.red;

        // 몬스터의 감지 범위를 구체로 그리기
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

#endif

}
