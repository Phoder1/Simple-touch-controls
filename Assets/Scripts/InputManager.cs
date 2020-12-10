using UnityEngine;


public class InputManager : MonoBehaviour {
    enum TouchMode { Drag, Joystick, None }

    [SerializeField]
    float joystickSpeed;

    [SerializeField]
    Canvas canvas;
    [SerializeField]
    GameObject virtualJoystick;
    [SerializeField]
    GameObject knob;
    [SerializeField]
    GameObject joystickOutline;
    [SerializeField]
    GameObject slave;
    [SerializeField]
    Camera mainCamera;

    float itemDetectionDistance = 1.3f;
    float JoystickSize = 150f;

    TouchMode mode = TouchMode.None;

    Vector3 touchRatioPos;

    Vector2 joystickPosition;
    Vector2 joystickVector;

    private void Start() {
    }

    // Update is called once per frame
    void Update() {
        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);

            Vector2 touchPosition = mainCamera.ScreenToWorldPoint(touch.position);
            switch (touch.phase) {
                case TouchPhase.Began:
                    Debug.Log(Vector2.Distance(touch.position, joystickOutline.transform.position));
                    Debug.Log(JoystickSize);
                    if (Vector2.Distance(touchPosition, slave.transform.position) <= itemDetectionDistance) {
                        touchRatioPos = slave.transform.position - (Vector3)touchPosition;
                        mode = TouchMode.Drag;
                    }
                    else {
                        mode = TouchMode.Joystick;
                        joystickPosition = touch.position;
                        virtualJoystick.transform.position = joystickPosition;
                        virtualJoystick.SetActive(true);
                    }
                    break;
                case TouchPhase.Moved:
                    if (mode == TouchMode.Drag) {
                        slave.transform.position = (Vector3)touchPosition + touchRatioPos;
                    }
                    else if (mode == TouchMode.Joystick) {
                        joystickVector = touch.position - joystickPosition;
                        if (joystickVector.magnitude <= 20) {
                            joystickVector = Vector2.zero;
                        }
                        if (joystickVector.magnitude >= JoystickSize / 2) {
                            joystickVector = joystickVector.normalized * JoystickSize / 2;
                        }
                        knob.transform.position = joystickPosition + joystickVector;
                    }

                    MoveObject(joystickVector);



                    break;
                case TouchPhase.Stationary:
                    MoveObject(joystickVector);
                    break;
                case TouchPhase.Ended:
                    mode = TouchMode.None;
                    ResetJoystick();
                    break;
                case TouchPhase.Canceled:
                    mode = TouchMode.None;
                    ResetJoystick();
                    break;
            }
        }

    }

    void ResetJoystick() {
        virtualJoystick.SetActive(false);
        virtualJoystick.transform.position = Vector3.zero;
        joystickVector = Vector2.zero;
        joystickPosition = Vector2.zero;
        
    }
    void MoveObject(Vector2 move) {
        slave.transform.position += (Vector3)(move * Time.deltaTime * joystickSpeed);
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(slave.transform.position, itemDetectionDistance);
    }
}
