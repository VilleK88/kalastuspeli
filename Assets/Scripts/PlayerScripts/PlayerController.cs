using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputActionAsset inputActions;

    private InputAction m_moveAction;
    private InputAction m_jumpAction;
    private InputAction m_fishingAction;
    private InputAction m_stopFishingAction;
    private InputAction m_fishingCastAction;

    private InputAction m_pauseactionPlayer;
    private InputAction m_pauseActionUI;

    private Vector2 m_moveAmt;
    private Animator m_animator;
    private Rigidbody m_rigidbody;

    public float walkSpeed = 5;
    public float rotateSpeed = 5;
    public float jumpSpeed = 5;

    public GameObject pauseDisplay;

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
        m_fishingCastAction = InputSystem.actions.FindAction("FishingRodCast");

        m_pauseactionPlayer = InputSystem.actions.FindAction("Player/Pause");
        m_pauseActionUI = InputSystem.actions.FindAction("UI/Pause");

        m_animator = GetComponent<Animator>();
        m_rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Fishing();

        m_moveAmt = m_moveAction.ReadValue<Vector2>();

        if (m_jumpAction.WasPressedThisFrame())
        {
            Jump();
        }

        DisplayPause();
    }

    private void FixedUpdate()
    {
        Walking();
    }

    private void Fishing()
    {
        if(m_fishingAction.WasPressedThisFrame())
        {
            m_animator.SetBool("Fishing", true);
            inputActions.FindActionMap("Player").Disable();
            inputActions.FindActionMap("FishingMode").Enable();
        }
        else if(m_stopFishingAction.WasPressedThisFrame())
        {
            m_animator.SetBool("Fishing", false);
            inputActions.FindActionMap("FishingMode").Disable();
            StartCoroutine(StopFishing());
        }
    }

    IEnumerator StopFishing()
    {
        yield return new WaitForSeconds(1.5f);
        inputActions.FindActionMap("Player").Enable();
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