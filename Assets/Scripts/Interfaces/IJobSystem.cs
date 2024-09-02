using Unity.Jobs;

public interface IJobSystem {
    
    void Register(JobSystemManager jobSystemManager);
    void CalculateJobCount();
    int GetJobCount();
    void EnsureArrayCapacity();
    JobHandle ScheduleJobs();
    void ApplyJobResults();
}