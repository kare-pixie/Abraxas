using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
public class ThirdPersonController : MonoBehaviour
{
    [Header("Player")]
    [Tooltip("ĳ������ �̵� �ӵ� (m/s)")]
    public float MoveSpeed = 2.0f;

    [Tooltip("ĳ������ �޸��� �ӵ� (m/s)")]
    public float SprintSpeed = 5.335f;

    [Tooltip("ĳ���Ͱ� �����̴� �������� ȸ���ϴ� �ӵ�")]
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;

    [Tooltip("���� �� ���� ����")]
    public float SpeedChangeRate = 10.0f;

    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

    [Space(10)]
    [Tooltip("�÷��̾ ������ �� �ִ� ����")]
    public float JumpHeight = 1.2f;

    [Tooltip("ĳ���Ͱ� ����ϴ� �߷� ��. �⺻ ���� ���� -9.81f")]
    public float Gravity = -15.0f;

    [Space(10)]
    [Tooltip("�ٽ� ������ �� �ֱ���� �ʿ��� �ð�. 0���� �����ϸ� ��� �ٽ� ������ �� ����")]
    public float JumpTimeout = 0.50f;

    [Tooltip("���� ���·� ��ȯ�Ǳ� �������� �ð�. ����� ������ �� ������")]
    public float FallTimeout = 0.15f;

    [Header("Player Grounded")]
    [Tooltip("ĳ���Ͱ� ���鿡 ��� �ִ��� ����. CharacterController�� �⺻ ���� üũ�ʹ� ����")]
    public bool Grounded = true;

    [Tooltip("�ұ�Ģ�� ���鿡�� ������ ������")]
    public float GroundedOffset = -0.14f;

    [Tooltip("���� üũ�� ����� �ݰ�. CharacterController �ݰ�� ��ġ�ؾ� ��")]
    public float GroundedRadius = 0.28f;

    [Tooltip("ĳ���Ͱ� �������� ����� ���̾�")]
    public LayerMask GroundLayers;

    [Header("Cinemachine")]
    [Tooltip("Cinemachine ���� ī�޶� ���� ��ǥ")]
    public GameObject CinemachineCameraTarget;

    [Tooltip("ī�޶� ���� �̵��� �� �ִ� �ִ� ����")]
    public float TopClamp = 70.0f;

    [Tooltip("ī�޶� �Ʒ��� �̵��� �� �ִ� �ִ� ����")]
    public float BottomClamp = -30.0f;

    [Tooltip("ī�޶��� ��ġ�� �̼��ϰ� �����ϱ� ���� ���� �������̵�")]
    public float CameraAngleOverride = 0.0f;

    [Tooltip("ī�޶��� ��� �࿡�� ��ġ�� ��ױ� ���� ����")]
    public bool LockCameraPosition = false;

    [Tooltip("ī�޶��� ���� ����")]
    [SerializeField] private GameObject cameraRoot; // ī�޶��� ��Ʈ ������Ʈ
    [SerializeField] private float cameraSensitivity = 0.1f;  // ���콺 ���� (ȸ�� �ӵ�)

    // cinemachine
    private float cinemachineTargetYaw;
    private float cinemachineTargetPitch;

    // player
    private float speed;
    private float animationBlend;
    private float targetRotation = 0.0f;
    private float rotationVelocity;
    private float verticalVelocity;
    private float terminalVelocity = 53.0f;

    // timeout deltatime
    private float jumpTimeoutDelta;
    private float fallTimeoutDelta;

    // animation IDs
    private int animIDSpeed;
    private int animIDGrounded;
    private int animIDJump;
    private int animIDFreeFall;
    private int animIDMotionSpeed;

#if ENABLE_INPUT_SYSTEM
        private PlayerInput playerInput;
#endif
    private Animator animator;
    private CharacterController controller;
    private PlayerInputs input;
    private GameObject mainCamera;

    private const float threshold = 0.01f;

    private bool hasAnimator;

    private bool isMovementBlocked;

    private Vector3? targetPosition = null; // ���콺 Ŭ������ ������ ��ǥ ��ġ
    private bool isMovingToTarget = false;  // ���콺�� ������ ��ġ�� �̵� ������ ����

    private bool IsCurrentDeviceMouse
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
                return playerInput.currentControlScheme == "KeyboardMouse";
#else
            return false;
#endif
        }
    }


    private void Awake()
    {
        // ���� ī�޶� ����
        if (mainCamera == null)
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }

    private void Start()
    {
        cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

        hasAnimator = TryGetComponent(out animator);
        controller = GetComponent<CharacterController>();
        input = GetComponent<PlayerInputs>();
#if ENABLE_INPUT_SYSTEM
            playerInput = GetComponent<PlayerInput>();
#endif

        AssignAnimationIDs();

        // ���� �� Ÿ�Ӿƿ� ���� �ʱ�ȭ
        jumpTimeoutDelta = JumpTimeout;
        fallTimeoutDelta = FallTimeout;
    }
    private void Update()
    {
        hasAnimator = TryGetComponent(out animator);

        JumpAndGravity();
        GroundedCheck();

        // ���콺 Ŭ������ �̵��� ��ġ ����
        if (Input.GetMouseButtonDown(0)) // ���콺 ���� Ŭ��
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // �����̳� �̵� ������ ǥ�鿡 �¾��� ��
            if (Physics.Raycast(ray, out hit))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }
                targetPosition = hit.point; // Ŭ���� ��ġ�� ��ǥ�� ����
                isMovingToTarget = true;    // ��ǥ �������� �̵� ����
            }
        }

        // Ű����� ������ �� ��ǥ ���������� �̵� ���
        if (input.Move != Vector2.zero)
        {
            targetPosition = null;  // ��ǥ ���� �ʱ�ȭ
            isMovingToTarget = false; // ���콺 Ŭ������ �̵� ���
        }

        // ���� ��� ���� �ִϸ��̼��� BlockMovement �±׸� �������� Ȯ��
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsTag("BlockMovement"))
        {
            isMovementBlocked = true;
            targetPosition = null;
            isMovingToTarget = false; // �̵� ���
            animationBlend = 0; // �ȴ� �ִϸ��̼� �ʱ�ȭ
            if (hasAnimator)
            {
                animator.SetFloat(animIDSpeed, animationBlend);
            }
        }
        else
        {
            isMovementBlocked = false;
        }

        // ĳ���� �̵�
        if (!isMovementBlocked)
        {
            Move();
        }
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void AssignAnimationIDs()
    {
        animIDSpeed = Animator.StringToHash("Speed");
        animIDGrounded = Animator.StringToHash("Grounded");
        animIDJump = Animator.StringToHash("Jump");
        animIDFreeFall = Animator.StringToHash("FreeFall");
        animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    private void GroundedCheck()
    {
        // ���� ��ġ ���� (������ ����)
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
            transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);

        // ĳ���Ͱ� �ִϸ����͸� ����ϴ� ��� �ִϸ����� ������Ʈ
        if (hasAnimator)
        {
            animator.SetBool(animIDGrounded, Grounded);
        }
    }

    private void CameraRotation()
    {
        if (input.isRightClicking)
        {
            // ���� ���콺 ��ġ ��������
            Vector2 currentMousePosition = Mouse.current.position.ReadValue();

            // ���콺�� X�� �̵��� ��� (�̵����� �ΰ����� ����)
            float deltaX = currentMousePosition.x - input.lastMousePosition.x;
            float rotationY = deltaX * cameraSensitivity;

            // ���� ������Ʈ�� Y�� ȸ��
            cameraRoot.transform.Rotate(0f, rotationY, 0f);

            // ������ ���콺 ��ġ ������Ʈ
            input.lastMousePosition = currentMousePosition;

            cinemachineTargetYaw += ClampAngle(rotationY, float.MinValue, float.MaxValue);

            // Cinemachine ī�޶� ������Ʈ
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(cinemachineTargetPitch + CameraAngleOverride,
                cinemachineTargetYaw, 0.0f);
        }
        else
        {
            // ȸ�� ���� 360�� ������ ����
            cinemachineTargetYaw = ClampAngle(cinemachineTargetYaw, float.MinValue, float.MaxValue);
            cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine ī�޶� ������Ʈ
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(cinemachineTargetPitch + CameraAngleOverride,
                cinemachineTargetYaw, 0.0f);
        }
    }
    private void Move()
    {
        // �̵� �ӵ� �Ǵ� �޸��� �ӵ��� ���� ��ǥ �ӵ� ����
        float targetSpeed = input.Sprint ? SprintSpeed : MoveSpeed;

        float inputMagnitude = input.AnalogMovement ? input.Move.magnitude : 1f;

        // ���� ���� �ӵ� ���
        float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

        float speedOffset = 0.1f;

        // ī�޶��� ������ ������� ĳ������ �̵� ���� ����
        Vector3 targetDirection;

        // ���콺 Ŭ������ ��ǥ ������ ������ ���
        if (isMovingToTarget && targetPosition.HasValue)
        {
            targetDirection = (targetPosition.Value - transform.position).normalized; // ��ǥ ���������� ����
            targetDirection.y = 0; // ���� �̵��� ó��

            // ��ǥ ���������� �Ÿ�
            float distanceToTarget = Vector3.Distance(transform.position, targetPosition.Value);

            // ��ǥ ������ ���� ������ ��� ����
            if (distanceToTarget < 0.1f)
            {
                isMovingToTarget = false; // �̵� �Ϸ�
                targetSpeed = 0.0f;
            }
            else
            {
                // ��ǥ ������ ���� ȸ��
                targetRotation = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, RotationSmoothTime);
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
        }
        else // WASD �Ǵ� ����Ű �̵� ó��
        {
            // �Է��� ������ ��ǥ �ӵ��� 0���� ����
            if (input.Move == Vector2.zero) targetSpeed = 0.0f;

            // �Է� ������ ����ȭ
            Vector3 inputDirection = new Vector3(input.Move.x, 0.0f, input.Move.y).normalized;

            // ī�޶��� ������ ������� �̵� ������ ����
            if (input.Move != Vector2.zero)
            {
                targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity,
                    RotationSmoothTime);

                // ĳ���͸� ī�޶� ���⿡ ���� ȸ��
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            // ī�޶��� ������ ������� ĳ������ �̵� ���� ����
            targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;
        }

        // ��ǥ �ӵ��� ���� �Ǵ� ����
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // �������� ��� ��� ��� ����� �� �������� �ӵ� ��ȭ ����
            speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                Time.deltaTime * SpeedChangeRate);

            // �ӵ��� �Ҽ��� �� �ڸ��� �ݿø�
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }
        else
        {
            speed = targetSpeed;
        }

        // �÷��̾� �̵�
        controller.Move(targetDirection * (targetSpeed * Time.deltaTime) + new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);

        animationBlend = Mathf.Lerp(animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (animationBlend < 0.01f) animationBlend = 0f;

        // ĳ���Ͱ� �ִϸ����͸� ����ϴ� ��� �ִϸ����� ������Ʈ
        if (hasAnimator)
        {
            animator.SetFloat(animIDSpeed, animationBlend);
            animator.SetFloat(animIDMotionSpeed, inputMagnitude);
        }
    }


    private void JumpAndGravity()
    {
        if (Grounded)
        {
            // ���� Ÿ�Ӿƿ� Ÿ�̸� �ʱ�ȭ
            fallTimeoutDelta = FallTimeout;

            // ĳ���Ͱ� �ִϸ����͸� ����ϴ� ��� �ִϸ����� ������Ʈ
            if (hasAnimator)
            {
                animator.SetBool(animIDJump, false);
                animator.SetBool(animIDFreeFall, false);
            }

            // ���鿡 ���� �� �ӵ��� ������ ������ �ʵ��� ����
            if (verticalVelocity < 0.0f)
            {
                verticalVelocity = -2f;
            }

            // ����
            if (input.Jump && jumpTimeoutDelta <= 0.0f)
            {
                // H * -2 * G�� �������� ���ϴ� ���̿� �����ϱ� ���� �ʿ��� �ӵ��� ���
                verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                // ĳ���Ͱ� �ִϸ����͸� ����ϴ� ��� �ִϸ����� ������Ʈ
                if (hasAnimator)
                {
                    animator.SetBool(animIDJump, true);
                }
            }

            // ���� Ÿ�Ӿƿ�
            if (jumpTimeoutDelta >= 0.0f)
            {
                jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // ���� Ÿ�Ӿƿ� Ÿ�̸� �ʱ�ȭ
            jumpTimeoutDelta = JumpTimeout;

            // ���� Ÿ�Ӿƿ�
            if (fallTimeoutDelta >= 0.0f)
            {
                fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                // ĳ���Ͱ� �ִϸ����͸� ����ϴ� ��� �ִϸ����� ������Ʈ
                if (hasAnimator)
                {
                    animator.SetBool(animIDFreeFall, true);
                }
            }

            // ���鿡 ���� ������ ������ �� ����
            input.Jump = false;
        }

        // �͹̳� �ӵ� ������ ��� �ð��� ������ ���� �߷� ���� (�߷��� ���������� ����)
        if (verticalVelocity < terminalVelocity)
        {
            verticalVelocity += Gravity * Time.deltaTime;
        }
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (Grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // ���õ� ��� ���� �浹ü�� ��ġ�ϴ� ��ġ�� ��ü �׸���
        Gizmos.DrawSphere(
            new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
            GroundedRadius);
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(controller.center), FootstepAudioVolume);
            }
        }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(controller.center), FootstepAudioVolume);
        }
    }
}
