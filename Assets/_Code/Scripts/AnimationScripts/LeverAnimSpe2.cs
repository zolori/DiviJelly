using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverAnimSpe2 : MonoBehaviour
{
    private Animator _animator;
    private InputMaster m_InputMaster;

    public GameObject movingPiece1;
    public GameObject movingPiece2;
    private Animator _movingPieceAnimator1;
    private Animator _movingPieceAnimator2;

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
        _movingPieceAnimator1 = movingPiece1.GetComponent<Animator>();
        _movingPieceAnimator2 = movingPiece2.GetComponent<Animator>();
    }

    public void OnInteract()
    {
        if (!_collided) {
            Debug.Log($"collided : {_collided}");
            return;
        }

        bool platformState1 = _movingPieceAnimator1.GetBool("bState");
        bool platformState2 = _movingPieceAnimator2.GetBool("bState");

        _animator.SetTrigger(leverState ? "trOn" : "trOff");
        leverState = !leverState;

        _movingPieceAnimator1.SetTrigger(platformState1 ? "trOff" : "trOn");
        _movingPieceAnimator1.SetBool("bState", !platformState1);

        _movingPieceAnimator2.SetTrigger(platformState2 ? "trOff" : "trOn");
        _movingPieceAnimator2.SetBool("bState", !platformState2);

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
