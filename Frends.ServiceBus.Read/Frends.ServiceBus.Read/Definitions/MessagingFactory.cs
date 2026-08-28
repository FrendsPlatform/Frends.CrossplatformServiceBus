using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;

namespace Frends.ServiceBus.Read.Definitions;

/// <summary>
/// Class handles clients for the service bus. Enables cached connections to the service bus.
/// </summary>
public sealed class ServiceBusMessagingFactory : IDisposable
{
    private static readonly Lazy<ServiceBusMessagingFactory> InstanceHolder = new(() => new ServiceBusMessagingFactory());

    private static readonly object FactoryLock = new();

    private readonly ConcurrentDictionary<string, ServiceBusConnection> connections = new();

    private bool disposedValue;

    private ServiceBusMessagingFactory()
    {
    }

    /// <summary>
    /// The ServiceBusMessagingFactory singleton instance
    /// </summary>
    /// <example>ServiceBusMessagingFactory.Instance</example>
    public static ServiceBusMessagingFactory Instance => InstanceHolder.Value;

    /// <summary>
    /// Create message receiver for the given connection string and entity path
    /// </summary>
    /// <param name="connectionString">Connection string</param>
    /// <param name="path">Name of the queue</param>
    /// <param name="timeout">TimeoutSeconds</param>
    /// <returns>A MessageReceiver for the specified path.</returns>
    public MessageReceiver GetMessageReceiver(string connectionString, string path, TimeSpan timeout)
    {
        var receiver = new MessageReceiver(GetCachedMessagingFactory(connectionString, timeout), path, receiveMode: ReceiveMode.ReceiveAndDelete);
        return receiver;
    }

    /// <summary>
    /// Create a message sender for the given connection string and entity path
    /// </summary>
    /// <param name="connectionString">Connection string to the Service Bus namespace.</param>
    /// <param name="path">Name of the queue or topic.</param>
    /// <param name="timeout">Operation timeout for the sender.</param>
    /// <returns>A MessageSender for the specified path.</returns>
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

    [ExcludeFromCodeCoverage]
    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            var factoriesToClose = connections.ToList();
            connections.Clear();

            if (disposing)
            {
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

    private ServiceBusConnection GetCachedMessagingFactory(string connectionString, TimeSpan timeout)
    {
        var key = $"{timeout.TotalSeconds}-{connectionString}";

        if (!connections.ContainsKey(key))
        {
            lock (FactoryLock)
            {
                if (!connections.ContainsKey(key))
                    connections.TryAdd(key, CreateConnectionWithTimeout(connectionString, timeout));
            }
        }

        return connections[key];
    }
}