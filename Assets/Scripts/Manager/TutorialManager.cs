using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public enum TutorialStep
{
    Intro,
    Hold,
    Drag,
    Tilt,
    Success,
    Timer,
    Finished
}

public class TutorialManager : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement player;
    public UIManager uiManager;
    public TrayBalancer trayBalancer;

    [Header("Tutorial UI")]
    public GameObject tutorialCanvas;
    public CanvasGroup tutorialPanel;

    public TMP_Text tutorialText;
    public TMP_Text continueText;
    public TMP_Text instructionText;

    [Header("Hint Images")]
    public GameObject holdingPhone;
    public GameObject circleFX;
    public GameObject leftArrow;
    public GameObject rightArrow;
    public GameObject pointingHand;

    [Header("Success")]
    public TMP_Text successText;

    [Header("Animation Settings")]
    public RectTransform circleFXTransform;
    public RectTransform pointingHandTransform;
    public RectTransform holdingPhoneTransform;

    public float pulseSpeed = 1.5f;
    public float handMoveDistance = 50f;
    public float handMoveSpeed = 1f;

    public float phoneTiltAngle = 15f;

    private Coroutine circleRoutine;
    private Coroutine handRoutine;
    private Coroutine phoneRoutine;

    private TutorialStep currentStep;

    private float holdTimer;

    private bool draggedLeft;
    private bool draggedRight;

    private bool tiltedLeft;
    private bool tiltedRight;
    private bool tiltedForward;
    private bool tiltedBackward;
    public void BeginTutorial()
    {
        tutorialCanvas.SetActive(true);

        uiManager.SetPauseButton(false);
        uiManager.SetTimerVisible(false);

        ShowIntro();
    }

    void Update()
    {
        switch (currentStep)
        {
            case TutorialStep.Intro:
                if (TapDetected())
                {
                    Debug.Log("Tapped!");
                    ShowHoldInstruction();
                }
                break;

            case TutorialStep.Hold:
                UpdateHold();
                break;

            case TutorialStep.Drag:
                UpdateDrag();
                break;

            case TutorialStep.Timer:
                if (TapDetected())
                {
                    FinishTutorial();
                }
                break;
            case TutorialStep.Tilt:

                UpdateTilt();
                break;
            case TutorialStep.Success:

                if (TapDetected())
                {
                    successText.gameObject.SetActive(false);
                    continueText.gameObject.SetActive(false);

                    nextTutorialStep?.Invoke();
                }

                break;
        }
    }

    private bool TapDetected()
    {
        bool touchPressed = Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame;
        bool mousePressed = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;

        return touchPressed || mousePressed;
    }

    void ShowIntro()
    {
        currentStep = TutorialStep.Intro;

        tutorialPanel.gameObject.SetActive(true);
        tutorialText.text = "Welcome!\nDeliver the items safely to the finish without dropping them.";
        continueText.text = "Tap anywhere to continue";
        continueText.gameObject.SetActive(true);
        instructionText.gameObject.SetActive(false);
        player.tutorialMovementEnabled = false;

        HideAllHints();
    }

    void ShowHoldInstruction()
    {
        currentStep = TutorialStep.Hold;
        

        tutorialPanel.gameObject.SetActive(false);
        tutorialText.gameObject.SetActive(false);
        continueText.gameObject.SetActive(false);
        instructionText.gameObject.SetActive(true);
        instructionText.text =
            "Hold anywhere on the screen\nto move forward.";

        //holdingPhone.SetActive(true);
        circleFX.SetActive(true);
        circleRoutine = StartCoroutine(PulseCircle());

        holdTimer = 0f;
        StartCoroutine(EnableMovementAfterDelay());
    }

    void UpdateHold()
    {
        bool isTouching = Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed;
        bool isMouseDown = Mouse.current != null && Mouse.current.leftButton.isPressed;

        if (isTouching || isMouseDown)
        {
            holdTimer += Time.deltaTime;

            if (holdTimer >= 3f)
            {
                Debug.Log("Hold Success!");

                // Lock step temporarily so UpdateHold isn't called again during transition
                continueText.gameObject.SetActive(true);
                currentStep = TutorialStep.Success;

                holdingPhone.SetActive(false);
                StopCircleAnimation();
                circleFX.SetActive(false);
                

                // Transition to Drag instruction AFTER the success text finishes
                ShowSuccessRoutine("Great!", ShowDragInstruction);
                instructionText.gameObject.SetActive(false);
            }
        }
        else
        {
            holdTimer = 0f;
        }
    }

    void ShowDragInstruction()
    {
        currentStep = TutorialStep.Drag;

        instructionText.gameObject.SetActive(true);
        instructionText.text =
            "Drag left & right\nto avoid obstacles.";

        //holdingPhone.SetActive(false);
        circleFX.SetActive(false);
        continueText.gameObject.SetActive(false);

        leftArrow.SetActive(true);
        rightArrow.SetActive(true);
        pointingHand.SetActive(true);
        handRoutine = StartCoroutine(MoveHand());

        draggedLeft = false;
        draggedRight = false;
    }

    void UpdateDrag()
    {
        float deltaX = 0f;

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            deltaX = Touchscreen.current.primaryTouch.delta.ReadValue().x;
        }
        else if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            deltaX = Mouse.current.delta.ReadValue().x;
        }

        if (deltaX < -3f) draggedLeft = true;
        if (deltaX > 3f) draggedRight = true;

        if (draggedLeft && draggedRight)
        {
            Debug.Log("Drag Success!");

            currentStep = TutorialStep.Success;

            leftArrow.SetActive(false);
            rightArrow.SetActive(false);
            pointingHand.SetActive(false);
            StopHandAnimation();

            // Correct transition to Timer step
            ShowSuccessRoutine("Nice!", ShowTiltInstruction);
            instructionText.gameObject.SetActive(false);
            continueText.gameObject.SetActive(true);
        }
    }
    void ShowTiltInstruction()
    {
        trayBalancer.tutorialTiltEnabled = true;
        currentStep = TutorialStep.Tilt;

        tiltedLeft = false;
        tiltedRight = false;
        tiltedForward = false;
        tiltedBackward = false;

        HideAllHints();
        continueText.gameObject.SetActive(false);

        instructionText.gameObject.SetActive(true);
        instructionText.text =
            "Tilt your phone\nto balance the items.";

        holdingPhone.SetActive(true);
        phoneRoutine = StartCoroutine(TiltPhone());

        tutorialPanel.gameObject.SetActive(true);

        tutorialText.text =
            "Balancing is just as important as moving.";

        continueText.text = "Tap anywhere to continue";
    }

    void UpdateTilt()
    {
        Vector3 accel = Vector3.zero;

        if (Accelerometer.current != null)
        {
            accel = Accelerometer.current.acceleration.ReadValue();
        }

        else if (Keyboard.current != null)
        {
            // Editor testing
            if (Keyboard.current.aKey.isPressed)
                tiltedLeft = true;

            if (Keyboard.current.dKey.isPressed)
                tiltedRight = true;

            if (Keyboard.current.wKey.isPressed)
                tiltedForward = true;

            if (Keyboard.current.sKey.isPressed)
                tiltedBackward = true;
        }

        if (accel.x < -0.25f)
            tiltedLeft = true;

        if (accel.x > 0.25f)
            tiltedRight = true;

        if (accel.y < -0.25f)
            tiltedBackward = true;

        if (accel.y > 0.25f)
            tiltedForward = true;

        if (tiltedLeft &&
            tiltedRight &&
            tiltedForward &&
            tiltedBackward)
        {
            currentStep = TutorialStep.Success;

            ShowSuccessRoutine("Perfect!", ShowTimerInstruction); 
            instructionText.gameObject.SetActive(false);
            continueText.gameObject.SetActive(true);
            StopPhoneAnimation();
            holdingPhone.SetActive(false);
        }
    }

    void ShowTimerInstruction()
    {
        trayBalancer.tutorialTiltEnabled = false;
        player.tutorialMovementEnabled = false;
        currentStep = TutorialStep.Timer;
        uiManager.SetTimerVisible(true);

        HideAllHints();

        instructionText.gameObject.SetActive(false);

        tutorialPanel.gameObject.SetActive(true);
        tutorialText.gameObject.SetActive(true);
        continueText.gameObject.SetActive(true);

        tutorialText.text =
            "\nYou're good to go! Race to the end before time runs out!";
        StartCoroutine(BounceText(tutorialText));

        continueText.text = "Tap anywhere to start";
    }

    void FinishTutorial()
    {
        currentStep = TutorialStep.Finished;
        tutorialCanvas.SetActive(false);
        uiManager.SetPauseButton(true);

        GameManager.Instance.BeginGameplay();
    }

    void HideAllHints()
    {
        StopCircleAnimation();
        StopHandAnimation();
        StopPhoneAnimation();

        holdingPhone.SetActive(false);
        circleFX.SetActive(false);
        leftArrow.SetActive(false);
        rightArrow.SetActive(false);
        pointingHand.SetActive(false);
    }

    private System.Action nextTutorialStep;

    private void ShowSuccessRoutine(string text, System.Action nextStep)
    {
        currentStep = TutorialStep.Success;

        successText.gameObject.SetActive(true);
        successText.text = text;

        StartCoroutine(BounceText(successText));

        continueText.gameObject.SetActive(true);
        continueText.text = "Tap anywhere to continue";

        nextTutorialStep = nextStep;
    }

    private IEnumerator EnableMovementAfterDelay()
    {
        player.tutorialMovementEnabled = false;

        yield return new WaitForSeconds(1f);

        player.tutorialMovementEnabled = true;
    }

    IEnumerator PulseCircle()
    {
        Vector3 originalScale = circleFXTransform.localScale;

        while (true)
        {
            float scale = Mathf.Lerp(
                1f,
                1.3f,
                (Mathf.Sin(Time.time * pulseSpeed) + 1) / 2
            );

            circleFXTransform.localScale =
                originalScale * scale;

            yield return null;
        }
    }

    void StopCircleAnimation()
    {
        if (circleRoutine != null)
        {
            StopCoroutine(circleRoutine);
            circleRoutine = null;
        }

        circleFXTransform.localScale = Vector3.one;
    }

    IEnumerator MoveHand()
    {
        Vector3 startPos = pointingHandTransform.localPosition;

        while (true)
        {
            Vector3 target =
                startPos + Vector3.right * handMoveDistance;


            while (Vector3.Distance(
                pointingHandTransform.localPosition,
                target) > 0.1f)
            {
                pointingHandTransform.localPosition =
                    Vector3.Lerp(
                        pointingHandTransform.localPosition,
                        target,
                        Time.deltaTime * handMoveSpeed
                    );

                yield return null;
            }


            while (Vector3.Distance(
                pointingHandTransform.localPosition,
                startPos) > 0.1f)
            {
                pointingHandTransform.localPosition =
                    Vector3.Lerp(
                        pointingHandTransform.localPosition,
                        startPos,
                        Time.deltaTime * handMoveSpeed
                    );

                yield return null;
            }
        }
    }

    void StopHandAnimation()
    {
        if (handRoutine != null)
        {
            StopCoroutine(handRoutine);
            handRoutine = null;
        }
    }

    IEnumerator TiltPhone()
    {
        while (true)
        {
            yield return RotatePhone(-phoneTiltAngle);

            yield return RotatePhone(phoneTiltAngle);

            yield return RotatePhone(0);
        }
    }


    IEnumerator RotatePhone(float targetAngle)
    {
        Quaternion start =
            holdingPhoneTransform.localRotation;

        Quaternion end =
            Quaternion.Euler(0, 0, targetAngle);


        float timer = 0;

        while (timer < 1)
        {
            timer += Time.deltaTime;

            holdingPhoneTransform.localRotation =
                Quaternion.Lerp(
                    start,
                    end,
                    timer
                );

            yield return null;
        }
    }

    void StopPhoneAnimation()
    {
        if (phoneRoutine != null)
        {
            StopCoroutine(phoneRoutine);
            phoneRoutine = null;
        }

        holdingPhoneTransform.localRotation =
            Quaternion.identity;
    }

    IEnumerator BounceText(TMP_Text text)
    {
        RectTransform rect =
            text.GetComponent<RectTransform>();

        rect.localScale = Vector3.zero;


        float timer = 0;

        while (timer < 1)
        {
            timer += Time.deltaTime * 5f;

            float scale =
                Mathf.Sin(timer * Mathf.PI * 0.5f);

            rect.localScale =
                Vector3.one * scale;

            yield return null;
        }


        rect.localScale = Vector3.one;
    }
}