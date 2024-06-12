using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverAnim : MonoBehaviour
{
    private Animator _animator;
    private InputMaster m_InputMaster;

    public List<GameObject> movingPieces;
    private List<Animator> _movingPieceAnimator;

    private bool _collided;

    private void Awake()
    {
        m_InputMaster = InputMaster.Instance;
        m_InputMaster.InputAction.Jellys.Interact.performed += (ctx) => OnInteract();

    }

    void Start()
    {
        _animator = GetComponent<Animator>();
        foreach (var piece in movingPieces)
        {
            _movingPieceAnimator.Add(piece.GetComponent<Animator>());

        }
    }

    public void OnInteract()
    {
        if (!_collided) {
            Debug.Log($"collided : {_collided}");
            return;
        }

        bool leverState = _animator.GetBool("bLever");
        _animator.SetTrigger(leverState ? "trLeft" : "trRight");
        _animator.SetBool("bLever", !leverState);

        bool WallState = _movingPieceAnimator.GetBool("bOperate");
        _movingPieceAnimator.SetTrigger(WallState ? "trDown" : "trUp");
        _movingPieceAnimator.SetBool("bOperate", !WallState);

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
