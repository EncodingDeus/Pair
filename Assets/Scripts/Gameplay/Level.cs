using System;
using System.Collections.Generic;

namespace Dobrozaur.Gameplay
{
    [Serializable]
    public class Level
    {
        public const int MaxStars = 3;

        // public bool IsCompleted;
        // public long Score;
        // public int Attempts;
        // public int Misses;
        // public int Stars;
        public bool IsLocked;

        
        public LevelCompleteInfo CompleteInfo { get; private set; }
        public LevelSetting Setting { get; }
        public event Action<Level> LevelCompleted;
        
        
        public Level(int number, bool isCompleted, int columns, int rows, int identicalCards)
        {
            Setting = new LevelSetting(number, columns, rows, identicalCards);
            
            // IsCompleted = isCompleted;
            IsLocked = true;
        }

        public void Unlock()
        {
            IsLocked = false;
        }
        
        public LevelCompleteInfo CompleteLevel(int attempts, int misses)
        {
            var stars = 0;
            
            for (int i = Setting.CompleteSettings.Length - 1; i >= 0; i--)
            {
                if (misses <= Setting.CompleteSettings[i])
                {
                    stars = i;
                    break;
                } 
            }

            CompleteInfo = new LevelCompleteInfo(stars, attempts, misses, stars > 0);
            
            LevelCompleted?.Invoke(this);

            return CompleteInfo;
        }

        public class LevelSetting
        {
            private readonly int _minMisses;
            private readonly int _maxMisses;
            private readonly int _cardsCount;

            public int Number;
            public int Columns;
            public int Rows;
            public int IdenticalCards;
            
            public int[] CompleteSettings { get; }

            public int MaxMisses => _maxMisses;
            public int MinMisses => _minMisses;

            
            public LevelSetting(int number, int columns, int rows, int identicalCards)
            {
                Number = number;
                Columns = columns;
                Rows = rows;
                IdenticalCards = identicalCards;
                
                _cardsCount = rows * columns;
                _minMisses = 0;
                _maxMisses = (int)(_cardsCount / (identicalCards / 2));

                CompleteSettings = new int[]
                {
                    _maxMisses,
                    (int)(_maxMisses),
                    (int)(_maxMisses * 0.5f),
                    (int)(_maxMisses * 0.2f),
                    _minMisses,
                };

            }
        }

        public class LevelCompleteInfo
        {
            public int Stars;
            public int Attempts;
            public int Misses;
            public bool IsCompleted;
            public bool IsPerfect;
            
            
            public LevelCompleteInfo(int star, int attempts, int misses, bool isCompleted)
            {
                IsPerfect = star > 3;
                Stars = IsPerfect ? 3 : star;
                Attempts = attempts;
                Misses = misses;
                IsCompleted = isCompleted;
            }
        }
    }
}