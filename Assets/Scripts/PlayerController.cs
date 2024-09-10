using System;
using System.Collections;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
	#region Variables: Movement

	private Vector2 playerInput;
	private CharacterController _characterController;
	private Vector3 _direction;



	[SerializeField] private Movement movement;

	#endregion
	#region Variables: Rotation

	[SerializeField] private float rotationSpeed = 500f;
	private Camera _mainCamera;

	#endregion
	#region Variables: Gravity

	private float _gravity = -9.81f;
	[SerializeField] private float gravityMultiplier = 3.0f;
	private float _velocity;

	#endregion
	#region Variables: Jumping

	[SerializeField] private float jumpPower;
	private int _numberOfJumps;
	[SerializeField] private int maxNumberOfJumps = 2;

	#endregion

	public LayerMask wallLayer;

	StateMachine stateMachine = new StateMachine();
	
	private void Awake()
	{
		_characterController = GetComponent<CharacterController>();
		_mainCamera = Camera.main;
	}

	void Start()
	{
		stateMachine.ChangeState(IdleState);
		_numberOfJumps = 0;
	}

	private void Update()
	{
		stateMachine?.Execute();

		if(!stateMachine.IsState(ClimbState))
		{
			CheckWall();
		}

		
	}

	private void CheckWall()
	{
		// Raycast parameters
		float rayDistance = 1.0f; 
		Vector3 rayOrigin = transform.position; //+ Vector3.up;
		Vector3 rayDirection = transform.forward; 

		// Perform raycast to detect a wall in front of the player
		if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, rayDistance, wallLayer))
		{
			// Check if the object hit is close enough to the player to initiate climbing
			if (Vector3.Distance(transform.position, hit.point) < 0.8f)
			{
				stateMachine.ChangeState(ClimbState);
			}
		}
		else
		{
			// Optional: Debugging purposes to see the ray
			Debug.DrawRay(rayOrigin, rayDirection * rayDistance, Color.red);
		}
	}



	private void ApplyGravity()
	{
		if (IsGrounded() && _velocity < 0.0f)
		{
			_velocity = -1.0f;
		}
		else
		{
			_velocity += _gravity * gravityMultiplier * Time.deltaTime;
		}
		
		_direction.y = _velocity;
	}
	
	private void ApplyRotation()
	{
		if (playerInput.sqrMagnitude == 0) return;

		_direction = Quaternion.Euler(0.0f, _mainCamera.transform.eulerAngles.y, 0.0f) * new Vector3(playerInput.x, 0.0f, playerInput.y);
		var targetRotation = Quaternion.LookRotation(_direction, Vector3.up);

		transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
	}

	private void ApplyMovement()
	{
		var targetSpeed = movement.isSprinting ? movement.speed * movement.multiplier : movement.speed;
		movement.currentSpeed = Mathf.MoveTowards(movement.currentSpeed, targetSpeed, movement.acceleration * Time.deltaTime);

		_characterController.Move(_direction * movement.currentSpeed * Time.deltaTime);
	}

	public void Move(InputAction.CallbackContext context)
	{
		playerInput = context.ReadValue<Vector2>();

		if(stateMachine.IsState(ClimbState))
		{
			_direction = new Vector3(playerInput.x, playerInput.y, 0f);
		}
		else
		{
			_direction = new Vector3(playerInput.x, 0.0f, playerInput.y);
		}
		
	}

	public void Jump(InputAction.CallbackContext context)
	{
		if (!context.performed) return;
		if (!IsGrounded() && _numberOfJumps >= maxNumberOfJumps) return;
		if (_numberOfJumps == 0) StartCoroutine(WaitForLanding());

		_numberOfJumps++;

		stateMachine.ChangeState(JumpState);
		
	}

	public void Fire(InputAction.CallbackContext context)
	{
		if (!context.performed) return;
		
		//TODO:Change attack, box attack
	}

	private void ApplyJumping()
	{
		_velocity = jumpPower;
	}

	public void Sprint(InputAction.CallbackContext context)
	{
		movement.isSprinting = context.started || context.performed;
	}

	private IEnumerator WaitForLanding()
	{
		yield return new WaitUntil(() => !IsGrounded());
		yield return new WaitUntil(IsGrounded);

		_numberOfJumps = 0;
	}

	private bool IsGrounded() => _characterController.isGrounded;

	void Reset()
	{
		movement.speed = 5;
		movement.multiplier = 2;
		movement.acceleration = 20;
		rotationSpeed = 500;
		gravityMultiplier = 1;
		jumpPower = 3;
		maxNumberOfJumps = 1;
	}

	#region StateMachine

	private void BasicAction()
    {
        ApplyMovement();
        ApplyRotation();
        ApplyGravity();
    }

	private void IdleState(ref Action onEnter, ref Action onExecute, ref Action onExit)
	{
		onEnter = () =>
		{
			//TODO: Change animation
		};

		onExecute = () =>
		{
			if(playerInput != Vector2.zero)
			{
				stateMachine.ChangeState(MoveState);
			}

			BasicAction();
		};

		onExit = () =>
		{

		};
	}

	private void MoveState(ref Action onEnter, ref Action onExecute, ref Action onExit)
	{
		onEnter = () =>
		{
			//TODO: Change animation
		};

		onExecute = () =>
		{
			if(playerInput == Vector2.zero)
			{
				stateMachine.ChangeState(IdleState);
			}
			else
            {
                BasicAction();
            }

        };

		onExit = () =>
		{

		};
	}

    

    private void JumpState(ref Action onEnter, ref Action onExecute, ref Action onExit)
	{
		onEnter = () =>
		{
			//TODO: Change animation
			ApplyJumping();
		};

		onExecute = () =>
		{
			if(_velocity < 0)
			{
				stateMachine.ChangeState(FallState);
			}

			BasicAction();
		};

		onExit = () =>
		{

		};
	}

	private void FallState(ref Action onEnter, ref Action onExecute, ref Action onExit)
	{
		onEnter = () =>
		{
			//TODO: Change animation
		};

		onExecute = () =>
		{
			if(IsGrounded())
			{
				stateMachine.ChangeState(IdleState);
			}

			BasicAction();
		};

		onExit = () =>
		{

		};
	}

	// private void AttackState(ref Action onEnter, ref Action onExecute, ref Action onExit)
	// {
	// 	onEnter = () =>
	// 	{
	// 		//TODO: attack
	// 	};

	// 	onExecute = () =>
	// 	{
	// 		if(stateMachine.IsP_State(ClimbState))
	// 		{
	// 			//stateMachine.ChangeState(ClimbState);
	// 		}
	// 	};

	// 	onExit = () =>
	// 	{

	// 	};
	// }


	private void ClimbState(ref Action onEnter, ref Action onExecute, ref Action onExit)
	{
		onEnter = () =>
		{
			//TODO: Change animation
		};

		onExecute = () =>
		{
			ApplyMovement();

			// Perform raycast to detect a wall in front of the player
			if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 1.0f, wallLayer)) return;

			stateMachine.ChangeState(JumpState);

		};

		onExit = () =>
		{

		};
	}


	#endregion
}

[Serializable]
public struct Movement
{
	public float speed;
	public float multiplier;
	public float acceleration;

	[HideInInspector] public bool isSprinting;
	[HideInInspector] public float currentSpeed;
}