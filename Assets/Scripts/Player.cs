using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class Player : MonoBehaviour
{
	[Header("Preinstalled cubes")]
	[SerializeField] private List<Transform> _playersCubes;

	[Header("Material's settings")]
	[SerializeField] private Material _playersMat;
	[SerializeField] private Material _neitralMat;

	private Rigidbody _rb;
	private BoxCollider _bc;
	[Header("Physics settings")]
	[SerializeField] private Vector3 _gravity;
	[SerializeField] private float _maxJumpY;
	[SerializeField] private float _jumpForce;
	private Vector3 _jumpVector = new Vector3();
	private bool _canJump = false;

	[SerializeField] private float _speed;
	private bool _canMove = false;
	private Vector3 _moveVector = new Vector3();
	private Vector3 _newPosVertical = new Vector3();

	private Vector3 _bcDeltaSize = new Vector3(0.0f, 1.0f, 0.0f);
	private Vector3 _bcDeltaCenter = new Vector3(0.0f, 0.5f, 0.0f);

	public static Player Instance { get; private set; }

	[Header("Player's events")]
	public UnityEvent OnPlayerDead;
	public UnityEvent OnPlayerWin;
	public UnityEvent OnPlayerStart;


	private void Awake()
	{
		Instance = this;
	}


	private void Start()
	{
		Application.targetFrameRate = 60;
		
		_rb = this.gameObject.GetComponent<Rigidbody>();
		_bc = this.gameObject.GetComponent<BoxCollider>();

		Physics.gravity = _gravity * 3.0f; 
	}


	private void Update()
	{
		if (Input.GetMouseButton(0))
		{
			_jumpVector.Set(0.0f, _jumpForce, 0.0f);
		}

		if (Input.GetMouseButtonUp(0))
		{
			_canJump = false;
		}
	}


	private void FixedUpdate()
	{
		jump();

		move();
	}


	private void jump()
	{
		if (!_canJump) return;

		_rb.AddForce(_jumpVector, ForceMode.Impulse);

		if (this.transform.position.y > _maxJumpY)
		{
			_canJump = false;
		}
	}


	private void move()
	{
		if (!_canMove) return;

		_moveVector.Set(
			0.0f,
			0.0f,
			_speed * Time.fixedDeltaTime);
		this.transform.Translate(_moveVector, Space.World);
	}


	private void OnCollisionStay(Collision collision)
	{
		if(collision.gameObject.tag == "ground")
		{
			detectGround();
		}
	}


	private void OnCollisionEnter(Collision collision)
	{
		var collisionGO = collision.gameObject;

		switch (collisionGO.tag)
		{
			case "enemy":
				detectEnemy(collisionGO);
				break;
			case "neitral":
				detectNeitral(collisionGO);
				break;
			case "Finish":
				detectFinish();
				break;
			default:
				break;
		}
	}

	private void detectFinish()
	{
		OnPlayerWin?.Invoke();
	}

	private void detectGround()
	{
		_jumpVector.Set(0.0f, 0.0f, 0.0f);
		_canJump = true;
	}

	private void detectEnemy(GameObject enemyGO)
	{
		enemyGO.tag = "Untagged";
		Debug.Log("Detect enemy!");

		if (_playersCubes.Count == 1)
		{
			playerDead();
			return;
		}

		var lastCube = _playersCubes[_playersCubes.Count - 1];
		lastCube.parent = enemyGO.transform;
		_playersCubes.Remove(lastCube);

		_newPosVertical.Set(0.0f, 1.0f, 0.0f);
		this.transform.position += _newPosVertical;
		_newPosVertical.Set(0.0f, -1.0f, 0.0f);
		foreach (var cube in _playersCubes)
		{
			cube.localPosition += _newPosVertical;
		}
		_bc.size -= _bcDeltaSize;
		_bc.center -= _bcDeltaCenter;

		var lastCubeGO = lastCube.gameObject;
		lastCubeGO.GetComponent<MeshRenderer>().material = _neitralMat;
		lastCubeGO.GetComponent<BoxCollider>().enabled = true;
		lastCubeGO.AddComponent<Rigidbody>();
		lastCubeGO.tag = "Untagged";
	}


	private void detectNeitral(GameObject neitralGo)
	{
		Debug.Log("Detect neitral!");
		neitralGo.GetComponent<BoxCollider>().enabled = false;
		neitralGo.GetComponent<MeshRenderer>().material = _playersMat;

		_newPosVertical.Set(0.0f, 1.0f, 0.0f);
		foreach(var cube in _playersCubes)
		{
			cube.localPosition += _newPosVertical;
		}
		_bc.size += _bcDeltaSize;
		_bc.center += _bcDeltaCenter;

		var neitralTransform = neitralGo.transform;
		neitralTransform.parent = this.transform;
		_newPosVertical.Set(0.0f, 0.0f, 0.0f);
		neitralTransform.localPosition = _newPosVertical;
		_playersCubes.Add(neitralTransform);
	}


	public void StartGame()
	{
		_canJump = true;
		_canMove = true;
		OnPlayerStart?.Invoke();
	}


	private void playerDead()
	{
		OnPlayerDead?.Invoke();
	}
}
