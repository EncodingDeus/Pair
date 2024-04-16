using System;
using System.Linq;

namespace Dobrozaur.Gameplay
{
    [Serializable]
    public class Stage
    {
        public int StageNumber;
        public Level[] Levels;
        public int RequiredStars;
        public bool IsLocked;

        public event Action<Stage> StageCompleted;
        
        public int CurrentStars
        {
            get
            {
                return Levels.Sum(l => l.CompleteInfo?.Stars ?? 0);
            }
        }

        public int StarsToComplete => Levels.Length * Level.MaxStars;

        
        public void Init()
        {
            foreach (var level in Levels)
            {
                level.LevelCompleted += OnLevelCompleted;
            }
        }

        public void Unlock()
        {
            IsLocked = false;
            Levels[0].IsLocked = false;
        }

        private void OnLevelCompleted(Level level)
        {
            if (level.CompleteInfo.IsCompleted == false) return;
            
            var nextLevel = Levels.FirstOrDefault(l => l.Setting.Number == (level.Setting.Number + 1));

            if (nextLevel != null)
            {
                nextLevel.IsLocked = false;
            }
            else
            {
                StageCompleted?.Invoke(this);
            }
            
            // if (Levels.Count(l => l.IsCompleted) >= Levels.Length)
            // {
            //     StageCompleted?.Invoke(this);
            // }
        }
    }
}