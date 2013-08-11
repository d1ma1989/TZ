using UnityEngine;

public class Sphere : MonoBehaviour
{
    private Transform _transform;

    private float _speed;

    private int _points;

    public event System.Action<int> Destroyed;

    private void Start()
    {
        //caching transform component
        _transform = transform;
    }

    private void Update()
    {
        _transform.Translate(Vector3.down * _speed * Time.deltaTime, Space.World);

        if (_transform.position.y < -225f)
            Destroy(gameObject);
    }

    private void OnMouseDown()
    {
        if (Destroyed != null)
            Destroyed(_points);

        Destroy(gameObject);
    }

    public void CalculateSpeedAndPoints(float size, int difficulty)
    {
        _points = (int)(1000 / size);
        _speed = (int)(5000 / size) * difficulty;
    }
}
