using Unity.Jobs;

public interface IJobSystem {
    
    void CalculateJobCount();
    int GetJobCount();
    JobHandle ScheduleJobs();
    void ApplyJobResults();
}