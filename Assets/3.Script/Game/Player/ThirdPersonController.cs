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
    [Tooltip("캐릭터의 이동 속도 (m/s)")]
    public float MoveSpeed = 2.0f;

    [Tooltip("캐릭터의 달리기 속도 (m/s)")]
    public float SprintSpeed = 5.335f;

    [Tooltip("캐릭터가 움직이는 방향으로 회전하는 속도")]
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;

    [Tooltip("가속 및 감속 비율")]
    public float SpeedChangeRate = 10.0f;

    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

    [Space(10)]
    [Tooltip("플레이어가 점프할 수 있는 높이")]
    public float JumpHeight = 1.2f;

    [Tooltip("캐릭터가 사용하는 중력 값. 기본 엔진 값은 -9.81f")]
    public float Gravity = -15.0f;

    [Space(10)]
    [Tooltip("다시 점프할 수 있기까지 필요한 시간. 0으로 설정하면 즉시 다시 점프할 수 있음")]
    public float JumpTimeout = 0.50f;

    [Tooltip("낙하 상태로 전환되기 전까지의 시간. 계단을 내려갈 때 유용함")]
    public float FallTimeout = 0.15f;

    [Header("Player Grounded")]
    [Tooltip("캐릭터가 지면에 닿아 있는지 여부. CharacterController의 기본 지면 체크와는 별도")]
    public bool Grounded = true;

    [Tooltip("불규칙한 지면에서 유용한 오프셋")]
    public float GroundedOffset = -0.14f;

    [Tooltip("지면 체크에 사용할 반경. CharacterController 반경과 일치해야 함")]
    public float GroundedRadius = 0.28f;

    [Tooltip("캐릭터가 지면으로 사용할 레이어")]
    public LayerMask GroundLayers;

    [Header("Cinemachine")]
    [Tooltip("Cinemachine 가상 카메라가 따라갈 목표")]
    public GameObject CinemachineCameraTarget;

    [Tooltip("카메라가 위로 이동할 수 있는 최대 각도")]
    public float TopClamp = 70.0f;

    [Tooltip("카메라가 아래로 이동할 수 있는 최대 각도")]
    public float BottomClamp = -30.0f;

    [Tooltip("카메라의 위치를 미세하게 조정하기 위한 각도 오버라이드")]
    public float CameraAngleOverride = 0.0f;

    [Tooltip("카메라의 모든 축에서 위치를 잠그기 위한 설정")]
    public bool LockCameraPosition = false;

    [Tooltip("카메라의 시점 설정")]
    [SerializeField] private GameObject cameraRoot; // 카메라의 루트 오브젝트
    [SerializeField] private float cameraSensitivity = 0.1f;  // 마우스 감도 (회전 속도)

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

    private Vector3? _targetPosition = null; // 마우스 클릭으로 설정된 목표 위치
    private bool _isMovingToTarget = false;  // 마우스로 설정된 위치로 이동 중인지 여부

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
        // 메인 카메라를 참조
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

        // 시작 시 타임아웃 값을 초기화
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
    }
    private void Update()
    {
        _hasAnimator = TryGetComponent(out _animator);

        JumpAndGravity();
        GroundedCheck();

        // 마우스 클릭으로 이동할 위치 설정
        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 클릭
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // 지형이나 이동 가능한 표면에 맞았을 때
            if (Physics.Raycast(ray, out hit))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }
                _targetPosition = hit.point; // 클릭한 위치를 목표로 설정
                _isMovingToTarget = true;    // 목표 지점으로 이동 시작
            }
        }

        // 키보드로 움직일 때 목표 지점으로의 이동 취소
        if (_input.Move != Vector2.zero)
        {
            _targetPosition = null;  // 목표 지점 초기화
            _isMovingToTarget = false; // 마우스 클릭으로 이동 취소
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
        // 구의 위치 설정 (오프셋 적용)
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
            transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);

        // 캐릭터가 애니메이터를 사용하는 경우 애니메이터 업데이트
        if (_hasAnimator)
        {
            _animator.SetBool(_animIDGrounded, Grounded);
        }
    }

    private void CameraRotation()
    {
        if (_input.isRightClicking)
        {
            // 현재 마우스 위치 가져오기
            Vector2 currentMousePosition = Mouse.current.position.ReadValue();

            // 마우스의 X축 이동량 계산 (이동량에 민감도를 곱함)
            float deltaX = currentMousePosition.x - _input.lastMousePosition.x;
            float rotationY = deltaX * cameraSensitivity;

            // 게임 오브젝트의 Y축 회전
            cameraRoot.transform.Rotate(0f, rotationY, 0f);

            // 마지막 마우스 위치 업데이트
            _input.lastMousePosition = currentMousePosition;

            _cinemachineTargetYaw += ClampAngle(rotationY, float.MinValue, float.MaxValue);

            // Cinemachine 카메라 업데이트
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }
        else
        {
            // 회전 값을 360도 범위로 제한
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine 카메라 업데이트
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }
    }
    private void Move()
    {
        // 이동 속도 또는 달리기 속도에 따라 목표 속도 설정
        float targetSpeed = _input.Sprint ? SprintSpeed : MoveSpeed;

        float inputMagnitude = _input.AnalogMovement ? _input.Move.magnitude : 1f;

        // 현재 수평 속도 계산
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

        float speedOffset = 0.1f;

        // 카메라의 방향을 기반으로 캐릭터의 이동 방향 설정
        Vector3 targetDirection;

        // 마우스 클릭으로 목표 지점이 설정된 경우
        if (_isMovingToTarget && _targetPosition.HasValue)
        {
            targetDirection = (_targetPosition.Value - transform.position).normalized; // 목표 지점까지의 방향
            targetDirection.y = 0; // 수평 이동만 처리

            // 목표 지점까지의 거리
            float distanceToTarget = Vector3.Distance(transform.position, _targetPosition.Value);

            // 목표 지점에 거의 도달한 경우 멈춤
            if (distanceToTarget < 0.1f)
            {
                _isMovingToTarget = false; // 이동 완료
                targetSpeed = 0.0f;
            }
            else
            {
                // 목표 지점을 향해 회전
                _targetRotation = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
        }
        else // WASD 또는 방향키 이동 처리
        {
            // 입력이 없으면 목표 속도를 0으로 설정
            if (_input.Move == Vector2.zero) targetSpeed = 0.0f;

            // 입력 방향을 정규화
            Vector3 inputDirection = new Vector3(_input.Move.x, 0.0f, _input.Move.y).normalized;

            // 카메라의 방향을 기반으로 이동 방향을 결정
            if (_input.Move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // 캐릭터를 카메라 방향에 맞춰 회전
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            // 카메라의 방향을 기반으로 캐릭터의 이동 방향 설정
            targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
        }

        // 목표 속도로 가속 또는 감속
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // 선형적인 결과 대신 곡선을 만들어 더 유기적인 속도 변화 제공
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                Time.deltaTime * SpeedChangeRate);

            // 속도를 소수점 세 자리로 반올림
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        // 플레이어 이동
        _controller.Move(targetDirection * (targetSpeed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        // 캐릭터가 애니메이터를 사용하는 경우 애니메이터 업데이트
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
            // 낙하 타임아웃 타이머 초기화
            _fallTimeoutDelta = FallTimeout;

            // 캐릭터가 애니메이터를 사용하는 경우 애니메이터 업데이트
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFreeFall, false);
            }

            // 지면에 있을 때 속도를 무한정 낮추지 않도록 설정
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            // 점프
            if (_input.Jump && _jumpTimeoutDelta <= 0.0f)
            {
                // H * -2 * G의 제곱근은 원하는 높이에 도달하기 위해 필요한 속도를 계산
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                // 캐릭터가 애니메이터를 사용하는 경우 애니메이터 업데이트
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, true);
                }
            }

            // 점프 타임아웃
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // 점프 타임아웃 타이머 초기화
            _jumpTimeoutDelta = JumpTimeout;

            // 낙하 타임아웃
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                // 캐릭터가 애니메이터를 사용하는 경우 애니메이터 업데이트
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDFreeFall, true);
                }
            }

            // 지면에 있지 않으면 점프할 수 없음
            _input.Jump = false;
        }

        // 터미널 속도 이하일 경우 시간이 지남에 따라 중력 적용 (중력을 선형적으로 증가)
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

        // 선택된 경우 지면 충돌체와 일치하는 위치에 구체 그리기
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
