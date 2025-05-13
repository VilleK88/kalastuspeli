using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Camera thirdPersonCamera;
    public Camera firstPersonCamera;

    public InputActionAsset inputActions;

    private InputAction m_moveAction;
    private InputAction m_jumpAction;
    private InputAction m_fishingAction;

    private InputAction m_stopFishingAction;
    private InputAction m_throwBaitAction;
    private InputAction m_reelInAction;

    private InputAction m_pauseactionPlayer;
    private InputAction m_pauseActionUI;

    private Vector2 m_moveAmt;
    private Animator m_animator;
    private Rigidbody m_rigidbody;

    public float walkSpeed = 5;
    public float rotateSpeed = 5;
    public float jumpSpeed = 5;

    public GameObject pauseDisplay;

    public GameObject baitPrefab;
    public Transform castPoint;
    public float maxCastForce = 20f;
    public float chargeSpeed = 10f;

    private float currentForce = 0f;
    private bool isCharging = false;
    private float chargeStartTime = 0f;

    public BaitSO[] baits;
    public int selectedBaitIndex = 0;

    public GameObject currentBait;
    public bool isReeling = false;
    private float reelSpeed = 12f;

    public LineRenderer fishingLine;

    private void OnEnable()
    {
        inputActions.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        inputActions.FindActionMap("Player").Disable();
    }

    private void Awake()
    {
        m_moveAction = InputSystem.actions.FindAction("Move");
        m_jumpAction = InputSystem.actions.FindAction("Jump");
        m_fishingAction = InputSystem.actions.FindAction("Player/Fishing");

        m_stopFishingAction = InputSystem.actions.FindAction("FishingMode/Fishing");
        m_throwBaitAction = InputSystem.actions.FindAction("ThrowLure");
        m_reelInAction = InputSystem.actions.FindAction("FishingMode/ReelIn");

        m_pauseactionPlayer = InputSystem.actions.FindAction("Player/Pause");
        m_pauseActionUI = InputSystem.actions.FindAction("UI/Pause");

        m_animator = GetComponent<Animator>();
        m_rigidbody = GetComponent<Rigidbody>();

        m_throwBaitAction.started += ctx => StartCharging();
        m_throwBaitAction.canceled += ctx => ReleaseCast();

        //m_reelInAction.performed += ctx => StartReeling();
    }

    private void Update()
    {
        Fishing();

        m_moveAmt = m_moveAction.ReadValue<Vector2>();

        if (m_jumpAction.WasPressedThisFrame())
        {
            Jump();
        }

        if(isCharging)
        {
            currentForce += chargeSpeed * Time.deltaTime;
            currentForce = Mathf.Clamp(currentForce, 0f, maxCastForce);
            ThrowPowerUI.Instance.UpdatePowerBar(currentForce, maxCastForce);
        }
        else
        {
            ThrowPowerUI.Instance.ResetPowerBar();
        }

        isReeling = m_reelInAction.ReadValue<float>() > 0.5f;

            DisplayPause();
    }

    private void FixedUpdate()
    {
        Walking();
    }

    private void StartCharging()
    {
        if (!m_animator.GetBool("Fishing")) return;

        isCharging = true;
        currentForce = 0f;
    }

    private void ReleaseCast()
    {
        if (!isCharging) return;

        ThrowLure();
        isCharging = false;
    }

    private void Fishing()
    {
        if(m_fishingAction.WasPressedThisFrame())
        {
            m_animator.SetBool("Fishing", true);
            inputActions.FindActionMap("Player").Disable();
            inputActions.FindActionMap("FishingMode").Enable();
            SwitchToFirstPersonCamera();

        }
        else if(m_stopFishingAction.WasPressedThisFrame())
        {
            m_animator.SetBool("Fishing", false);
            inputActions.FindActionMap("FishingMode").Disable();
            StartCoroutine(StopFishing());
        }

        UpdateReeling();
        UpdateFishingLine();
    }

    void SwitchToThirdPersonCamera()
    {
        firstPersonCamera.gameObject.SetActive(false);
        thirdPersonCamera.gameObject.SetActive(true);
        Debug.Log("Switch to third person camera");
    }

    void SwitchToFirstPersonCamera()
    {
        thirdPersonCamera.gameObject.SetActive(false);
        firstPersonCamera.gameObject.SetActive(true);
        Debug.Log("Switch to first person camera");
    }

    void ThrowLure()
    {
        GameObject prefabToThrow = baits[selectedBaitIndex].prefab;
        GameObject lure = Instantiate(prefabToThrow, castPoint.position, castPoint.rotation);
        Rigidbody rb = lure.GetComponent<Rigidbody>();
        currentBait = lure;
        rb.AddForce(castPoint.forward * currentForce, ForceMode.Impulse);
        isReeling = false;
    }

    void StartReeling()
    {
        if(currentBait != null)
        {
            Rigidbody rb = currentBait.GetComponent<Rigidbody>();
            if(rb != null)
            {
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }
        }
        isReeling = true;
    }

    void UpdateReeling()
    {
        if (!isReeling || currentBait == null) return;

        currentBait.transform.position = Vector3.MoveTowards(
            currentBait.transform.position,
            castPoint.position, reelSpeed * Time.deltaTime);

        if(Vector3.Distance(currentBait.transform.position, castPoint.position) < 0.2f)
        {
            isReeling = false;
            Destroy(currentBait);
            currentBait = null;
        }
    }

    void UpdateFishingLine()
    {
        if(currentBait != null)
        {
            fishingLine.enabled = true;
            fishingLine.SetPosition(0, castPoint.position);
            fishingLine.SetPosition(1, currentBait.transform.position);
        }
        else
        {
            fishingLine.enabled = false;
        }
    }

    IEnumerator StopFishing()
    {
        yield return new WaitForSeconds(1.5f);
        inputActions.FindActionMap("Player").Enable();
        SwitchToThirdPersonCamera();
    }

    private void DisplayPause()
    {
        if(m_pauseactionPlayer.WasPressedThisFrame())
        {
            pauseDisplay.SetActive(true);
            inputActions.FindActionMap("Player").Disable();
            inputActions.FindActionMap("UI").Enable();
        }
        else if(m_pauseActionUI.WasPressedThisFrame())
        {
            pauseDisplay.SetActive(false);
            inputActions.FindActionMap("UI").Disable();
            inputActions.FindActionMap("Player").Enable();
        }
    }

    public void Jump()
    {
        m_rigidbody.AddForceAtPosition(new Vector3(0, 5f, 0), Vector3.up, ForceMode.Impulse);
        m_animator.SetTrigger("Jump");
    }

    private void Walking()
    {
        // Kävely animaatio päivittyy eteen/taakse
        m_animator.SetFloat("Speed", m_moveAmt.y);

        // Liike eteen/taakse
        m_rigidbody.MovePosition(m_rigidbody.position + transform.forward * m_moveAmt.y * walkSpeed * Time.deltaTime);

        // Rotaatio
        if(Mathf.Abs(m_moveAmt.x) > 0.1f)
        {
            Quaternion turnOffset = Quaternion.Euler(0, m_moveAmt.x * rotateSpeed, 0);
            m_rigidbody.MoveRotation(m_rigidbody.rotation * turnOffset);
        }
    }
}