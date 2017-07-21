
namespace Assets.Scripts.Game.Events
{
    public class EventDispatcherInterface
    {
        protected EventDispatcher dispatcher = EventDispatcher.Instance;

        protected void Dispatch(EventConstant code)
        {
            dispatcher.Dispatch(code);
        }

        protected void AddEventListener(EventConstant code, EventDispatcher.EventCallback handler)
        {
            dispatcher.AddEventListener(code, handler);
        }

        protected void RemoveEventListener(EventConstant code, EventDispatcher.EventCallback handler)
        {
            dispatcher.RemoveEventListener(code, handler);
        }

        protected void Dispatch<T1>(EventConstant code, T1 t1)
        {
            dispatcher.Dispatch<T1>(code, t1);
        }

        protected void AddEventListener<T1>(EventConstant code, EventDispatcher.EventCallback<T1> handler)
        {
            dispatcher.AddEventListener<T1>(code, handler);
        }

        protected void RemoveEventListener<T1>(EventConstant code, EventDispatcher.EventCallback<T1> handler)
        {
            dispatcher.RemoveEventListener<T1>(code, handler);
        }

        protected void Dispatch<T1, T2>(EventConstant code, T1 t1, T2 t2)
        {
            dispatcher.Dispatch<T1, T2>(code, t1, t2);
        }

        protected void AddEventListener<T1, T2>(EventConstant code, EventDispatcher.EventCallback<T1, T2> handler)
        {
            dispatcher.AddEventListener<T1, T2>(code, handler);
        }

        protected void RemoveEventListener<T1, T2>(EventConstant code, EventDispatcher.EventCallback<T1, T2> handler)
        {
            dispatcher.RemoveEventListener<T1, T2>(code, handler);
        }

        protected void Dispatch<T1, T2, T3>(EventConstant code, T1 t1, T2 t2, T3 t3)
        {
            dispatcher.Dispatch<T1, T2, T3>(code, t1, t2, t3);
        }

        protected void AddEventListener<T1, T2, T3>(EventConstant code, EventDispatcher.EventCallback<T1, T2, T3> handler)
        {
            dispatcher.AddEventListener<T1, T2, T3>(code, handler);
        }

        protected void RemoveEventListener<T1, T2, T3>(EventConstant code, EventDispatcher.EventCallback<T1, T2, T3> handler)
        {
            dispatcher.RemoveEventListener<T1, T2, T3>(code, handler);
        }
    }
}
