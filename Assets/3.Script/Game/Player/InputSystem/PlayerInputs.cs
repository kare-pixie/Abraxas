using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerInputs : MonoBehaviour
{
	[Header("캐릭터 입력 값")]
	[SerializeField] public Vector2 move; // 이동 입력 값
	[SerializeField] public bool jump; // 점프 입력 여부
	[SerializeField] public bool sprint; // 달리기 입력 여부
	public Vector2 Move { get { return move; } }
	public bool Jump { get { return jump; } set { jump = value; } }
	public bool Sprint { get { return sprint; } }

	[Header("움직임 설정")]
	[SerializeField] private bool analogMovement; // 아날로그 움직임 여부
	public bool AnalogMovement { get { return analogMovement; } }

	[Header("마우스 스크롤 설정")]
	[SerializeField] private Camera playerCamera;        // 카메라 참조
	[SerializeField] private float zoomSpeed = 0.2f; // 마우스 휠에 따른 줌 속도
	[SerializeField] private float minFov = 60f;  // 최소 FOV 값
	[SerializeField] private float maxFov = 100f; // 최대 FOV 값
	private float currentFov;    // 현재 FOV 값

	//카메라 시점 설정

	public bool isRightClicking { get; private set; } = false;  // 오른쪽 클릭이 눌린 상태인지 확인하는 변수
	public Vector2 lastMousePosition;     // 마지막 마우스 위치 저장


	private void Start()
	{
		// 카메라의 초기 FOV 값을 설정
		if (playerCamera == null)
		{
			playerCamera = Camera.main;
		}
		currentFov = playerCamera.fieldOfView;
	}

#if ENABLE_INPUT_SYSTEM
    public void OnMove(InputValue value)
	{
		MoveInput(value.Get<Vector2>());
	}

	public void OnJump(InputValue value)
	{
		//JumpInput(value.isPressed);
	}

	public void OnSprint(InputValue value)
	{
        SprintInput(value.isPressed);
    }

	public void OnRightClick(InputValue value)
	{
		isRightClicking = value.isPressed;

		// 오른쪽 마우스 클릭 시 마우스 위치 초기화
		if (isRightClicking)
		{
			lastMousePosition = Mouse.current.position.ReadValue();
		}
	}
	public void OnScroll(InputValue value)
	{
		// Vector2 형태로 Scroll 값을 받음 (y축 값이 휠의 방향)
		Vector2 scrollInput = value.Get<Vector2>();

		// 휠의 y축 값에 따라 FOV 값 조정
		float scrollAmount = scrollInput.y * zoomSpeed;
		currentFov -= scrollAmount;

		// FOV 값이 minFov와 maxFov 사이에 있도록 제한
		currentFov = Mathf.Clamp(currentFov, minFov, maxFov);

		// 카메라에 FOV 값 적용
		playerCamera.fieldOfView = currentFov;
	}
#endif
	public void MoveInput(Vector2 newMoveDirection)
	{
		move = newMoveDirection;
	}

	public void JumpInput(bool newJumpState)
	{
		jump = newJumpState;
	}

	public void SprintInput(bool newSprintState)
	{
		sprint = newSprintState;
	}
}