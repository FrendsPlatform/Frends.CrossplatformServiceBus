using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;

namespace Frends.ServiceBus.Send.Definitions;

/// <summary>
/// Class handles clients for the service bus. Enables cached connections to the service bus.
/// </summary>
public sealed class ServiceBusMessagingFactory : IDisposable
{
    private static readonly Lazy<ServiceBusMessagingFactory> InstanceHolder =
        new(() => new ServiceBusMessagingFactory());

    private static readonly object FactoryLock = new();

    private readonly ConcurrentDictionary<string, ServiceBusConnection> connections = new();

    private bool disposedValue = false; // To detect redundant calls

    private ServiceBusMessagingFactory()
    {
    }

    /// <summary>
    /// The ServiceBusMessagingFactory singleton instance
    /// </summary>
    /// <example>ServiceBusMessagingFactory.Instance</example>
    public static ServiceBusMessagingFactory Instance
    {
        get { return InstanceHolder.Value; }
    }

    /// <summary>
    /// Create a message sender for the given connection string and entity path
    /// </summary>
    /// <param name="connectionString">The connection string for the Service Bus</param>
    /// <param name="path">The entity path (queue or topic name)</param>
    /// <param name="timeout">The operation timeout</param>
    /// <returns>A MessageSender instance for sending messages</returns>
    public MessageSender GetMessageSender(string connectionString, string path, TimeSpan timeout)
    {
        return new MessageSender(GetCachedMessagingFactory(connectionString, timeout), path);
    }

    /// <summary>
    /// Dispose of the MessagingFactory and close all the cached connections
    /// </summary>
    [ExcludeFromCodeCoverage]
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(true);
    }

    /// <summary>
    /// Create new client for servicebus connection. This method is slow!
    /// </summary>
    /// <param name="connectionString">Connection string</param>
    /// <param name="operationTimeoutForClients">Operation timeout for clients</param>
    /// <returns>Object that can handle messaging to the service bus</returns>
    internal static ServiceBusConnection CreateConnectionWithTimeout(string connectionString, TimeSpan operationTimeoutForClients)
    {
        var connBuilder = new ServiceBusConnectionStringBuilder(connectionString)
        {
            OperationTimeout = operationTimeoutForClients,
        };

        var connection = new ServiceBusConnection(connBuilder) { RetryPolicy = RetryPolicy.Default };

        return connection;
    }

    private ServiceBusConnection GetCachedMessagingFactory(string connectionString, TimeSpan timeout)
    {
        var key = $"{timeout.TotalSeconds}-{connectionString}";

        if (!connections.ContainsKey(key))
        {
            lock (FactoryLock) // TODO: change double check
            {
                if (!connections.ContainsKey(key))
                    connections.TryAdd(key, CreateConnectionWithTimeout(connectionString, timeout));
            }
        }

        return connections[key];
    }

    [ExcludeFromCodeCoverage]
    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            var factoriesToClose = connections.ToList();
            connections.Clear();

            if (disposing)
            {
                // TODO: dispose managed state (managed objects).
            }

            foreach (var item in factoriesToClose)
            {
                try
                {
                    item.Value.CloseAsync().Wait();
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Error when aborting messaging factory connection " + ex);
                }
            }

            disposedValue = true;
        }
    }

    // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
}