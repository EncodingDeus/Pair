using System;

namespace Dobrozaur.Gameplay
{
    [Serializable]
    public class Level
    {

        public const int MaxStars = 3;

        public int Number;
        public bool IsCompleted;
        public int Columns;
        public int Rows;
        public long Score;
        public short Attempts;
        public int IdenticalCards;
        public int Stars;
        public bool IsLocked;
        
        public event Action<Level> LevelCompleted;
        
        
        public Level(int number, bool isCompleted, int columns, int rows, int identicalCards)
        {
            Number = number;
            IsCompleted = isCompleted;
            Columns = columns;
            Rows = rows;
            IdenticalCards = identicalCards;
            Stars = 0;
            IsLocked = true;
        }

        public void Unlock()
        {
            IsLocked = false;
        }
        
        public void CompleteLevel(int stars)
        {
            IsLocked = false;
            Stars = stars;
            IsCompleted = true;
            
            LevelCompleted?.Invoke(this);
        }
    }
}