namespace ThinkAway.Core
{
    /// <summary>
    /// 封装了一种无返回值无参数的委托
    /// </summary>
    public delegate void MyAction();
    /// <summary>
    /// 封装了使用一个参数并且不返回值的委托
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    public delegate void MyAction<T>(T obj);
    /// <summary>
    /// 封装了使用两个参数的无返回值的委托
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    public delegate void MyAction<T1, T2>(T1 arg1,T2 arg2);



    /// <summary>
    /// 封装了具有指定返回值并使用一个参数的委托
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="arg"></param>
    /// <returns></returns>
    public delegate TResult MyFunc<TResult, T>(T arg);
    /// <summary>
    /// 封装了具有指定返回值并使用了两个参数的委托
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    /// <returns></returns>
    public delegate TResult MyFunc<TResult, T1, T2>(T1 arg1,T2 arg2);
    /// <summary>
    /// 封装了具有指定返回值并使用了三个参数的委托
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    /// <param name="arg3"></param>
    /// <returns></returns>
    public delegate TResult MyFunc<TResult, T1, T2, T3>(T1 arg1, T2 arg2,T3 arg3);
}
