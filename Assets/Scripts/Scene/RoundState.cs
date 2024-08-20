public abstract class RoundState {
    public abstract void UpdateState(EnemySpawner spawner);
}

public class BuildState : RoundState {
    public override void UpdateState(EnemySpawner spawner) {
        spawner.HandleBuildState();
    }
}

public class FightState : RoundState {
    public override void UpdateState(EnemySpawner spawner) {
        spawner.HandleFightState();
    }
}