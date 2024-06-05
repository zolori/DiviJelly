using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverAnim : MonoBehaviour
{
    private Animator _animator;
    private InputMaster m_InputMaster;

    public GameObject movingPiece;
    private Animator _movingPieceAnimator;

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
        if (!_collided)
            return;

        bool leverState = _animator.GetBool("bLever");

        _animator.SetTrigger(leverState ? "trLeft" : "trRight");
        _animator.SetBool("bLever", !leverState);

        _movingPieceAnimator.SetTrigger("trOperate");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _collided = true;
    }
}
