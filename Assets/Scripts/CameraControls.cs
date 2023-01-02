using UnityEngine;
using UnityEngine.UIElements;

public class CameraControls : MonoBehaviour
{
    public Camera SceneCamera;
    public float ScrollStrength = 0.001f;
    public float ZoomStrength = 0.25f;

    private bool _scrolling;
    private Vector2 _lastMousePos;

    void Start()
    {
        _lastMousePos = Vector2.zero;
    }

    void Update()
    {
        HandleExit();
        HandleScroll();
        HandleZoom();
    }

    private void HandleExit()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    private void HandleScroll()
    {
        if (Input.GetMouseButton((int) MouseButton.RightMouse))
        {
            var currentMousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            if (_scrolling && currentMousePos.y != _lastMousePos.y)
            {
                var yDelta = (_lastMousePos.y - currentMousePos.y) * ScrollStrength;
                // TODO: Scroll on x-axis also?
                SceneCamera.transform.position = new Vector3(SceneCamera.transform.position.x, SceneCamera.transform.position.y + yDelta, SceneCamera.transform.position.z);
            }
            else
                _scrolling = true;
            _lastMousePos = currentMousePos;
        }
        else
        {
            _scrolling = false;
        }
    }

    private void HandleZoom()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            SceneCamera.orthographicSize -= ZoomStrength * (Input.mouseScrollDelta.y);
        }
    }
}
