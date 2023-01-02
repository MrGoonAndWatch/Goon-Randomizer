using UnityEngine;

public class Spinner : MonoBehaviour
{
    public float Speed = 1.0f;
    public bool CounterClockwise = false;

    private float direction;

    void Update()
    {
        direction = CounterClockwise ? 1.0f : -1.0f;
        gameObject.transform.Rotate(Vector3.forward * Speed * direction * Time.deltaTime);
    }
}
