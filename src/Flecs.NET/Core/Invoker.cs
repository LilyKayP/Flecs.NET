using System;
using Flecs.NET.Utilities;
using static Flecs.NET.Bindings.Native;

namespace Flecs.NET.Core
{
    /// <summary>
    ///     A static class for holding callback invokers.
    /// </summary>
    public static unsafe class Invoker
    {
        /// <summary>
        ///     Invokes an iter callback using a delegate.
        /// </summary>
        /// <param name="iter"></param>
        /// <param name="callback"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Iter(ecs_iter_t* iter, Ecs.IterCallback callback)
        {
            Macros.TableLock(iter->world, iter->table);
            callback(new Iter(iter));
            Macros.TableUnlock(iter->world, iter->table);
        }

        /// <summary>
        ///     Invokes an each callback using a delegate.
        /// </summary>
        /// <param name="iter"></param>
        /// <param name="callback"></param>
        public static void EachEntity(ecs_iter_t* iter, Ecs.EachEntityCallback callback)
        {
            Macros.TableLock(iter->world, iter->table);

            ecs_world_t* world = iter->world;
            int count = iter->count;

            Assert.True(count > 0, "No entities returned, use Each() without Entity argument");

            for (int i = 0; i < count; i++)
                callback(new Entity(world, iter->entities[i]));

            Macros.TableUnlock(iter->world, iter->table);
        }

#if NET5_0_OR_GREATER
        /// <summary>
        ///     Invokes an iter callback using a managed function pointer.
        /// </summary>
        /// <param name="iter"></param>
        /// <param name="callback"></param>
        public static void Iter(ecs_iter_t* iter, delegate* managed<Iter, void> callback)
        {
            Macros.TableLock(iter->world, iter->table);
            callback(new Iter(iter));
            Macros.TableUnlock(iter->world, iter->table);
        }

        /// <summary>
        ///     Invokes an iter callback using an unmanaged function pointer.
        /// </summary>
        /// <param name="iter"></param>
        /// <param name="callback"></param>
        public static void Iter(ecs_iter_t* iter, delegate* unmanaged<Iter, void> callback)
        {
            Macros.TableLock(iter->world, iter->table);
            callback(new Iter(iter));
            Macros.TableUnlock(iter->world, iter->table);
        }

        /// <summary>
        ///     Invokes an each callback using a managed function pointer.
        /// </summary>
        /// <param name="iter"></param>
        /// <param name="callback"></param>
        public static void EachEntity(ecs_iter_t* iter, delegate* managed<Entity, void> callback)
        {
            Macros.TableLock(iter->world, iter->table);

            ecs_world_t* world = iter->world;
            int count = iter->count;

            Assert.True(count > 0, "No entities returned, use Each() without Entity argument");

            for (int i = 0; i < count; i++)
                callback(new Entity(world, iter->entities[i]));

            Macros.TableUnlock(iter->world, iter->table);
        }

        /// <summary>
        ///     Invokes an each callback using an unmanaged function pointer.
        /// </summary>
        /// <param name="iter"></param>
        /// <param name="callback"></param>
        public static void EachEntity(ecs_iter_t* iter, delegate* unmanaged<Entity, void> callback)
        {
            Macros.TableLock(iter->world, iter->table);

            ecs_world_t* world = iter->world;
            int count = iter->count;

            Assert.True(count > 0, "No entities returned, use Each() without Entity argument");

            for (int i = 0; i < count; i++)
                callback(new Entity(world, iter->entities[i]));

            Macros.TableUnlock(iter->world, iter->table);
        }
#endif
    }
}
