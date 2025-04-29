using BloodBank.Infrastructure.MessageBus.TopologyConfig.Interfaces;

namespace BloodBank.Infrastructure.MessageBus.TopologyConfig;

public class TopologyDefinition : IEventBusTopologyDefinition
{
    public string DeadLetterExchange => "bloodbank.dlx";
    public string DonationsExchange => "bloodbank.donations";
    
    public string HealthCheckExchange  => "bloodbank.healthcheck";
    public string DeadLetterQueue => "bloodbank.deadletter";
    

}