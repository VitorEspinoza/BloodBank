namespace BloodBank.Infrastructure.MessageBus.TopologyConfig.Interfaces;

public interface IEventBusTopologyDefinition
{
    public string DeadLetterExchange { get; }
    public string DonationsExchange { get; }
    public string HealthCheckExchange { get; }
    public string DeadLetterQueue { get; }
    
}