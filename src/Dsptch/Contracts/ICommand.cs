namespace Dsptch.Contracts;

/// <summary>
/// Marker interface for a command
/// </summary>
/// <typeparam name="TResult">The type of the response</typeparam>
public interface ICommand<out TResult> : IRequest<TResult>;
