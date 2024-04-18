using Dobrozaur.Gameplay;

namespace Dobrozaur.Utility
{
    public static class GameManagerUtils
    {
        public static int GetStars(this Stage[] stages)
        {
            int stars = 0;

            foreach (var stage in stages)
            {
                foreach (var level in stage.Levels)
                {
                    stars += level.CompleteInfo?.Stars ?? 0;
                }
            }

            return stars;
        }
    }

}