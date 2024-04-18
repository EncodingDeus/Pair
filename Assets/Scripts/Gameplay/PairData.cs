using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace Dobrozaur.Gameplay
{
    public class PairData
    {
        public Stage[] Stages;

        public PairData()
        {
            Stages = new Stage[8];
            for (int i = 0; i < Stages.Length; i++)
            {
                Stages[i] = new Stage()
                {
                    StageNumber = i + 1,
                    Levels = new Level[25],
                    IsLocked = true,
                };

                for (int j = 0; j < Stages[i].Levels.Length; j++)
                {
                    Stages[i].Levels[j] = new Level(j + 1, false, 3,3,3);
                }
                
                Stages[i].Init();
                Stages[i].StageCompleted += OnStageCompleted;
            }

            int stars = 0;
            for (int i = 1; i < Stages.Length; i++)
            {
                stars += Stages[i - 1].StarsToComplete;
                Stages[i].RequiredStars = stars;
            }
            
            Stages[0].IsLocked = false;
            Stages[0].Levels[0].IsLocked = false;

            var json = JsonConvert.SerializeObject(Stages);

            string filePath = $"{Application.dataPath}/Stages.json";
            Debug.Log(filePath);
            using (FileStream file = File.Open(filePath, FileMode.Create, FileAccess.Write))
            {
                byte[] data = Encoding.UTF8.GetBytes(json);
                file.Write(data);
                file.Close();
            }
        }
        
        public int GetStars()
        {
            int stars = 0;

            foreach (var stage in Stages)
            {
                foreach (var level in stage.Levels)
                {
                    stars += level.CompleteInfo?.Stars ?? 0;
                }
            }

            return stars;
        }

        // private void Start()
        // {
        //     Application.targetFrameRate = 60;
        //     InitStages();
        // }

        private void OnStageCompleted(Stage stage)
        {
            stage.StageCompleted -= OnStageCompleted;
        
            int index = Array.IndexOf(Stages, stage);
            Stages[index + 1].Unlock();
        }
    }
}