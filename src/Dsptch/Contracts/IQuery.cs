namespace Dsptch.Contracts;

/// <summary>
/// Marker interface for a query
/// </summary>
/// <typeparam name="TResult">The type of the response</typeparam>
public interface IQuery<out TResult> : IRequest<TResult> { }
