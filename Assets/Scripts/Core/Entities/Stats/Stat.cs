using UnityEngine;

namespace Coursework.Core.Entities.Stats
{
    // BASE CLASS
    public class Stat
    {
        protected readonly float maxValue;
        protected readonly float increaseRate;
        public float Value { get; protected set; }
        public float Percentage { get { return Value / maxValue; } }

        public Stat(float maxValue, float increaseRate) {
            this.maxValue = maxValue;
            this.increaseRate = increaseRate;

            Value = 0f;
        }

        public void Restore(float restoreValue = -1) {
            if (restoreValue == -1) {
                Value = 0f;
                return;
            }
            
            Value -= restoreValue;
            Value = Mathf.Clamp(Value, 0f, maxValue);
        }

        public virtual bool OnUpdate() {
            Value += Time.deltaTime * increaseRate;
            return Value >= maxValue;
        }

        public virtual void OnReset() {
            Value = 0f;
        }
    }

    public class Mating : Stat
    {
        public Mating(float maxValue, float increaseRate) : base(maxValue, increaseRate) {}

        public override bool OnUpdate() {
            Value += Time.deltaTime * increaseRate;
            return false;
        }
    }
}