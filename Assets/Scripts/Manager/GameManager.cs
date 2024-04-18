using System;
using System.IO;
using System.Text;
using Dobrozaur.Gameplay;
using Newtonsoft.Json;
using UnityEngine;

namespace Dobrozaur.Manager
{
    public class GameManager
    {
        public PairData PairData;

        public void InitGames()
        {
            PairData = new PairData();
        }
    }
}