using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BallisticsManager : MonoBehaviour
{

    public List<Projectile> projectiles = new List<Projectile>();

    public static BallisticsManager main;


    private void Awake()
    {
        main = this;
    }


    public void LateUpdate()
    {
        UpdateProjectiles();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        foreach(var p in projectiles.ToArray())
        {
            Gizmos.DrawWireSphere(p.position, 0.1f);
        }
    }

    public void UpdateProjectiles()
    {
        foreach (var p in projectiles.ToArray())
        {
            UpdateProjectile(p);
        }
    }

    public void UpdateProjectile(Projectile _proj)
    {
        _proj.position += _proj.speed * Time.deltaTime * _proj.direction;

        _proj.lr.positionCount = 2;
        _proj.lr.SetPosition(0, _proj.position - _proj.direction * 1f);
        _proj.lr.SetPosition(1, _proj.position);
    }


    public void NewProjectile(Vector3 _position, Vector3 _direction, float _speed, float _mass)
    {
        var lr = Pool.main.Get(PoolObjectTypes.BulletRenderer).GetComponent<LineRenderer>();

        var projectile = new Projectile(_position,_direction, _speed, _mass, 100f, lr);

        projectiles.Add(projectile);
    }
}


[Serializable]
public class Projectile
{
    public Vector3 position;
    public Vector3 direction;
    public float speed;
    public float mass;
    public float life;
    public LineRenderer lr;

    public Projectile() { }

    public Projectile(Vector3 _position, Vector3 _direction, float _speed, float _mass, float _startLife, LineRenderer _lr)
    {
        position = _position;
        direction = _direction;
        speed = _speed;
        mass = _mass;
        life = _startLife;
        lr = _lr;
        lr.SetPosition(0, position - direction * 0.2f);
        lr.SetPosition(1, position);
    }
}
