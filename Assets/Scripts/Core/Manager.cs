using System;
using UnityEngine;
using Zenject;

namespace Dobrozaur.Core
{
    public abstract class Manager : IInitializable
    {
        public abstract void Initialize();
    }
}