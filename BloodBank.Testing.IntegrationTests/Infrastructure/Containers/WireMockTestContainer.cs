using DotNet.Testcontainers.Builders;
 using WireMock.Net.Testcontainers;
 
 namespace BloodBank.Testing.IntegrationTests.Infrastructure.Containers;
 
 public class WireMockTestContainer
 {
     public static WireMockContainer CreateContainer()
     {
         return new WireMockContainerBuilder()
             .WithAutoRemove(true)
             .WithCleanUp(true)
             .Build();
     }
 }