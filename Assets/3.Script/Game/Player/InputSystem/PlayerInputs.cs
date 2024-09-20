using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerInputs : MonoBehaviour
{
	[Header("ĳ���� �Է� ��")]
	[SerializeField] public Vector2 move; // �̵� �Է� ��
	[SerializeField] public bool jump; // ���� �Է� ����
	[SerializeField] public bool sprint; // �޸��� �Է� ����
	public Vector2 Move { get { return move; } }
	public bool Jump { get { return jump; } set { jump = value; } }
	public bool Sprint { get { return sprint; } }

	[Header("������ ����")]
	[SerializeField] private bool analogMovement; // �Ƴ��α� ������ ����
	public bool AnalogMovement { get { return analogMovement; } }

	[Header("���콺 ��ũ�� ����")]
	[SerializeField] private Camera playerCamera;        // ī�޶� ����
	[SerializeField] private float zoomSpeed = 0.2f; // ���콺 �ٿ� ���� �� �ӵ�
	[SerializeField] private float minFov = 60f;  // �ּ� FOV ��
	[SerializeField] private float maxFov = 100f; // �ִ� FOV ��
	private float currentFov;    // ���� FOV ��

	//ī�޶� ���� ����

	public bool isRightClicking { get; private set; } = false;  // ������ Ŭ���� ���� �������� Ȯ���ϴ� ����
	public Vector2 lastMousePosition;     // ������ ���콺 ��ġ ����


	private void Start()
	{
		// ī�޶��� �ʱ� FOV ���� ����
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

		// ������ ���콺 Ŭ�� �� ���콺 ��ġ �ʱ�ȭ
		if (isRightClicking)
		{
			lastMousePosition = Mouse.current.position.ReadValue();
		}
	}
	public void OnScroll(InputValue value)
	{
		// Vector2 ���·� Scroll ���� ���� (y�� ���� ���� ����)
		Vector2 scrollInput = value.Get<Vector2>();

		// ���� y�� ���� ���� FOV �� ����
		float scrollAmount = scrollInput.y * zoomSpeed;
		currentFov -= scrollAmount;

		// FOV ���� minFov�� maxFov ���̿� �ֵ��� ����
		currentFov = Mathf.Clamp(currentFov, minFov, maxFov);

		// ī�޶� FOV �� ����
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