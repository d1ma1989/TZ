using UnityEngine;

public class BackgroundMoving : MonoBehaviour
{
    private const float SPEED = 200;

    private Transform _transform;

    private void Start()
    {
        //caching transform component
        _transform = transform;
    }

    //moving background to make flying effect
    private void Update()
    {
        //move with speed of 200 units in second
        _transform.position -= new Vector3(0, SPEED * Time.deltaTime, 0);

        if (_transform.position.y < -450)
            _transform.position = new Vector3(0, 450, 600);
    }
}
