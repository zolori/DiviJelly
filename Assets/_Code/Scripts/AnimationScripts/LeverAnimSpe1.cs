using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverAnimSpe1 : MonoBehaviour
{
    private Animator _animator;
    private InputMaster m_InputMaster;

    public GameObject movingPiece;
    private Animator _movingPieceAnimator;

    bool leverState = false;

    private bool _collided;

    private void Awake()
    {
        m_InputMaster = InputMaster.Instance;
        m_InputMaster.InputAction.Jellys.Interact.performed += (ctx) => OnInteract();

    }

    void Start()
    {
        _animator = GetComponent<Animator>();
        _movingPieceAnimator = movingPiece.GetComponent<Animator>();
    }

    public void OnInteract()
    {
        if (!_collided) {
            Debug.Log($"collided : {_collided}");
            return;
        }

        _animator.SetTrigger(leverState ? "trOn" : "trOff");
        leverState = !leverState;

        bool platformState = _movingPieceAnimator.GetBool("bState");
        _movingPieceAnimator.SetTrigger(platformState ? "trOff" : "trOn");
        _movingPieceAnimator.SetBool("bState", !platformState);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _collided = true;
        Debug.Log($"collided : {_collided}");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _collided = false;
        Debug.Log($"collided : {_collided}");
    }
}
