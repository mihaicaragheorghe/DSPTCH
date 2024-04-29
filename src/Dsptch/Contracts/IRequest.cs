namespace Dsptch.Contracts;

/// <summary>
/// Marker interface for a request
/// </summary>
/// <typeparam name="TResult">The type of the result.</typeparam>
public interface IRequest<out TResult>;