using BloodBank.Testing.IntegrationTests.Infrastructure.Fixtures;

namespace BloodBank.Testing.IntegrationTests.Infrastructure.Collections;

[CollectionDefinition(nameof(OutboxTestsCollection), DisableParallelization = true)]
public class OutboxTestsCollection : ICollectionFixture<SharedTestFixture>;
