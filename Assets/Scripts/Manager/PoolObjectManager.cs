using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Nito.Collections;
using UnityEngine;
using UnityEngine.Pool;
using Zenject;

namespace Dobrozaur.Manager
{
    public class PoolObjectManager : IInitializable
    {
        private const float CheckPoolTime = 1f;

        private readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
        
        private Dictionary<string, PoolGroup<object>> _poolGroups = new Dictionary<string, PoolGroup<object>>(10);
        // private Dictionary<string, Dictionary<string, PoolGroup<object>>> _poolGroups = new Dictionary<string, Dictionary<string, PoolGroup<object>>>(10);

        /// <summary>
        /// Creates group of poolable objects
        /// </summary>
        /// <param name="name">Group name</param>
        /// <param name="capacity">Maximum object in group</param>
        /// <param name="objectLifetime">Duration of lifetime in seconds when object added to the pool</param>
        public PoolGroup<object> CreateGroup(string name, int capacity, int objectLifetime)
        {
            var poolGroup = new PoolGroup<object>(name, capacity, objectLifetime);
            
            if (_poolGroups.TryAdd(name, poolGroup))
            {
                return poolGroup;
            }
            else
            {
                throw new Exception("JOPA, u tebya ne poluchilos");
            }
        }

        public void AddObject(string groupName, object obj)
        {
            if (_poolGroups.TryGetValue(groupName, out var group))
            {
                group.AddObject(obj);
            }
        }
        
        public TObject PopObject<TObject>(string groupName)
        {
            if (_poolGroups.TryGetValue(groupName, out var group))
            {
                if (group.TryGetFromBack<TObject>(out var value))
                {
                    return (TObject)value;
                }
            }

            return default;
        }
        

        public void Initialize()
        {
            CheckPoolObjects(CancellationTokenSource.Token).Forget();
        }

        private async UniTaskVoid CheckPoolObjects(CancellationToken cancellationToken)
        {
            while (cancellationToken.IsCancellationRequested == false)
            {
                await UniTask.WaitForSeconds(
                    duration: CheckPoolTime, 
                    cancellationToken: cancellationToken);

                var time = DateTime.Now.Ticks;
                
                
                foreach (var group in _poolGroups)
                {
                    
                    while (group.Value.PoolObjects.Count > 0)
                    {
                        Debug.Log("Time: " + TimeSpan.FromTicks(time));
                        Debug.Log("ObjectLifetime: " + TimeSpan.FromTicks(group.Value.PoolObjects[0].ObjectLifetime));
                        Debug.Log("Substract: " + TimeSpan.FromTicks(group.Value.PoolObjects[0].ObjectLifetime - time));

                        if (group.Value.PoolObjects[0].ObjectLifetime < time)
                        {
                            group.Value.RemoveFromFront();
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
    
    

    public class PoolGroup<T>
    {
        // private readonly Dictionary<T, int> _objectsMap;
        public Deque<PoolObject<T>> PoolObjects { get; }

        /// <summary>
        /// Group name
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// Maximum object in group
        /// </summary>
        public int Capacity { get; }
        
        /// <summary>
        /// Duration of lifetime in seconds when object added to the pool
        /// </summary>
        public int ObjectLifetime { get; }


        public void AddObject<TObject>(TObject obj) where TObject : T
        {
            PoolObjects.AddToBack(new PoolObject<T>(obj, DateTime.Now.Ticks + (ObjectLifetime * TimeSpan.TicksPerSecond)));
        }

        public TObject GetFromBack<TObject>() where TObject : T
        {
            if (PoolObjects.Count <= 0)
                return default;
            
            return (TObject)PoolObjects.RemoveFromBack().Obj;
        }
        
        public TObject GetFromFront<TObject>() where TObject : T
        {
            if (PoolObjects.Count <= 0)
                return default;
            
            return (TObject)PoolObjects.RemoveFromFront().Obj;
        }

        
        public bool TryGetFromBack<TObject>(out T value) where TObject : T
        {
            value = GetFromBack<TObject>();

            return value != null;
        }
        
        public bool TryGetFromFront<TObject>(out T value) where TObject : T
        {
            value = GetFromFront<TObject>();

            return value != null;
        }

        public void RemoveFromFront()
        {
            PoolObjects.RemoveFromFront();
        }

        public void RemoveFromBack()
        {
            PoolObjects.RemoveFromBack();
        }

        public PoolGroup(string name, int capacity, int objectLifetime)
        {
            Name = name;
            Capacity = capacity;
            ObjectLifetime = objectLifetime;

            // _objectsMap = new Dictionary<T, int>(Capacity);
            PoolObjects = new Deque<PoolObject<T>>(Capacity);
        }
    }

    public class PoolObject<T>
    {
        public T Obj;
        public long ObjectLifetime;
        
        /// <param name="obj">Maximum object in group</param>
        /// <param name="objectLifetime">Duration of lifetime in seconds when object added to the pool</param>
        public PoolObject(T obj, long objectLifetime)
        {
            Obj = obj;
            ObjectLifetime = objectLifetime;
        }
    }
}