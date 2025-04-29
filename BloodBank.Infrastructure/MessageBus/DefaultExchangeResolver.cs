using BloodBank.Core.DomainEvents;
using BloodBank.Core.DomainEvents.Donations;
using BloodBank.Infrastructure.MessageBus.Interfaces;
using BloodBank.Infrastructure.MessageBus.TopologyConfig.Interfaces;

namespace BloodBank.Infrastructure.MessageBus;

public class DefaultExchangeResolver : IEventExchangeResolver
{
    private IEventBusTopologyDefinition _topologyDefinition;
    public DefaultExchangeResolver(IEventBusTopologyDefinition topologyDefinition)
    {
        _topologyDefinition = topologyDefinition;
    }
    public string Resolve(IDomainEvent domainEvent)
    {
        return domainEvent switch
        {
            DonationRegistered => _topologyDefinition.DonationsExchange
        };
    }
}