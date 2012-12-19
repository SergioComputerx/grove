﻿namespace Grove.Infrastructure
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Reflection;
  using Castle.DynamicProxy;

  public class Publisher : ICopyable
  {
    private ChangeTracker _changeTracker;
    private Dictionary<Type, List<Type>> _handlersByType = new Dictionary<Type, List<Type>>();
    private Dictionary<Type, TrackableList<object>> _subscribers = new Dictionary<Type, TrackableList<object>>();

    private Publisher() {}

    public Publisher(Assembly assembly, ChangeTracker changeTracker, string ns = null)
    {
      _changeTracker = changeTracker;
      Func<Type, bool> namespaceFilter = type => ns == null
        || type.Namespace.Equals(ns, StringComparison.InvariantCultureIgnoreCase);

      MapHandlersToTypes(assembly, changeTracker, namespaceFilter);
    }

    public Publisher(ChangeTracker changeTracker) : this(Assembly.GetExecutingAssembly(), changeTracker) {}

    public void Copy(object original, CopyService copyService)
    {
      var org = (Publisher) original;
      _changeTracker = copyService.Copy(org._changeTracker);
      _handlersByType = org._handlersByType;
      _subscribers = new Dictionary<Type, TrackableList<object>>();

      foreach (var subscriber in org._subscribers)
      {
        var subscribers = subscriber.Value
          .Where(x => IsUiComponent(x) == false)
          .Select(copyService.Copy);

        _subscribers.Add(subscriber.Key, new TrackableList<object>(subscribers, _changeTracker));
      }
    }

    private void MapHandlersToTypes(Assembly assembly, ChangeTracker changeTracker, Func<Type, bool> typeFilter)
    {
      var types = assembly.GetTypes()
        .Where(typeFilter)
        .Where(x => x.Implements<IReceive>())
        .ToList();

      foreach (var type in types)
      {
        var handlers = type.GetInterfaces()
          .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof (IReceive<>))
          .ToList();

        foreach (var handler in handlers)
        {
          var messageType = handler.GetGenericArguments()[0];

          List<Type> allHandlers;
          if (!_handlersByType.TryGetValue(type, out allHandlers))
          {
            allHandlers = new List<Type>();
            _handlersByType[type] = allHandlers;
          }

          if (!_subscribers.ContainsKey(messageType))
            _subscribers.Add(messageType, new TrackableList<object>(changeTracker));


          allHandlers.Add(messageType);
        }
      }
    }

    public void Publish<TMessage>(TMessage message)
    {
      var subscribers = _subscribers[typeof (TMessage)].ToList();
      var orderedSubscribers = new List<IOrderedReceive<TMessage>>();
            
      foreach (IReceive<TMessage> subscriber in subscribers)
      {
        var orderered = subscriber as IOrderedReceive<TMessage>;
        if (orderered != null)
        {
          orderedSubscribers.Add(orderered);    
          continue;
        }
                
        subscriber.Receive(message);
      }

      foreach (var orderedSubscriber in orderedSubscribers.OrderBy(x => x.Order))
      {
        orderedSubscriber.Receive(message);
      }
    }

    public void Subscribe(object instance)
    {
      List<Type> handlers = GetHandlers(instance);

      if (handlers == null)
        return;

      foreach (var handler in handlers)
      {
        _subscribers[handler].Add(instance);
      }
    }

    private List<Type> GetHandlers(object instance) {
      List<Type> handlers;
      _handlersByType.TryGetValue(ProxyUtil.GetUnproxiedType(instance), out handlers);        
      return handlers;
    }

    public void Unsubscribe(object instance)
    {
      List<Type> handlers = GetHandlers(instance);

      if (handlers == null)
        return;

      foreach (var handler in handlers)
      {
        _subscribers[handler].Remove(instance);
      }
    }

    private static bool IsUiComponent(object target)
    {
      var targetType = ProxyUtil.GetUnproxiedType(target);
      var isUi = targetType.Namespace.StartsWith("Grove.Ui");

      return isUi;
    }
  }
}