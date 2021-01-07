using UnityEngine;

namespace Coursework.Core.Entities
{
    public abstract class EntityController : MonoBehaviour
    {
        // VARIABLES
        [SerializeField] [Range(0f, 20f)] protected float moveCooldown = 3f;
        protected float currentMoveCooldown = 0f;

        protected EntityManager manager;
        protected Tile startPosition;

        public Tile[] Neighbours { get { return manager.Movement.GetNeighbours(); } }

        // METHODS
        public virtual void Initialize(EntityManager manager, Tile startPosition) {
            this.manager = manager;
            this.startPosition = startPosition;
        }

        protected virtual void Move(float x, float y) {
            int finalX = 0;
            int finalY = 0;

            if (x <= -0.5f) finalX = -1;
            else if (x >= 0.5f) finalX = 1; 

            if (y <= -0.5f) finalY = -1;
            else if (y >= 0.5f) finalY = 1;

            manager.Movement.MoveToPoint(manager.Movement.GetNeighbour(finalX, finalY));
        }

        protected bool ReadyToMove() {
            if (currentMoveCooldown > 0f)
            {
                currentMoveCooldown -= Time.deltaTime;
                return false;
            }

            currentMoveCooldown = moveCooldown;
            return true;
        }

        public abstract void Kill();
        public abstract void ResetController();
    }
}
