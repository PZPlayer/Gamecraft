using Gamecraft.Player;
using UnityEngine;

namespace Invector.vCharacterController
{
    public class vThirdPersonInput : MonoBehaviour
    {
        #region Variables       

        [Header("Controller Input")]
        public string horizontalInput = "Horizontal";
        public string verticallInput = "Vertical";
        public KeyCode jumpInput = KeyCode.Space;
        public KeyCode AimInputKey = KeyCode.Mouse1;
        public KeyCode ShootInputKey = KeyCode.Mouse0;
        public KeyCode strafeInput = KeyCode.Tab;
        public KeyCode sprintInput = KeyCode.LeftShift;
        public KeyCode InventoryKey1 = KeyCode.Alpha1;
        public KeyCode InventoryKey2 = KeyCode.Alpha2;
        public KeyCode InventoryKey3 = KeyCode.Alpha3;
        public DashSetting DashForward;
        public DashSetting DashBack;
        public DashSetting DashRight;
        public DashSetting DashLeft;

        [Header("Camera Input")]
        public string rotateCameraXInput = "Mouse X";
        public string rotateCameraYInput = "Mouse Y";

        [HideInInspector] public vThirdPersonController cc;
        [HideInInspector] public Aim AimSc;
        [HideInInspector] public Inventory Inv;
        [HideInInspector] public Dash DashController;
        [HideInInspector] public vThirdPersonCamera tpCamera;
        [HideInInspector] public Camera cameraMain;

        #endregion

        protected virtual void Start()
        {
            InitilizeController();
            InitializeTpCamera();
        }

        protected virtual void FixedUpdate()
        {
            cc.UpdateMotor();               // updates the ThirdPersonMotor methods
            cc.ControlLocomotionType();     // handle the controller locomotion type and movespeed
            if(!AimSc.isAiming) cc.ControlRotationType();       // handle the controller rotation type
        }

        protected virtual void Update()
        {
            InputHandle();                  // update the input methods
            cc.UpdateAnimator();            // updates the Animator Parameters
        }

        public virtual void OnAnimatorMove()
        {
            cc.ControlAnimatorRootMotion(); // handle root motion animations 
        }

        #region Basic Locomotion Inputs

        protected virtual void InitilizeController()
        {
            cc = GetComponent<vThirdPersonController>();
            DashController = GetComponent<Dash>();
            AimSc = GetComponent<Aim>();
            Inv = GetComponent<Inventory>();

            if (cc != null)
                cc.Init();
        }

        protected virtual void InitializeTpCamera()
        {
            if (tpCamera == null)
            {
                tpCamera = FindObjectOfType<vThirdPersonCamera>();
                if (tpCamera == null)
                    return;
                if (tpCamera)
                {
                    tpCamera.SetMainTarget(this.transform);
                    tpCamera.Init();
                }
            }
        }

        protected virtual void InputHandle()
        {
            MoveInput();
            CameraInput();
            SprintInput();
            StrafeInput();
            JumpInput();
            DashInput();
            AimInput();
            InventoryInput();
        }

        public virtual void InventoryInput()
        {
            if (Input.GetKeyDown(InventoryKey1))
            {
                Inv.ChangeItem(1);
            }
            else if (Input.GetKeyDown(InventoryKey2))
            {
                Inv.ChangeItem(2);
            }
            else if (Input.GetKeyDown(InventoryKey3))
            {
                Inv.ChangeItem(3);
            }
        }

        public virtual void AimInput()
        {
            if (Input.GetKey(AimInputKey))
            {
                AimSc.OnStartAiming();
                cc.AimControlRotationType(true);
            }
            else if (Input.GetKeyUp(AimInputKey))
            {
                AimSc.OnEndAiming();
            }

            if (Input.GetKey(ShootInputKey))
            {
                Inv.UseItem();
            }
        }

        public virtual void MoveInput()
        {
            cc.input.x = Input.GetAxis(horizontalInput);
            cc.input.z = Input.GetAxis(verticallInput);
            cc.ifAiming = AimSc.isAiming;
        }

        public void DashInput()
        {
            DashController.IfGrounded = JumpConditions();
            if (Input.GetKey(DashForward.Key) && Input.GetKey(KeyCode.LeftControl))
            {
                DashController.StartCoroutine(DashController.Jump(transform.forward, DashForward.Power));
            }
        }

        protected virtual void CameraInput()
        {
            if (!cameraMain)
            {
                if (!Camera.main) Debug.Log("Missing a Camera with the tag MainCamera, please add one.");
                else
                {
                    cameraMain = Camera.main;
                    cc.rotateTarget = cameraMain.transform;
                }
            }

            if (cameraMain)
            {
                cc.UpdateMoveDirection(cameraMain.transform);
            }

            if (tpCamera == null)
                return;

            var Y = Input.GetAxis(rotateCameraYInput);
            var X = Input.GetAxis(rotateCameraXInput);

            tpCamera.RotateCamera(X, Y);
        }

        protected virtual void StrafeInput()
        {
            if (Input.GetKeyDown(strafeInput))
                cc.Strafe();
        }

        protected virtual void SprintInput()
        {
            if (Input.GetKeyDown(sprintInput))
                cc.Sprint(true);
            else if (Input.GetKeyUp(sprintInput))
                cc.Sprint(false);
        }

        /// <summary>
        /// Conditions to trigger the Jump animation & behavior
        /// </summary>
        /// <returns></returns>
        protected virtual bool JumpConditions()
        {
            return cc.isGrounded && cc.GroundAngle() < cc.slopeLimit && !cc.isJumping && !cc.stopMove;
        }

        /// <summary>
        /// Input to trigger the Jump 
        /// </summary>
        protected virtual void JumpInput()
        {
            if (Input.GetKeyDown(jumpInput) && JumpConditions())
                cc.Jump();
        }

        #endregion       
    }
}