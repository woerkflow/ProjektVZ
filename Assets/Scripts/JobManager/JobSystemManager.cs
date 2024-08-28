using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class JobSystemManager : MonoBehaviour {
    
    private readonly List<IJobSystem> _jobSystems = new();
    private NativeArray<JobHandle> _jobs;
    
    
    #region Unity Methods
    
    private void Start() {
        IJobSystem bulletJobManager = FindObjectOfType<BulletJobManager>();

        if (bulletJobManager != null) {
            _jobSystems.Add(bulletJobManager);
        }
        IJobSystem turretJobManager = FindObjectOfType<TurretJobManager>();
        
        if (turretJobManager != null) {
            _jobSystems.Add(turretJobManager);
        }
        IJobSystem spawnJobManager = FindObjectOfType<SpawnJobManager>();
        
        if (spawnJobManager != null) {
            _jobSystems.Add(spawnJobManager);
        }
        _jobs = new (_jobSystems.Count, Allocator.Persistent);
    }

    private void Update() {

        for (int i = 0; i < _jobSystems.Count; i++) {
            _jobSystems[i].CalculateJobCount();
            
            if (_jobSystems[i].GetJobCount() <= 0) {
                continue;
            }
            _jobs[i] = _jobSystems[i].ScheduleJobs();
        }
        JobHandle.CompleteAll(_jobs);
        
        for (int i = 0; i < _jobSystems.Count; i++) {

            if (_jobSystems[i].GetJobCount() <= 0) {
                continue;
            }
            _jobSystems[i].ApplyJobResults();
        }
    }

    private void OnDestroy() {
        _jobs.Dispose();
    }
    
    #endregion
}