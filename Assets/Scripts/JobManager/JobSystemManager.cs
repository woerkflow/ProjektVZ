using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class JobSystemManager : MonoBehaviour {
    
    public List<Turret> Turrets { get; } = new();
    public List<Spawn> Spawns { get; } = new();
    public List<Enemy> Enemies { get; } = new();
    public List<Bullet> Bullets { get; } = new();
    public List<Building> Buildings { get; } = new();
    
    private readonly List<IJobSystem> _jobSystems = new();
    private NativeArray<JobHandle> _jobs;
    
    
    #region Unity Methods
    
    private void Start() {
        IJobSystem seekEnemyJobManager = gameObject.AddComponent<SeekEnemyJobManager>();
        _jobSystems.Add(seekEnemyJobManager);
        seekEnemyJobManager.Register(this);
        
        IJobSystem seekBuildingJobManager = gameObject.AddComponent<SeekBuildingJobManager>();
        _jobSystems.Add(seekBuildingJobManager);
        seekBuildingJobManager.Register(this);

        IJobSystem bulletJobManager = gameObject.AddComponent<BulletJobManager>();
        _jobSystems.Add(bulletJobManager);
        bulletJobManager.Register(this);

        IJobSystem turretJobManager = gameObject.AddComponent<TurretJobManager>();
        _jobSystems.Add(turretJobManager);
        turretJobManager.Register(this);

        IJobSystem spawnJobManager = gameObject.AddComponent<SpawnJobManager>();
        _jobSystems.Add(spawnJobManager);
        spawnJobManager.Register(this);
    }

    private void Update() {
        _jobs = new NativeArray<JobHandle>(_jobSystems.Count, Allocator.TempJob);
        
        for (int i = 0; i < _jobSystems.Count; i++) {
            IJobSystem jobSystem = _jobSystems[i];
            jobSystem.CalculateJobCount();
            
            if (jobSystem.GetJobCount() <= 0) {
                continue;
            }
            jobSystem.EnsureArrayCapacity();
            _jobs[i] = jobSystem.ScheduleJobs();
        }
        JobHandle.CompleteAll(_jobs);
        
        for (int i = 0; i < _jobSystems.Count; i++) {
            IJobSystem jobSystem = _jobSystems[i];
            
            if (jobSystem.GetJobCount() <= 0) {
                continue;
            }
            jobSystem.ApplyJobResults();
        }
        _jobs.Dispose();
    }
    
    #endregion
    
    
    #region Public Class Methods
    
    public void RegisterTurret(Turret turret) {
        Turrets.Add(turret);
    }

    public void UnregisterTurret(Turret turret) {
        Turrets.Remove(turret);
    }
    
    public void RegisterSpawn(Spawn spawn) {
        Spawns.Add(spawn);
    }

    public void UnregisterSpawn(Spawn spawn) {
        Spawns.Remove(spawn);
    }
    
    public void RegisterEnemy(Enemy enemy) {
        Enemies.Add(enemy);
    }

    public void UnregisterEnemy(Enemy enemy) {
        Enemies.Remove(enemy);
    }
    
    public void RegisterBuilding(Building building) {
        Buildings.Add(building);
    }
    
    public void UnregisterBuilding(Building building) {
        Buildings.Remove(building);
    }
    
    public void RegisterBullet(Bullet bullet) {
        Bullets.Add(bullet);
    }
    
    public void UnregisterBullet(Bullet bullet) {
        Bullets.Remove(bullet);
    }
    
    #endregion
}