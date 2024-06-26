using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverAnim : MonoBehaviour
{
    private Animator _animator;
    private InputMaster m_InputMaster;

    public GameObject movingPiece;
    private Animator _movingPieceAnimator;

    bool leverState = false;
    bool PieceState = false;

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
            Debug.Log($"pas collided");
            return;
        }

        _animator.SetTrigger(leverState ? "trOn" : "trOff");
        leverState = !leverState;

        _movingPieceAnimator.SetTrigger(PieceState ? "trOff" : "trOn");
        PieceState = !PieceState;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _collided = true;
        Debug.Log($"je rentre dans la trigger zone : {_collided}");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _collided = false;
        Debug.Log($"je sors de la trigger zone : {_collided}");
    }
}
