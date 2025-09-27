using System.Collections;
using UnityEngine;

public class MapCamera : MonoBehaviour
{
    public float moveDuration = 0.5f;   // used by SlideCamera (optional)
    public float smoothTime = 0.25f;    // SmoothDamp time for Update smoothing
    public float panSpeed = 7.5f;       // how fast target moves when panning
    private int moveDirection = 0;      // -1 left, 1 right, 0 stop
    public float leftBounnds = 0f;
    public float rightBounnds = 14.5f;

    private Camera cam;
    private Vector3 targetPosition;
    private Vector3 smoothVelocity = Vector3.zero;

    // Remember original smoothTime for temporary overrides
    private float originalSmoothTime;

    void Awake()
    {
        cam = Camera.main;
    }

    void Start()
    {
        // initialize target position to current camera position
        targetPosition = cam.transform.position;
        originalSmoothTime = smoothTime;

        // move once at start to current node (uses targetPosition so it will smooth)
        MoveToCurrentNode();
    }

    void Update()
    {
        // 1) Input: mouse scroll - adjust target position (no direct Translate)
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scrollInput) > Mathf.Epsilon)
        {
            // compute desired delta and apply to target
            float deltaX = (scrollInput / 2f) * panSpeed;
            targetPosition += Vector3.right * deltaX;

            // clamp
            targetPosition.x = Mathf.Clamp(targetPosition.x, leftBounnds, rightBounnds);
        }

        // 2) If there's a direction to move in (e.g., held button), move target continuously
        if (moveDirection != 0)
        {
            // target moves continuously while holding direction
            targetPosition += Vector3.right * moveDirection * panSpeed * Time.deltaTime;

            // clamp
            targetPosition.x = Mathf.Clamp(targetPosition.x, leftBounnds, rightBounnds);
        }

        // 3) Smoothly move the camera toward targetPosition
        // keep camera y and z fixed (we only pan on X), but support different target Y/Z if set elsewhere
        Vector3 desired = new Vector3(targetPosition.x, targetPosition.y, targetPosition.z);
        cam.transform.position = Vector3.SmoothDamp(cam.transform.position, desired, ref smoothVelocity, smoothTime);
    }

    // called by UI/other scripts to start continuous panning
    public void StartPanning(int direction)
    {
        moveDirection = direction;
    }

    public void StopPanning()
    {
        moveDirection = 0;
    }

    // Move to current node by setting targetPosition (Update smoothing will handle the motion)
    private void MoveToCurrentNode()
    {
        MapNode current = SceneSwitcher.Instance.GetCurrentNode();
        if (current == null) return;

        // Skip trivial nodes (your previous logic)
        if (current.transform.position.x <= 0) return;

        Vector3 targetPos = current.transform.position;
        targetPos.z = cam.transform.position.z; // keep camera's existing z
        targetPos.y = cam.transform.position.y; // keep camera's existing y

        // Set as target; Update() will smooth towards it
        targetPosition = targetPos;
    }

    // Optional: older SlideCamera coroutine behavior — now sets targetPosition but temporarily adjusts smoothTime
    public void SlideCameraTo(Vector3 newTarget, float duration = -1f)
    {
        StopAllCoroutines(); // stop previous temporary overrides
        StartCoroutine(AutoSlideTo(newTarget, duration));
    }

    private IEnumerator AutoSlideTo(Vector3 newTarget, float duration)
    {
        // set newTarget's y/z to keep camera plane
        newTarget.z = cam.transform.position.z;
        newTarget.y = cam.transform.position.y;

        // clamp x
        newTarget.x = Mathf.Clamp(newTarget.x, leftBounnds, rightBounnds);

        if (duration > 0f)
        {
            // temporarily override smoothTime so it arrives roughly in duration seconds
            float startSmooth = smoothTime;
            // heuristic: set smoothTime relative to duration
            smoothTime = Mathf.Max(0.01f, duration * 0.45f);

            targetPosition = newTarget;

            // wait until near target or time exceeded
            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                if (Mathf.Abs(cam.transform.position.x - newTarget.x) < 0.01f) break;
                yield return null;
            }

            smoothTime = startSmooth; // restore
        }
        else
        {
            // no duration: just set target
            targetPosition = newTarget;
            yield break;
        }
    }
}
