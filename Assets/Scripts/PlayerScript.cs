using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    public UIScript ui;
    public MenuScript menu;

    // Private variables
    private Vector2 input;
    private Vector3 moveDirection;
    CharacterController cc;
    Animator animator;
    private bool isSlow = false;
    private float gravityScale = 4f;
    private float turnCalmTime = 0.05f;

    //Ground Detection
    private float startTime;
    public float duration = 1f;
    private bool counterStarted = false;
    public bool isGrounded = false;
    public RaycastHit hit;                                  
    public float GroundRaycastDist = 0.2f;                       
    public Vector3 direction = new Vector3(0f, -1f, 0f);

    [Header("Configurables")]
    public float walkSpeed;
    public float runSpeed;
    public float gunWalkSpeed;
    public float gunRunSpeed;
    public float playerHealth;
    public float currHealth;
    public float jumpForce = 1f;

    [Header("Player")]
    public Transform playerCamera;
    public GameObject playerGun;
    public GameObject flashCanvas;

    [Header("Sound")]
    public AudioSource normalAudioSource;
    public AudioSource slowAudioSource;
    public AudioSource endSource;
    public AudioClip normalFootstep;
    public AudioClip slowFootStep;
    public AudioClip overSound;

    // Private default values
    private float walkSpeedStore;
    private float runSpeedStore;
    private float gunWalkSpeedStore;
    private float gunRunSpeedStore;

    /***
     * public GameObject buttonPressStandingSpot;
    public GameObject buttonObject;
    public GameObject buttonObject2;
    public float buttonCloseEnoughForMatchDistance = 2f;
    public float buttonCloseEnoughForPressDistance = 0.22f;
    public float buttonCloseEnoughForPressAngleDegrees = 5f;
    ***/

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        currHealth = playerHealth;
        ui.SetFullHealth(playerHealth);

        walkSpeedStore = walkSpeed;
        runSpeedStore = runSpeed;
        gunWalkSpeedStore = gunWalkSpeed;
        gunRunSpeedStore = gunRunSpeed;
    }

    /***
    void FixedUpdate()
    {
        bool doButtonPress = false;
        bool doMatchToButtonPress = false;       

        float buttonDistance = float.MaxValue;
        float buttonAngleDegrees = float.MaxValue;

        if (buttonPressStandingSpot != null)
        {
            buttonDistance = Vector3.Distance(transform.position, buttonPressStandingSpot.transform.position);
            buttonAngleDegrees = Quaternion.Angle(transform.rotation, buttonPressStandingSpot.transform.rotation);
        }

        if (_inputActionFired)
        {
            _inputActionFired = false; // clear the input event that came from Update()

            Debug.Log("Action pressed");

            if (buttonDistance <= buttonCloseEnoughForMatchDistance)
            {
                if (buttonDistance <= buttonCloseEnoughForPressDistance &&
                    buttonAngleDegrees <= buttonCloseEnoughForPressAngleDegrees)
                {
                    Debug.Log("Button press initiated");

                    doButtonPress = true;

                }
                else
                {
                    // TODO UNCOMMENT THESE LINES FOR TARGET MATCHING
                    Debug.Log("match to button initiated");
                    doMatchToButtonPress = true;
                }

            }
        }
        
        // get info about current animation
        var animState = anim.GetCurrentAnimatorStateInfo(0);

        // If the transition to button press has been initiated then we want
        // to correct the character position to the correct place
        if (animState.IsName("MatchToButtonPress") && !anim.IsInTransition(0) && !anim.isMatchingTarget)
        {
            if (buttonPressStandingSpot != null)
            {
                Debug.Log("Target matching correction started");

                initalMatchTargetsAnimTime = animState.normalizedTime;

                var t = buttonPressStandingSpot.transform;
                anim.MatchTarget(t.position, t.rotation, AvatarTarget.Root, new MatchTargetWeightMask(new Vector3(1f, 0f, 1f), 1f), initalMatchTargetsAnimTime, exitMatchTargetsAnimTime);
            }
        }
     
        anim.SetBool("doButtonPress", doButtonPress);
        anim.SetBool("matchToButtonPress", doMatchToButtonPress);
        
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (anim)
        {
            AnimatorStateInfo astate = anim.GetCurrentAnimatorStateInfo(0);

            if (astate.IsName("ButtonPress"))
            {
                float buttonWeight = anim.GetFloat("buttonClose");

                // Set the look target position, if one has been assigned
                if (buttonObject != null)
                {
                    anim.SetLookAtWeight(buttonWeight);
                    anim.SetLookAtPosition(buttonObject.transform.position);
                    anim.SetIKPositionWeight(AvatarIKGoal.RightHand, buttonWeight);
                    anim.SetIKPosition(AvatarIKGoal.RightHand, buttonObject.transform.position);
                }
            }
            else
            {
                anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                anim.SetLookAtWeight(0);
            }
        }
    }
    ***/

    IEnumerator FlashScreen () {
        yield return new WaitForSeconds (.3f);
        if (!flashCanvas.activeSelf) {
            flashCanvas.SetActive (true);
        }
        Image img = flashCanvas.GetComponentInChildren<Image>();
        for (float i = 0.8f; i >= 0; i -= Time.deltaTime)
        {
            img.color = new Color(255, 9, 9, i);
            if (i==0.0f){
                flashCanvas.SetActive(false);
            }
            yield return null;
        }
     }

    // Update is called once per frame
    void Update()
    {
        // Set direction for animation blend tree
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");
        animator.SetFloat("InputX", input.x, turnCalmTime, Time.deltaTime);
        animator.SetFloat("InputY", input.y, turnCalmTime, Time.deltaTime);

        if (IsGrounded() && Input.GetButtonDown("Jump"))
        {
            animator.SetBool("isTakeOff", true);
        }
        if (IsGrounded() == false)
        {
            animator.SetBool("isTakeOff", false);
            animator.SetBool("IsFalling", true);
        }
        if (IsGrounded())
        {

            animator.SetBool("IsFalling", false);
            animator.SetBool("isJumping", false);
        }
        else
        {
            animator.SetBool("isJumping", true);
        }

        if (playerGun.activeSelf)
        {
            WithGunMotion();
        }
        else
        {
            NoGunMotion();
        }
    }

    void NoGunMotion()
    {
        if (Input.GetButton("Run"))
        {
            animator.SetBool("Walk", false);
            animator.SetBool("Run", true);
            animator.SetBool("GunWalk", false);
            animator.SetBool("GunRun", false);

            MovePlayer(runSpeed);

        }
        else
        {
            animator.SetBool("Walk", true);
            animator.SetBool("Run", false);
            animator.SetBool("GunWalk", false);
            animator.SetBool("GunRun", false);

            MovePlayer(walkSpeed);
        }
    }

    void WithGunMotion()
    {
        if (Input.GetButton("Run"))
        {
            animator.SetBool("Walk", false);
            animator.SetBool("Run", false);
            animator.SetBool("GunWalk", false);
            animator.SetBool("GunRun", true);

            MovePlayer(gunRunSpeed);
        }
        else
        {
            animator.SetBool("Walk", false);
            animator.SetBool("Run", false);
            animator.SetBool("GunWalk", true);
            animator.SetBool("GunRun", false);

            MovePlayer(gunWalkSpeed);
        }
    }

    private void MovePlayer(float speed)
    {
        if (!IsGrounded())
        {
            moveDirection.y += (Physics.gravity.y * gravityScale * Time.deltaTime);
            cc.Move(moveDirection * Time.deltaTime);
        }
        if (currHealth > 0)
        {
            float h_axis = Input.GetAxisRaw("Horizontal");
            float v_axis = Input.GetAxisRaw("Vertical");
            Vector3 direction = new Vector3(h_axis, 0f, v_axis).normalized;
            if (IsGrounded() && Input.GetButtonDown("Jump"))
            {
                moveDirection.y = jumpForce;
                cc.Move(moveDirection * Time.deltaTime);
            }
            if (direction.magnitude >= 0.1f)
            {
                float yStore = moveDirection.y;
                moveDirection = (transform.forward * input.y) + (transform.right * input.x);
                moveDirection = moveDirection.normalized * speed;
                moveDirection.y = yStore;
                
                moveDirection.y += (Physics.gravity.y * gravityScale * Time.deltaTime);
                cc.Move(moveDirection * Time.deltaTime);
            }
        }
    }
    public bool IsGrounded()
    {
        return IsGroundedByCController() || IsGroundedByRaycast();      //this also doesn't call raycast if we know we are grounded

    }
    public float CountTime()
    {
        return Time.time - startTime;
    }
    public bool IsGroundedByCController()
    {

        if (cc.isGrounded == false)
        {
            if (counterStarted == false)
            {
                startTime = Time.time;
                counterStarted = true;
            }
        }
        else counterStarted = false;

        if (CountTime() > duration)
        {
            return false;
        }
        return true;
    }
    public bool IsGroundedByRaycast()
    {
        

        if (Physics.Raycast(transform.position, direction, out hit, GroundRaycastDist))
        {      
            return true;
        }
        return false;
    }


    public void PlayerTakeDamage(float amount)
    {
        currHealth -= amount;
        ui.UpdateHealth(currHealth);

        if (currHealth <= 0)
        {
            PlayerDie();
        } else {
            StartCoroutine(FlashScreen());
        }

    }

    private void PlayerDie()
    {
        endSource.PlayOneShot(overSound);
        StartCoroutine(ShowMenu());
    }

    IEnumerator ShowMenu()
    {
        animator.SetBool("Die", true);
        yield return new WaitForSeconds(5f);
        menu.GameOver();
    }

    private void Step()
    {
        if (isSlow)
        {
            slowAudioSource.PlayOneShot(slowFootStep);
        } else
        {
            normalAudioSource.PlayOneShot(normalFootstep);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("GroundStain"))
        {
            isSlow = true;
            walkSpeed -= 1.5f;
            runSpeed -= 1.5f;
            gunWalkSpeed -= 1.5f;
            gunRunSpeed -= 1.5f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("GroundStain"))
        {
            isSlow = false;
            walkSpeed = walkSpeedStore;
            runSpeed = runSpeedStore;
            gunWalkSpeed = gunWalkSpeedStore;
            gunRunSpeed = gunRunSpeedStore;
        }
    }
}

//is grounded source
//https://forum.unity.com/threads/controller-isgrounded-doesnt-work-reliably.91436/