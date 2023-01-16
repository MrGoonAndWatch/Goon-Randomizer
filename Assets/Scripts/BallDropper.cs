using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class BallDropper : MonoBehaviour
{
    [Tooltip("The time (in seconds) to wait before playing the sound again since the last time the ball hit the floor.")]
    [SerializeField]
    private float _ballHitsFloorSoundDelay = 1.0f;

    public bool DroppingBall;
    public TextMeshProUGUI BallDropStatusText;

    private const string BallPrefabPath = "Ball";

    private GameObject _ball;
    private bool _ballDropped;
    private int _ballForceDirection = 1;

    private void Start()
    {
        if (BallDropStatusText == null)
            BallDropStatusText = FindObjectOfType<BallDropStatusDisplay>().GetComponent<TextMeshProUGUI>();
    }

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
            var playSoundScript = _ball.AddComponent<PlaySoundWhenBallHitsGround>();
            playSoundScript.SoundCooldownTime = _ballHitsFloorSoundDelay;
            _ballDropped = true;
        }
    }

    public void ProcessUnstickMe()
    {
        if (!_ballDropped)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            BumpBall(new Vector2(5 * _ballForceDirection, 5));
            _ballForceDirection *= -1;
        }
    }

    public void BumpBall(Vector2? direction = null)
    {
        if(direction == null)
            direction = new Vector2(Random.Range(-15.0f, 15.0f), Random.Range(-15.0f, 15.0f));

        if (!_ballDropped)
            return;

        var ballRigidbody = _ball.GetComponent<Rigidbody2D>();
        ballRigidbody.AddForce(direction.Value, ForceMode2D.Impulse);
    }

    public void SetGravity(float newGravity)
    {
        if (!_ballDropped)
            return;
        var ballRigidBody = _ball.GetComponent<Rigidbody2D>();
        ballRigidBody.gravityScale = newGravity;
    }
}
