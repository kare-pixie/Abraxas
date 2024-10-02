using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class TargetScanner : MonoBehaviour
{

    public PlayerController player;           // 플레이어의 위치를 받아올 변수
    public float detectionRange = 10f; // 몬스터가 플레이어를 인식할 범위
    public float moveSpeed = 3.5f;     // 몬스터 이동 속도
    public float fieldOfView = 45f;    // 몬스터의 시야각 (45도 예시)

    private NavMeshAgent agent;        // 몬스터의 NavMeshAgent
    private bool isChasing = false;    // 몬스터가 플레이어를 추적 중인지 확인하는 변수

    [Header("Attack Settings")] 
    public float attackRange = 2f;          // 공격 가능 범위
    public float minAttackInterval = 1.5f;    // 최소 공격 간격
    public float maxAttackInterval = 3f;    // 최대 공격 간격
    public int damage = 10;                 // 공격 데미지

    private bool isAttacking = false;       // 공격 상태
    private Animator animator;
    public LayerMask obstacleLayer;    // 장애물 레이어

    private Coroutine attackCoroutine;

    void Start()
    {
        TryGetComponent(out animator);
        agent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<PlayerController>();
        agent.speed = moveSpeed;      // 몬스터의 이동 속도 설정
    }

    void Update()
    {
        if (UIManager.instance != null && UIManager.instance.isGameOver)
        {
            StopChasingPlayer();
            return;
        }

        float distance = Vector3.Distance(player.transform.position, transform.position);

        // 플레이어가 시야각 내에 있고 시야에 들어왔을 때 추적을 시작
        if (IsPlayerInFieldOfView() && IsPlayerVisible())
        {
            isChasing = true;  // 추적 시작
        }

        // 추적 중일 때
        if (isChasing)
        {

            // 감지 범위 안에 있을 때 계속 추적
            if (distance <= detectionRange)
            {
                ChasePlayer();

                // 공격 범위 내에 있으면 공격
                if (distance <= attackRange && !isAttacking)
                {
                    if (attackCoroutine != null)
                        StopCoroutine(attackCoroutine);
                    attackCoroutine = StartCoroutine(AttackRoutine());
                }
            }
            else
            {
                // 감지 범위를 벗어나면 추적 중지
                StopChasingPlayer();
            }
        }
    }

    private bool IsPlayerVisible()
    {
        // 플레이어와의 방향 계산
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // Raycast를 사용하여 플레이어와의 시야에 장애물이 있는지 확인
        if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleLayer))
        {
            return true;  // 장애물이 없으면 플레이어가 시야에 있는 것
        }
        return false;      // 장애물이 있으면 플레이어가 시야에 없음
    }

    private bool IsPlayerInFieldOfView()
    {
        // 플레이어와의 방향 벡터
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;

        // 몬스터가 보고 있는 방향 (정면)
        Vector3 forward = transform.forward;

        // 몬스터 정면과 플레이어 방향 사이의 각도 계산
        float angleToPlayer = Vector3.Angle(forward, directionToPlayer);

        // 각도가 시야각보다 작으면 시야에 있는 것
        return angleToPlayer < fieldOfView / 2f;
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        float sec = 0.4f;

        while (Vector3.Distance(player.transform.position, transform.position) <= attackRange)
        {
            // 플레이어를 향해 회전
            Vector3 direction = (player.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

            // 공격 애니메이션 재생 (애니메이터가 있는 경우)
            Animator animator = GetComponent<Animator>();
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsTag("BlockMovement")) //
                break;

            if (animator != null)
            {
                animator.SetTrigger("Punch");
                sec = 0.4f;
            }

            // 데미지 적용 (예: 플레이어 스크립트에 데미지를 입히는 메서드 호출)
            PlayerStatus status = player.GetComponent<PlayerStatus>();
            if (status != null)
            {
                status.TakeDamage(damage, sec);
            }

            // 랜덤 간격 대기
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
        agent.isStopped = false;       // NavMeshAgent가 이동할 수 있도록 설정
        agent.SetDestination(player.transform.position); // 플레이어의 위치로 이동
    }

    private void StopChasingPlayer()
    {
        if (isChasing)
        {
            isChasing = false;
            agent.isStopped = true;    // 추적 중지

            if (attackCoroutine != null)
                StopCoroutine(attackCoroutine);
        }
    }


#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        // Gizmos의 색상 설정
        Gizmos.color = Color.red;

        // 몬스터의 감지 범위
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // 시야각 표시
        Gizmos.color = Color.yellow;
        Vector3 forward = transform.forward * detectionRange;
        Vector3 leftBoundary = Quaternion.Euler(0, -fieldOfView / 2, 0) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, fieldOfView / 2, 0) * forward;

        Gizmos.DrawRay(transform.position, leftBoundary);
        Gizmos.DrawRay(transform.position, rightBoundary);
    }

#endif

}
