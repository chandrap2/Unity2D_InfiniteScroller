using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    public Transform gCheckTransform;
    public LayerMask gLayer;

    NewActions na;
    
    private ContactFilter2D collFilter;
    private RaycastHit2D[] collResults;
    [SerializeField] private Vector2 gCheckDim;

    private Rigidbody2D rg;

    private int animParamID;
    private Animator animController;
 
    private enum PlayerState {
        RUNNING, JUMPING
    };

    private PlayerState currState = PlayerState.JUMPING;

    // Start is called before the first frame update
    void Start()
    {
        na = new();
        na.Player.Jump.performed += Jump;
        na.Player.PlayerReset.performed += ResetPos;
        na.Player.Dash.performed += Dash;
        na.Enable();

        collResults = new RaycastHit2D[2];
        collFilter = new();
        collFilter.SetLayerMask(gLayer);
        collFilter.SetNormalAngle(89, 91);

        rg = gameObject.GetComponent<Rigidbody2D>();

        animController = gameObject.GetComponent<Animator>();
        animParamID = Animator.StringToHash("playerAnimState");
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (currState == PlayerState.JUMPING &&
            Physics2D.BoxCast(gCheckTransform.position, gCheckDim, 0, Vector2.up, collFilter, collResults, 0) > 0) {

            SetPlayerState(PlayerState.RUNNING);
        }
    }

    //private void OnCollisionExit2D(Collision2D collision) {
    //    currState = PlayerState.JUMPING;
    //}

    private void ResetPos(InputAction.CallbackContext obj) {
        gameObject.transform.position = Vector2.up * 4;
        rg.velocity = Vector2.zero;
        SetPlayerState(PlayerState.JUMPING);
    }

    private void Dash(InputAction.CallbackContext obj) {
        rg.AddForce(Vector2.right * 2f, ForceMode2D.Impulse);
    }

    private void Jump(InputAction.CallbackContext obj) {
        if (currState == PlayerState.RUNNING) {

            int numHits = Physics2D.BoxCast(gCheckTransform.position, gCheckDim, 0, Vector2.up, collFilter, collResults, 0);

            if (numHits > 0) {
                SetPlayerState(PlayerState.JUMPING);
                rg.AddForce(Vector2.up * 7f, ForceMode2D.Impulse);
            }
        }
    }

    private void SetPlayerState(PlayerState st) {
        currState = st;
        animController.SetInteger(animParamID, (int) st);
    }
}
