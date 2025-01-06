using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class NPCMovement : MonoBehaviour
{
    [SerializeField] protected float _walkSpeed = 1f;
    [SerializeField] protected float _runSpeed = 1f;
    [SerializeField] protected float _activeSpeed = 1f;
    
    [SerializeField] protected bool _isIdle = true;
    [SerializeField] protected bool _isRunning = false;
    [SerializeField] protected bool _goRight = true;

    [SerializeField] protected float _minWalkRange;
    [SerializeField] protected float _maxWalkRange;

    [SerializeField] protected float _movePoint;

    protected void Start()
    {
        _movePoint = GetNewWalkPoint();
    }

    protected void FixedUpdate()
    {
        if (!_isIdle)
        {
            Move();
            
            if (IsArrived())
            {
                _isIdle = true;
                _movePoint = GetNewWalkPoint();
            }
        }
    }

    protected void Move()
    {
        if (_goRight)
            transform.Translate(Vector2.right * _activeSpeed * Time.deltaTime);
        else
            transform.Translate(Vector2.left * _activeSpeed * Time.deltaTime);
    }

    protected float GetNewWalkPoint()
    {
        float newPoint = Random.Range(_minWalkRange, _maxWalkRange);
        
        if (newPoint < transform.position.x)
            _goRight = false;
        else if (newPoint > transform.position.x)
            _goRight = true;
        
        StartCoroutine(Wait(2));
        return newPoint;
    }

    protected bool IsArrived()
    {
        if (_goRight)
        {
            if (transform.position.x >= _movePoint)
            {
                return true;
            }
        }
        else
        {
            if (transform.position.x <= _movePoint)
            {
                return true;
            }
        }
        
        return false;
    }

    protected IEnumerator Wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _isIdle = false;
    }

    protected void IsRunning(bool value)
    {
        _isRunning = value;

        if (_isRunning)
            _activeSpeed = _runSpeed;
        else
            _activeSpeed = _walkSpeed;
    }
}
