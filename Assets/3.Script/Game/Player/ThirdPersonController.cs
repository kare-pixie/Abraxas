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
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    // player
    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    // animation IDs
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;

#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif
    private Animator _animator;
    private CharacterController _controller;
    private PlayerInputs _input;
    private GameObject _mainCamera;

    private const float _threshold = 0.01f;

    private bool _hasAnimator;

    private Vector3? _targetPosition = null; // ���콺 Ŭ������ ������ ��ǥ ��ġ
    private bool _isMovingToTarget = false;  // ���콺�� ������ ��ġ�� �̵� ������ ����

    private bool IsCurrentDeviceMouse
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
            return false;
#endif
        }
    }


    private void Awake()
    {
        // ���� ī�޶� ����
        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }

    private void Start()
    {
        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

        _hasAnimator = TryGetComponent(out _animator);
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<PlayerInputs>();
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#endif

        AssignAnimationIDs();

        // ���� �� Ÿ�Ӿƿ� ���� �ʱ�ȭ
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
    }
    private void Update()
    {
        _hasAnimator = TryGetComponent(out _animator);

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
                _targetPosition = hit.point; // Ŭ���� ��ġ�� ��ǥ�� ����
                _isMovingToTarget = true;    // ��ǥ �������� �̵� ����
            }
        }

        // Ű����� ������ �� ��ǥ ���������� �̵� ���
        if (_input.Move != Vector2.zero)
        {
            _targetPosition = null;  // ��ǥ ���� �ʱ�ȭ
            _isMovingToTarget = false; // ���콺 Ŭ������ �̵� ���
        }

        Move();
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    private void GroundedCheck()
    {
        // ���� ��ġ ���� (������ ����)
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
            transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);

        // ĳ���Ͱ� �ִϸ����͸� ����ϴ� ��� �ִϸ����� ������Ʈ
        if (_hasAnimator)
        {
            _animator.SetBool(_animIDGrounded, Grounded);
        }
    }

    private void CameraRotation()
    {
        if (_input.isRightClicking)
        {
            // ���� ���콺 ��ġ ��������
            Vector2 currentMousePosition = Mouse.current.position.ReadValue();

            // ���콺�� X�� �̵��� ��� (�̵����� �ΰ����� ����)
            float deltaX = currentMousePosition.x - _input.lastMousePosition.x;
            float rotationY = deltaX * cameraSensitivity;

            // ���� ������Ʈ�� Y�� ȸ��
            cameraRoot.transform.Rotate(0f, rotationY, 0f);

            // ������ ���콺 ��ġ ������Ʈ
            _input.lastMousePosition = currentMousePosition;

            _cinemachineTargetYaw += ClampAngle(rotationY, float.MinValue, float.MaxValue);

            // Cinemachine ī�޶� ������Ʈ
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }
        else
        {
            // ȸ�� ���� 360�� ������ ����
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine ī�޶� ������Ʈ
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }
    }
    private void Move()
    {
        // �̵� �ӵ� �Ǵ� �޸��� �ӵ��� ���� ��ǥ �ӵ� ����
        float targetSpeed = _input.Sprint ? SprintSpeed : MoveSpeed;

        float inputMagnitude = _input.AnalogMovement ? _input.Move.magnitude : 1f;

        // ���� ���� �ӵ� ���
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

        float speedOffset = 0.1f;

        // ī�޶��� ������ ������� ĳ������ �̵� ���� ����
        Vector3 targetDirection;

        // ���콺 Ŭ������ ��ǥ ������ ������ ���
        if (_isMovingToTarget && _targetPosition.HasValue)
        {
            targetDirection = (_targetPosition.Value - transform.position).normalized; // ��ǥ ���������� ����
            targetDirection.y = 0; // ���� �̵��� ó��

            // ��ǥ ���������� �Ÿ�
            float distanceToTarget = Vector3.Distance(transform.position, _targetPosition.Value);

            // ��ǥ ������ ���� ������ ��� ����
            if (distanceToTarget < 0.1f)
            {
                _isMovingToTarget = false; // �̵� �Ϸ�
                targetSpeed = 0.0f;
            }
            else
            {
                // ��ǥ ������ ���� ȸ��
                _targetRotation = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
        }
        else // WASD �Ǵ� ����Ű �̵� ó��
        {
            // �Է��� ������ ��ǥ �ӵ��� 0���� ����
            if (_input.Move == Vector2.zero) targetSpeed = 0.0f;

            // �Է� ������ ����ȭ
            Vector3 inputDirection = new Vector3(_input.Move.x, 0.0f, _input.Move.y).normalized;

            // ī�޶��� ������ ������� �̵� ������ ����
            if (_input.Move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // ĳ���͸� ī�޶� ���⿡ ���� ȸ��
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            // ī�޶��� ������ ������� ĳ������ �̵� ���� ����
            targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
        }

        // ��ǥ �ӵ��� ���� �Ǵ� ����
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // �������� ��� ��� ��� ����� �� �������� �ӵ� ��ȭ ����
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                Time.deltaTime * SpeedChangeRate);

            // �ӵ��� �Ҽ��� �� �ڸ��� �ݿø�
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        // �÷��̾� �̵�
        _controller.Move(targetDirection * (targetSpeed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        // ĳ���Ͱ� �ִϸ����͸� ����ϴ� ��� �ִϸ����� ������Ʈ
        if (_hasAnimator)
        {
            _animator.SetFloat(_animIDSpeed, _animationBlend);
            _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
        }
    }


    private void JumpAndGravity()
    {
        if (Grounded)
        {
            // ���� Ÿ�Ӿƿ� Ÿ�̸� �ʱ�ȭ
            _fallTimeoutDelta = FallTimeout;

            // ĳ���Ͱ� �ִϸ����͸� ����ϴ� ��� �ִϸ����� ������Ʈ
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFreeFall, false);
            }

            // ���鿡 ���� �� �ӵ��� ������ ������ �ʵ��� ����
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            // ����
            if (_input.Jump && _jumpTimeoutDelta <= 0.0f)
            {
                // H * -2 * G�� �������� ���ϴ� ���̿� �����ϱ� ���� �ʿ��� �ӵ��� ���
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                // ĳ���Ͱ� �ִϸ����͸� ����ϴ� ��� �ִϸ����� ������Ʈ
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, true);
                }
            }

            // ���� Ÿ�Ӿƿ�
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // ���� Ÿ�Ӿƿ� Ÿ�̸� �ʱ�ȭ
            _jumpTimeoutDelta = JumpTimeout;

            // ���� Ÿ�Ӿƿ�
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                // ĳ���Ͱ� �ִϸ����͸� ����ϴ� ��� �ִϸ����� ������Ʈ
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDFreeFall, true);
                }
            }

            // ���鿡 ���� ������ ������ �� ����
            _input.Jump = false;
        }

        // �͹̳� �ӵ� ������ ��� �ð��� ������ ���� �߷� ���� (�߷��� ���������� ����)
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += Gravity * Time.deltaTime;
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
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
        }
    }
}
