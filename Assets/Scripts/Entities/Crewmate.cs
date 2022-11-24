using UnityEngine;
using UnityEngine.AI;
using DLIFR.Game;
using DLIFR.Data;

namespace DLIFR.Entities
{
    public class Crewmate : InteractableBehaviour
    {
        public const float PATH_CHECK_INTERVAL = 0.5F;

        #region Public Fields
        [Header("REFERENCES")]
        public Transform ship;
        [HideInInspector, SerializeField]
        public NavMeshAgent agent;
        [HideInInspector, SerializeField]
        public Transform targetHolder;
        public GameMatch match;
        public Canvas paymentCanvas;
        public RectTransform paymentDisplay;
        public Animator animator;

        [Header("COLOR")]
        public GameObject shellColor;
        public Color characterColor;

        [Header("SETTINGS")]
        public float walkSpeed = 3f;

        [Header("ACTION")]
        [HideInInspector, SerializeField]
        private CrewmateState _state;
        public CrewmateState state {
            get => _state;
            set {
                _state?.OnLeave();
                _state = value;
                
                if(_state != null)
                {
                    _state.crewmate = this;
                    _state.OnEnter();
                }
            }
        }
        public Transform grabbedObject;

        [Header("STATE")]
        public bool walking = false;
        public GameObject carrying;

        public Value<float> salaryTime;
        public Value<int> salaryRechargePrice;
        public Value<float> salaryRechargeTime;
        public Value<bool> shouldTimePass;
        

        [Header("GRAVITY")]
        public float gravityForce = 10f;

        [Header("GROUND")]
        public bool isGrounded = false;
        public Vector3 groundNormal = Vector3.up;
        public LayerMask groundLayerMask;
        public float groundCheckDistance = 0.8f;

        [Header("PREFABS")]
        public GameObject prefabTargetHolder;
        #endregion
        
        #region Private Fields
        private Rigidbody _rigidbody;
        private float _lastWalkStart = 0;
        private float _lastPathCheck = 0;
        #endregion

        public override void OnEnable() 
        {
            base.OnEnable();

            shellColor.GetComponent<Renderer>().material.color = characterColor;

            _rigidbody = GetComponent<Rigidbody>();

            if(ship == null)
                ship = GameObject.FindObjectOfType<Ship>().transform;

            if(match == null)
                match = GameObject.FindObjectOfType<GameMatch>();

            if(targetHolder == null)
            {
                GameObject empty = new GameObject();      
                empty.transform.name = $"{name}:Parent";
                transform.parent = empty.transform;

                targetHolder = GameObject.Instantiate(prefabTargetHolder).transform /*new GameObject().transform*/;
                targetHolder.name = $"{name}:Target";
                targetHolder.parent = transform.parent;
                
                //Set Color
                Color targetColor = characterColor;
                targetColor.a = 0.5f;
                targetHolder.GetChild(0).GetComponent<Renderer>().material.color = targetColor;

                GameObject agentHolder = new GameObject();
                agentHolder.name = $"{name}:Agent";
                agent = agentHolder.AddComponent<NavMeshAgent>();
                agentHolder.transform.parent = empty.transform;

                CapsuleCollider collider = GetComponent<CapsuleCollider>();
                agent.radius = collider.radius + 0.2f;
                agent.height = collider.height;
            } 

            targetHolder.parent = ship;
            targetHolder.up = ship.up;
        }

        private void Start() 
        {
            state = new CrewmateIdleState();
            salaryTime.value = salaryRechargeTime.value;
        }

        public override void OnDisable() 
        {
            base.OnDisable();
        }

        private void OnDestroy() 
        {
            state.OnLeave();

            if(match.currentCrewmate != null && match.currentCrewmate == this)
            {
                match.SetSelectedCrewmate(null);
            }

            if(targetHolder != null)
                Destroy(targetHolder.gameObject);
                
            GameObject temp = transform.parent.gameObject;

            Destroy(temp);
            Destroy(agent.gameObject);

            targetHolder = null;
            agent = null;
        }
        
        private void FixedUpdate()
        {
            animator.SetLayerWeight(1, carrying != null ? 1f : 0f);

            #region Update Payment Display
            paymentCanvas.transform.LookAt(Camera.main.transform);

            if(shouldTimePass.value)
            {
                salaryTime.value -= Time.fixedDeltaTime;

                if(salaryTime.value <= 0f)
                {
                    Destroy(gameObject);
                    return;
                }
            }

            paymentDisplay.sizeDelta = new Vector2(100 * salaryTime.value/salaryRechargeTime.value, 25f);
            
            
            
            #endregion

            #region Update Grounded
            Debug.DrawLine(transform.position - new Vector3(0, groundCheckDistance, 0), transform.position - new Vector3(0, groundCheckDistance + 0.25f, 0), Color.red);
            isGrounded = UnityEngine.Physics.CheckSphere(transform.position - new Vector3(0, groundCheckDistance, 0), 0.05f, groundLayerMask);
            #endregion

            Vector3 velocity = _rigidbody.velocity;

            if (isGrounded)
            {
                #region Ground Normal 
                if (UnityEngine.Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.1f, groundLayerMask))
                {
                    groundNormal = hit.normal;
                }
                #endregion
            }
            else
            {
                velocity.y -= gravityForce * Time.fixedDeltaTime;
            }

            #region Walking
            if(walking)
            {
                Vector3 tpos = agent.transform.position;
                Vector3 deltaPos = (tpos - transform.position);
                Vector3 fw = new Vector3(deltaPos.x, 0, deltaPos.z).normalized;
                float angle = Vector3.SignedAngle(Vector3.forward, fw, Vector3.up);
                float nAngle = Mathf.LerpAngle(transform.eulerAngles.y, angle, Time.fixedDeltaTime * 3f);
                transform.eulerAngles = new Vector3(0, nAngle,0);

                float walkSpeed = _state.GetWalkSpeed();
                
                agent.speed = deltaPos.magnitude > 1f ? 0 : walkSpeed;

                velocity.x = deltaPos.normalized.x * walkSpeed;
                velocity.z = deltaPos.normalized.z * walkSpeed;

                animator.SetBool("walking", agent.remainingDistance > 0.3f);

                if(agent.remainingDistance < 0.25f && Time.time > _lastWalkStart + 0.5f)
                {
                    walking = false;
                    agent.Warp(transform.position);
                    _state?.OnEndWalking(true);
                }
                else if(_lastPathCheck + PATH_CHECK_INTERVAL < Time.time)
                {
                    _lastPathCheck = Time.time;

                    if(_state != null)
                        targetHolder.position = _state.OnRecalculatePath(targetHolder.position);

                    agent.SetDestination(targetHolder.position);
                }
            }
            else
            {
                velocity.x = 0f;
                velocity.z = 0f;
                animator.SetBool("walking", false);
            }
            #endregion

            _rigidbody.velocity = velocity;

            _state?.OnTick(Time.fixedDeltaTime);
        }

        #region Walk
        public virtual void StopWalking()
        {
            if(walking)
                _state?.OnEndWalking(false);

            walking = false;
        }

        public virtual void WalkTo(Vector3 target)
        {
            if(_state != null)
                target = _state.OnStartWalking(target);

            agent.Warp(transform.position);
            targetHolder.position = target;
            agent.SetDestination(targetHolder.position);
            walking = true;
            _lastWalkStart = _lastPathCheck = Time.time;   
        }
        #endregion

        #region Carrying
        public void SetCarrying(GameObject carrying)
        {
            if(this.carrying != null) 
                DropCarrying();

            Rigidbody rb = carrying.GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.detectCollisions = false;

            carrying.transform.parent = transform;
            carrying.transform.localPosition = new Vector3(0, 0.25f, 0.55f);
            carrying.transform.localEulerAngles = Vector3.zero;

            this.carrying = carrying;
        }

        public void DropCarrying()
        {
            Rigidbody rb = carrying.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.detectCollisions = true;
            rb.velocity = transform.forward * 2f;

            carrying.transform.parent = null;
            carrying = null;
        }
        #endregion

        #region Interaction
        public override void OnInteract(int click, GameMatch match)
        {
            if(match.CanDoAction("crewmate_select", gameObject))
            {
                if(match.currentCrewmate != null)
                {
                    SetLayer(match.currentCrewmate.transform, 0);
                
                    if(match.currentCrewmate == this)
                    {
                        match.currentCrewmate = null;
                        return;
                    }
                }

                SetLayer(transform, LayerMask.NameToLayer("Selected")); 
                match.currentCrewmate = this;             
            }
        }
        
        public void OnClickOnGround(Vector3 position)
        {
            CrewmateWalkingToState state = new CrewmateWalkingToState();
            state.target = position;

            if(match.CanDoAction("crewmate_walk_to", gameObject))
            {
                this.state = state;
            }
        }
        #endregion

        #region Utils
        public static void SetLayer(Transform transform, int layer)
        {
            transform.gameObject.layer = layer;

            foreach(Transform ch in transform)
                SetLayer(ch, layer);
        }
        #endregion
    }
}