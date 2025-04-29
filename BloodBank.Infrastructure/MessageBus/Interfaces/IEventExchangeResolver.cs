using BloodBank.Core.DomainEvents;

namespace BloodBank.Infrastructure.MessageBus.Interfaces;

public interface IEventExchangeResolver
{
    string Resolve(IDomainEvent domainEvent);
}