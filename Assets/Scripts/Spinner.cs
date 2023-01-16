using UnityEngine;

public class Spinner : MonoBehaviour
{
    private float _initSpeed;
    public float Speed = 1.0f;
    public bool CounterClockwise = false;

    private float direction;

    void Start()
    {
        _initSpeed = Speed;
    }

    void Update()
    {
        direction = CounterClockwise ? 1.0f : -1.0f;
        gameObject.transform.Rotate(Vector3.forward * Speed * direction * Time.deltaTime);
    }

    public void Reverse()
    {
        CounterClockwise = !CounterClockwise;
    }

    public void DoubleSpeed()
    {
        Speed = _initSpeed * 2;
    }

    public void HalfSpeed()
    {
        Speed = _initSpeed * 0.5f;
    }

    public void Stop()
    {
        Speed = 0.0f;
    }

    public void ResetSpeed()
    {
        Speed = _initSpeed;
    }
}
