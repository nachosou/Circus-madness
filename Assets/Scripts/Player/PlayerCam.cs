using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public float sensMultiplyer = 10;

    public Transform orientation;

    [SerializeField] WallRunning wallRunningScript;

    float xRotation;
    float yRotation;

    public float tiltAngle;
    public float tiltSpeed;
    private float currentTilt;
    public float rotationClamp = 90;

    Vector2 mouse;

    [SerializeField] private InputReader inputReader;

    private void Awake()
    {
        inputReader ??= FindAnyObjectByType<InputReader>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        inputReader.OnMoveCamera += AttemptCameraMove;
    }

    private void OnDisable()
    {
        inputReader.OnMoveCamera -= AttemptCameraMove;
    }

    private void Update()
    {
        yRotation += mouse.x;
        xRotation -= mouse.y;

        xRotation = Mathf.Clamp(xRotation, -rotationClamp, rotationClamp);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, WallRunTilt());
        orientation.rotation = Quaternion.Euler(0, yRotation, WallRunTilt());
    }

    private void ChangeSensDependingOnInputSource()
    {
        if (InputReader.isUsingController)
        {
            sensX /= sensMultiplyer;
            sensY /= sensMultiplyer;
        }
        else
        {
            sensX *= sensMultiplyer;
            sensY *= sensMultiplyer;
        }
    }

    private void AttemptCameraMove(Vector2 dir)
    {
        mouse.x = dir.x * Time.deltaTime * sensX;
        mouse.y = dir.y * Time.deltaTime * sensY;
    }

    private float WallRunTilt()
    {
        if (wallRunningScript.isPlayerWallRunning)
        {
            if (wallRunningScript.isRunningInLeftWall)
            {
                currentTilt = Mathf.MoveTowards(currentTilt, -tiltAngle, Time.deltaTime * tiltSpeed);
            }
            else if (wallRunningScript.isRunningInRightWall)
            {
                currentTilt = Mathf.MoveTowards(currentTilt, tiltAngle, Time.deltaTime * tiltSpeed);
            }
        }
        else
        {
            currentTilt = Mathf.MoveTowards(currentTilt, 0, Time.deltaTime * tiltSpeed);
        }

        float targetRotation = currentTilt;
        return targetRotation;
    }
}
