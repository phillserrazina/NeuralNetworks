namespace Coursework.Core.Entities.Stats
{
    // Inspired by Sebastian Lague's video about evolution simulation
    // Link: https://www.youtube.com/watch?v=r_It_X7v-1E
    public class EntityGenes {

        //public readonly float GestationDuration;    // The lower it is, the more likely it is for new-born death
        public readonly int SensoryDistance;      // The distance at which it can sense other things
        public readonly float ReproductiveUrge;     // The amount of time dedicated to finding a mate, instead of food or water
        //public readonly int Speed;                // How fast the creature moves (The faster, the more hungry and thirsty it gets)
        public readonly float Desirability;         // Affects the likelihood of attracting a mate

        public EntityGenes(/*float gestationDuration,*/ int sensoryDistance, float reproductiveUrge/*, float speed, float desirability*/) {
            //GestationDuration = gestationDuration;
            SensoryDistance = sensoryDistance;
            ReproductiveUrge = reproductiveUrge;
            //Speed = speed;
            Desirability = 1f;
        }
    }
}

