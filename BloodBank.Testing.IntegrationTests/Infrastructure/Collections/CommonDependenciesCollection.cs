using BloodBank.Testing.IntegrationTests.Infrastructure.Fixtures;

namespace BloodBank.Testing.IntegrationTests.Infrastructure.Collections;

[CollectionDefinition(nameof(CommonDependenciesCollection))]
public class CommonDependenciesCollection : ICollectionFixture<SharedTestFixture>;