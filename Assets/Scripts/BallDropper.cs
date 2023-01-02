using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class BallDropper : MonoBehaviour
{
    public bool DroppingBall;
    public TextMeshProUGUI BallDropStatusText;

    private const string BallPrefabPath = "Ball";

    private GameObject _ball;
    private bool _ballDropped;
    private int _ballForceDirection = 1;

    void Update()
    {
        ProcessToggleEnable();
        ProcessReset();
        ProcessBallDrop();
        ProcessUnstickMe();
    }

    private void ProcessToggleEnable()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            DroppingBall = !DroppingBall;
            if (DroppingBall)
                BallDropStatusText.text = "Drop Active";
            else
                BallDropStatusText.text = "Drop Locked";
        }
    }

    private void ProcessReset()
    {
        if (!_ballDropped)
            return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            Destroy(_ball);
            _ballDropped = false;
        }
    }

    private void ProcessBallDrop()
    {
        if (!DroppingBall || _ballDropped)
            return;

        if (Input.GetMouseButtonDown((int)MouseButton.LeftMouse))
        {
            var ballPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            ballPosition.z = 0;
            var ballResource = Resources.Load(BallPrefabPath);
            _ball = Instantiate(ballResource, ballPosition, Quaternion.identity) as GameObject;
            _ballDropped = true;
        }
    }

    private void ProcessUnstickMe()
    {
        if (!_ballDropped)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            var ballRigidbody = _ball.GetComponent<Rigidbody2D>();
            ballRigidbody.AddForce(new Vector2(5 * _ballForceDirection, 5), ForceMode2D.Impulse);
            _ballForceDirection *= -1;
        }
    }
}
