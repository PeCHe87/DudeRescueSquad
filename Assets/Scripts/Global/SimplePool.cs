///
/// Simple pooling for Unity.
///   Author: Martin "quill18" Glaude (quill18@quill18.com)
///   Extended: Simon "Draugor" Wagner (https://www.twitter.com/Draugor_/)
///   Latest Version: https://gist.github.com/quill18/5a7cfffae68892621267
///   License: CC0 (http://creativecommons.org/publicdomain/zero/1.0/)
///   UPDATES:
///     2018-01-04: - Added Extension Method for Despawn on GameObjects 
///                 - Changed the Member Lookup so it doesn't require a PoolMemberComponent anymore.
///                     - for that i added a HashSet which contains all PoolMemberIDs  (HashSet has O(1) contains operator)
///                     - PoolMemberIDs are just ints from GameObject.getInstanceID() which are unique for the GameObject 
///                        over the runtime of the game
///                 - Changed PoolDictionary from (Prefab, Pool) to (int, Pool) using Prefab.GetInstanceID
/// 	2015-04-16: Changed Pool to use a Stack generic.
/// 
/// Usage:
/// 
///   There's no need to do any special setup of any kind.
/// 
///   Instead of calling Instantiate(), use this:
///       SimplePool.Spawn(somePrefab, somePosition, someRotation);
/// 
///   Instead of destroying an object, use this:
///       SimplePool.Despawn(myGameObject);
///   or this:
///       myGameObject.Despawn();
/// 
///   If desired, you can preload the pool with a number of instances:
///       SimplePool.Preload(somePrefab, 20);
/// 
/// Remember that Awake and Start will only ever be called on the first instantiation
/// and that member variables won't be reset automatically.  You should reset your
/// object yourself after calling Spawn().  (i.e. You'll have to do things like set
/// the object's HPs to max, reset animation states, etc...)
/// 
/// 
/// 

using UnityEngine;
using System.Collections.Generic;

namespace DudeRescueSquad.Global
{
    public static class SimplePool
    {
        // You can avoid resizing of the Stack's internal data by
        // setting this to a number equal to or greater to what you
        // expect most of your pool sizes to be.
        // Note, you can also use Preload() to set the initial size
        // of a pool -- this can be handy if only some of your pools
        // are going to be exceptionally large (for example, your bullets.)
        public const int DEFAULT_POOL_SIZE = 3;

        //Pool parent gameobject, just to keep the heirarchy tidy
        static GameObject Parent;

        /// <summary>
        /// The Pool class represents the pool for a particular prefab.
        /// </summary>
        public class Pool
        {
            // We append an id to the name of anything we instantiate.
            // This is purely cosmetic.
            private int _nextId = 1;
            // The structure containing our inactive objects.
            // Why a Stack and not a List? Because we'll never need to
            // pluck an object from the start or middle of the array.
            // We'll always just grab the last one, which eliminates
            // any need to shuffle the objects around in memory.
            private readonly Stack<GameObject> _inactive;
            //A Hashset which contains all GetInstanceIDs from the instantiated GameObjects 
            //so we know which GameObject is a member of this pool.
            public readonly HashSet<int> MemberIDs;
            // The prefab that we are pooling
            private readonly GameObject _prefab;

            // Constructor
            public Pool(GameObject prefab, int initialQty)
            {
                _prefab = prefab;
                // If Stack uses a linked list internally, then this
                // whole initialQty thing is a placebo that we could
                // strip out for more minimal code. But it can't *hurt*.
                _inactive = new Stack<GameObject>(initialQty);
                MemberIDs = new HashSet<int>();
            }

            // Spawn an object from our pool
            public GameObject Spawn(Vector3 pos, Quaternion rot, bool enable = true)
            {
                GameObject obj;
                if (_inactive.Count == 0)
                {
                    // We don't have an object in our pool, so we
                    // instantiate a whole new object.
                    obj = GameObject.Instantiate<GameObject>(_prefab, pos, rot);
                    obj.name = _prefab.name;
                    GameObject.DontDestroyOnLoad(obj);
                    // Add the unique GameObject ID to our MemberHashset so we know this GO belongs to us.
                    MemberIDs.Add(obj.GetInstanceID());
                }
                else
                {
                    // Grab the last object in the inactive array
                    obj = _inactive.Pop();

                    if (obj == null)
                    {
                        // The inactive object we expected to find no longer exists.
                        // The most likely causes are:
                        //   - Someone calling Destroy() on our object
                        //   - A scene change (which will destroy all our objects).
                        //     NOTE: This could be prevented with a DontDestroyOnLoad
                        //	   if you really don't want this.
                        // No worries -- we'll just try the next one in our sequence.

                        return Spawn(pos, rot, enable);
                    }
                }
                obj.transform.SetParent(null, false);
                obj.transform.position = pos;
                obj.transform.rotation = rot;
                if (enable)
                {
                    obj.SetActive(true);
                }
                return obj;
            }

            // Return an object to the inactive pool.
            public void Despawn(GameObject obj, bool removeFromParent)
            {
                if (removeFromParent)
                {
                    obj.transform.SetParent(null);
                }

                if (obj.activeSelf)
                {
                    obj.SetActive(false);
                    // Since Stack doesn't have a Capacity member, we can't control
                    // the growth factor if it does have to expand an internal array.
                    // On the other hand, it might simply be using a linked list 
                    // internally.  But then, why does it allow us to specify a size
                    // in the constructor? Maybe it's a placebo? Stack is weird.
                    _inactive.Push(obj);
                }
            }
        }

        //create parent
        public static void CreateParent(GameObject parent)
        {
            if (Parent == null)
            {
                Parent = parent;
            }
        }

        // All of our pools
        public static Dictionary<int, Pool> _pools;

        /// <summary>
        /// Initialize our dictionary.
        /// </summary>
        private static bool Init(GameObject prefab = null, int qty = DEFAULT_POOL_SIZE)
        {
            if (_pools == null)
                _pools = new Dictionary<int, Pool>();

            if (prefab != null)
            {
                //changed from (prefab, Pool) to (int, Pool) which should be faster if we have 
                //many different prefabs.
                var prefabID = prefab.GetInstanceID();
                if (!_pools.ContainsKey(prefabID))
                {
                    _pools[prefabID] = new Pool(prefab, qty);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// If you want to preload a few copies of an object at the start
        /// of a scene, you can use this. Really not needed unless you're
        /// going to go from zero instances to 100+ very quickly.
        /// Could technically be optimized more, but in practice the
        /// Spawn/Despawn sequence is going to be pretty darn quick and
        /// this avoids code duplication.
        /// </summary>
        static public void Preload(GameObject prefab, int qty = 1)
        {
            if (!Init(prefab, qty))
                return;
            // Make an array to grab the objects we're about to pre-spawn.
            var obs = new GameObject[qty];
            for (int i = 0; i < qty; i++)
                obs[i] = Spawn(prefab, Vector3.zero, Quaternion.identity, false);

            // Now despawn them all.
            for (int i = 0; i < qty; i++)
                Despawn(obs[i], true);
        }

        /// <summary>
        /// Spawns a copy of the specified prefab (instantiating one if required).
        /// NOTE: Remember that Awake() or Start() will only run on the very first
        /// spawn and that member variables won't get reset.  OnEnable will run
        /// after spawning -- but remember that toggling IsActive will also
        /// call that function.
        /// </summary>
        static public GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot, bool enable = true)
        {
            Init(prefab);

            return _pools[prefab.GetInstanceID()].Spawn(pos, rot, enable);
        }
        static public T Spawn<T>(T prefab, Vector3 pos, Quaternion rot, bool enable = true) where T : MonoBehaviour
        {
            return Spawn(prefab.gameObject, pos, rot, enable).GetComponent<T>();
        }
        static public T Spawn<T>(T prefab, Transform parent, Vector3? localScale = null) where T : MonoBehaviour
        {
            T ob = Spawn(prefab, parent.position, parent.rotation);
            ob.transform.SetParent(parent);
            ob.transform.localScale = localScale ?? Vector3.one;
            return ob;
        }

        static public T Spawn<T>(T prefab, Transform parent, bool enable) where T : MonoBehaviour
        {
            T ob = Spawn(prefab, parent.position, parent.rotation, enable);
            ob.transform.SetParent(parent);
            return ob;
        }

        static public GameObject Spawn(GameObject prefab, Transform parent, Vector3? localScale = null)
        {
            var ob = Spawn(prefab, parent.position, parent.rotation);
            ob.transform.SetParent(parent);
            ob.transform.localScale = localScale ?? Vector3.one;
            return ob;
        }
        /// <summary>
        /// Despawn the specified gameobject back into its pool.
        /// </summary>
        static public void Despawn(GameObject obj, bool removeFromParent)
        {
            if (_pools == null)
            {
                Debug.Log("Object '" + obj.name + "' despawned before pools initialized so destroying it.");
                GameObject.Destroy(obj);
            }
            Pool p = null;
            foreach (var pool in _pools.Values)
            {
                if (pool.MemberIDs.Contains(obj.GetInstanceID()))
                {
                    p = pool;
                    break;
                }
            }

            if (p == null)
            {
                Debug.Log("Object '" + obj.name + "' wasn't spawned from a pool. Destroying it instead.");
                GameObject.Destroy(obj);
            }
            else
            {
                p.Despawn(obj, removeFromParent);
            }

        }
    }

    public static class SimplePoolGameObjectExtensions
    {
        public static void Despawn(this GameObject go, bool removeFromParent)
        {
            SimplePool.Despawn(go, removeFromParent);
        }
        public static void DespawnAll(this Transform transform, bool removeFromParent)
        {
            int c = transform.childCount;
            while (c > 0)
            {
                SimplePool.Despawn(transform.GetChild(0).gameObject, removeFromParent);
                c--;
            }
        }
    }
}