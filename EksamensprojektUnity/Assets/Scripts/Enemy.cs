using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class Enemy : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private LayerMask playerMask;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Attack")]
    [SerializeField] private float attackCooldown = 2f;

    [Header("Health")]
    [SerializeField] private int maxHealth = 50;

    private NavMeshAgent _agent;
    private Animator _animator;
    private CharacterController _controller;
    private Transform _player;


    private enum State { Idle, Aggro }
    private State _state = State.Idle;

    private int _currentHealth;
    private float _detectionTimer;
    private float _attackTimer;
    private bool _isPerformingAction;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();

        _agent.updatePosition = true;
        _agent.updateRotation = false;
        _agent.speed = moveSpeed;

        _currentHealth = maxHealth;
    }

    private void Update()
    {
        if (_dead) return;

        _detectionTimer += Time.deltaTime;
        if (_detectionTimer >= 1f)
        {
            _detectionTimer = 0f;
            DetectPlayer();
        }

        if (_state == State.Aggro)
        {
            ChasePlayer();
            TryAttack();
        }

    }

    private void DetectPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRange, playerMask);
        if (hits.Length > 0)
        {
            _player = hits[0].transform;
            _state = State.Aggro;
        }
        else
        {
            _player = null;
            _state = State.Idle;
            _animator.SetFloat("Vertical", 0f);
            _animator.SetBool("isMoving", false);
        }
    }

    private void ChasePlayer()
    {
        if (_player == null) return;

        // never move or update blend tree while attacking
        if (_isPerformingAction)
        {
            _agent.isStopped = true;
            _animator.SetFloat("Vertical",   0f);
            _animator.SetFloat("Horizontal", 0f);
            _animator.SetBool("isMoving",    false);
            return;
        }

        float dist = Vector3.Distance(transform.position, _player.position);

        Vector3 desiredDir = Vector3.zero;

        if (dist > attackRange)
        {
            _agent.SetDestination(_player.position);
            _agent.isStopped = false;

            desiredDir = _agent.desiredVelocity;
            desiredDir.y = 0f;
            Vector3 localDesired = transform.InverseTransformDirection(desiredDir);
            float vertical   = Mathf.Clamp(localDesired.z / moveSpeed, -1f, 1f);
            float horizontal = Mathf.Clamp(localDesired.x / moveSpeed, -1f, 1f);
            _animator.SetFloat("Vertical",   vertical);
            _animator.SetFloat("Horizontal", horizontal);
            _animator.SetBool("isMoving",    vertical > 0.1f);
        }
        else
        {
            _agent.isStopped = true;
            _animator.SetFloat("Vertical",   0f);
            _animator.SetFloat("Horizontal", 0f);
            _animator.SetBool("isMoving",    false);
        }

        Vector3 rotDir = desiredDir.sqrMagnitude > 0.1f ? desiredDir : (_player.position - transform.position);
        rotDir.y = 0f;
        if (rotDir.sqrMagnitude > 0.001f)
        {
            Quaternion target = Quaternion.LookRotation(rotDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, rotationSpeed * Time.deltaTime);
        }
    }

    private void TryAttack()
    {
        if (_player == null) return;

        _attackTimer += Time.deltaTime;
        float dist = Vector3.Distance(transform.position, _player.position);

        if (dist <= attackRange && _attackTimer >= attackCooldown && !_isPerformingAction)
        {
            _attackTimer = 0f;
            _isPerformingAction = true;
            _animator.SetTrigger("Attack");
        }
    }

    public int CurrentHealth => _currentHealth;
    public int MaxHealth     => maxHealth;
    public bool IsAggro      => _state == State.Aggro;

    public void TakeDamage(int amount)
    {
        if (_dead) return;
        _currentHealth -= amount;
        if (_currentHealth <= 0)
            StartDeath();
    }

    private bool _dead;

    private void StartDeath()
    {
        _dead = true;

        // stop all movement and AI immediately
        _agent.isStopped = true;
        _agent.enabled   = false;
        _controller.enabled = false;
        _isPerformingAction = false;

        // reset all animator params so nothing can interrupt Death
        _animator.SetFloat("Vertical",   0f);
        _animator.SetFloat("Horizontal", 0f);
        _animator.SetBool("isMoving",    false);
        _animator.ResetTrigger("Attack");

        // force Death on Action Override layer (index 1) at time 0
        _animator.SetLayerWeight(1, 1f);
        _animator.Play("Death", 1, 0f);

        Destroy(gameObject, 3f);
    }

    public void OnActionComplete()
    {
        _isPerformingAction = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.2f);
        Gizmos.DrawSphere(transform.position, detectionRange);
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawSphere(transform.position, attackRange);
    }
#endif
}
