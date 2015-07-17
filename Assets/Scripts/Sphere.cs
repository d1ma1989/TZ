using UnityEngine;

using System;

public class Sphere : MonoBehaviour {
	private Transform _transform;

	private float _moveSpeed;
	private float _rotationSpeed;
	private int _points;

	public Color Color;

	public event Action<int, Vector3, Color> Destroyed;

	private void Start() {
		_transform = transform;
		_rotationSpeed = 100f * Time.deltaTime;
	}

	private void Update() {
		_transform.Translate(Vector3.down * _moveSpeed * Time.deltaTime, Space.World);
		_transform.Rotate(Vector3.up, _rotationSpeed);
		_transform.Rotate(Vector3.forward, _rotationSpeed);
	}

	private void OnBecameInvisible() {
		Destroy(gameObject);
	}

	private void OnMouseDown() {
		if (Destroyed != null) {
			Destroyed(_points, _transform.position, Color);
		}

		Destroy(gameObject);
	}

	public void CalculateSpeedAndPoints(float size, int difficulty) {
		_points = (int)(1000 / size);
		_moveSpeed = _points * 7 * difficulty;
	}
}
