using NUnit.Framework;
using UnityEngine;

namespace Tests.EditMode {
    
    public class TileStrategyTests {

        private Tile _tile;
        private PlayerManager _playerManager;
        private EnemySpawner _enemySpawner;

        [SetUp]
        public void SetUp() {

            _tile = new GameObject().AddComponent<Tile>();

            _playerManager = new GameObject().AddComponent<PlayerManager>();
            _tile.playerManager = _playerManager;

            _enemySpawner = new GameObject().AddComponent<EnemySpawner>();
            _tile.enemySpawner = _enemySpawner;
        }

        [Test]
        public void BuildStrategy_CanInteract_ReturnsTrue_WhenConditionsAreMet() {
            
            BuildStrategy buildStrategy = new BuildStrategy();
            TileObject tileObject = new GameObject().AddComponent<TileObject>();
            _tile.selectedBuilding = tileObject;
            _tile.selectedBuilding.blueprint.resources = new Resources { wood = 100, waste = 100, whiskey = 100 };
            _enemySpawner.state = RoundState.Build;
            
            _playerManager.AddResources(new Resources { wood = 100, waste = 100, whiskey = 100 });
            
            bool canInteract = buildStrategy.CanInteract(_tile);
            
            Assert.IsTrue(canInteract);
        }

        [Test]
        public void BuildStrategy_CanInteract_ReturnsFalse_WhenConditionsAreNotMet() {

            BuildStrategy buildStrategy = new BuildStrategy();
            TileObject tileObject = new GameObject().AddComponent<TileObject>();
            _tile.selectedBuilding = tileObject;
            _tile.selectedBuilding.blueprint.resources = new Resources { wood = 100, waste = 100, whiskey = 100 };
            _enemySpawner.state = RoundState.Build;
            
            _playerManager.ClearResources();
            bool canInteract = buildStrategy.CanInteract(_tile);
            
            Assert.IsFalse(canInteract);
        }

        [Test]
        public void BuildStrategy_Interact_BuildsObject_WhenConditionsAreMet() {
        
            BuildStrategy buildStrategy = new BuildStrategy();
    
            // Arrange: Setup the TileObject to be built
            TileObject tileObject = new GameObject("TileObject").AddComponent<TileObject>();
            tileObject.blueprint = new TileObjectBlueprint {
                type = TileObjectType.Building,
                prefab = tileObject.gameObject,
                resources = new Resources { wood = 100, waste = 100, whiskey = 100 }
            };
    
            _tile.selectedBuilding = tileObject;
            _tile.spawnPoint = new GameObject();
            _tile.spawnPoint.transform.position = Vector3.zero;
    
            _enemySpawner.state = RoundState.Build;
    
            // Arrange: Simulate enough player resources
            _playerManager.AddResources(new Resources { wood = 100, waste = 100, whiskey = 100 });
    
            // Act: Perform the interaction (build)
            buildStrategy.Interact(_tile);
    
            // Assert: Check that a new object of the same type was instantiated
            Assert.IsNotNull(_tile.tileObject);
            Assert.AreEqual(_tile.tileObject.blueprint.GetType(), tileObject.blueprint.GetType());
            Assert.AreEqual(_tile.tileObject.name, tileObject.name + "(Clone)");
        }
    }
}