using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public enum TutorialStep
{
    Intro,
    Hold,
    Drag,
    Timer,
    Finished
}

public class TutorialManager : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement player;
    public UIManager uiManager;

    [Header("Tutorial UI")]
    public GameObject tutorialCanvas;
    public CanvasGroup tutorialPanel;

    public TMP_Text tutorialText;
    public TMP_Text continueText;

    [Header("Hint Images")]
    public GameObject holdingPhone;
    public GameObject circleFX;
    public GameObject leftArrow;
    public GameObject rightArrow;
    public GameObject pointingHand;

    [Header("Success")]
    public TMP_Text successText;

    private TutorialStep currentStep;

    private float holdTimer;

    private bool draggedLeft;
    private bool draggedRight;

    public void BeginTutorial()
    {
        tutorialCanvas.SetActive(true);
        uiManager.SetPauseButton(false);
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

        HideAllHints();
    }

    void ShowHoldInstruction()
    {
        currentStep = TutorialStep.Hold;

        tutorialPanel.gameObject.SetActive(false);
        tutorialText.text = "";
        continueText.text = "";

        holdingPhone.SetActive(true);
        circleFX.SetActive(true);

        holdTimer = 0f;
    }

    void UpdateHold()
    {
        bool isTouching = Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed;
        bool isMouseDown = Mouse.current != null && Mouse.current.leftButton.isPressed;

        if (isTouching || isMouseDown)
        {
            holdTimer += Time.deltaTime;

            if (holdTimer >= 0.5f)
            {
                Debug.Log("Hold Success!");

                // Lock step temporarily so UpdateHold isn't called again during transition
                currentStep = TutorialStep.Finished;

                holdingPhone.SetActive(false);
                circleFX.SetActive(false);

                // Transition to Drag instruction AFTER the success text finishes
                StartCoroutine(ShowSuccessRoutine("Great!", ShowDragInstruction));
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

        holdingPhone.SetActive(false);
        circleFX.SetActive(false);

        leftArrow.SetActive(true);
        rightArrow.SetActive(true);
        pointingHand.SetActive(true);

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

            currentStep = TutorialStep.Finished;

            leftArrow.SetActive(false);
            rightArrow.SetActive(false);
            pointingHand.SetActive(false);

            // Correct transition to Timer step
            StartCoroutine(ShowSuccessRoutine("Great!", ShowTimerInstruction));
        }
    }

    void ShowTimerInstruction()
    {
        currentStep = TutorialStep.Timer;

        HideAllHints();

        tutorialPanel.gameObject.SetActive(true);
        tutorialText.text = "You're good to go!\nKeep an eye on the timer at the top.";
        continueText.text = "Tap anywhere to start";
        continueText.gameObject.SetActive(true);
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
        holdingPhone.SetActive(false);
        circleFX.SetActive(false);
        leftArrow.SetActive(false);
        rightArrow.SetActive(false);
        pointingHand.SetActive(false);
    }

    private IEnumerator ShowSuccessRoutine(string text, System.Action nextStep)
    {
        successText.gameObject.SetActive(true);
        successText.text = text;

        yield return new WaitForSeconds(0.8f);

        successText.gameObject.SetActive(false);
        nextStep?.Invoke();
    }
}