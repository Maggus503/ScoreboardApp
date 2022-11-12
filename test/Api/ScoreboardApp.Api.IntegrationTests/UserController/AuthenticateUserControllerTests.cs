﻿using ScoreboardApp.Application.Authentication;
using System.Net;
using System.Net.Http.Json;

namespace ScoreboardApp.Api.IntegrationTests.UserController
{
    public class AuthenticateUserControllerTests : IClassFixture<ScoreboardAppApiFactory>
    {
        private const string EndpointUnderTest = "api/Users/authenticate";
        private const string ValidPassword = "Pa@@word123";

        private readonly HttpClient _apiClient;
        private readonly ScoreboardAppApiFactory _apiFactory;

        private readonly Faker<AuthenticateCommand> _authenticateCommandGenerator = new Faker<AuthenticateCommand>()
            .RuleFor(x => x.UserName, faker => faker.Internet.UserName())
            .RuleFor(x => x.Password, faker => ValidPassword);

        private readonly Faker<RegisterCommand> _registerCommandGenerator = new Faker<RegisterCommand>()
            .RuleFor(x => x.UserName, faker => faker.Internet.UserName())
            .RuleFor(x => x.Email, faker => faker.Internet.Email())
            .RuleFor(x => x.Password, faker => ValidPassword);

        public AuthenticateUserControllerTests(ScoreboardAppApiFactory apiFactory)
        {
            _apiFactory = apiFactory;
            _apiClient = apiFactory.CreateClient();
        }

        [Fact]
        public async Task Authenticate_AuthenticatesUsers_WhenCredentialsAreCorrect()
        {
            // Arrange
            // Register valid user
            var userCredentials = _registerCommandGenerator.Generate();
            var httpResponseRegistration = await _apiClient.PostAsJsonAsync("api/Users/register", userCredentials);
            httpResponseRegistration.StatusCode.Should().Be(HttpStatusCode.OK);

            var authenticateCommand = new AuthenticateCommand() { Password = userCredentials.Password, UserName = userCredentials.UserName };

            // Act

            var httpResponseAuthentication = await _apiClient.PostAsJsonAsync(EndpointUnderTest, authenticateCommand);

            // Assert

            httpResponseAuthentication.StatusCode.Should().Be(HttpStatusCode.OK);

            var authenticationResponse = await httpResponseAuthentication.Content.ReadFromJsonAsync<AuthenticateCommandResponse>();

            authenticationResponse.Should().NotBeNull();
            authenticationResponse!.RefreshToken.Should().NotBeNullOrEmpty();
            authenticationResponse!.Token.Should().NotBeNullOrEmpty();
            authenticationResponse!.RefreshTokenExpiry.Should().BeAfter(DateTime.Now);
        }

        [Fact]
        public async Task Authenticate_ReturnsError_WhenUserDoesNotExist()
        {
            // Arrange

            var authenticateCommand = _authenticateCommandGenerator.Generate();

            // Act

            var httpResponseAuthentication = await _apiClient.PostAsJsonAsync(EndpointUnderTest, authenticateCommand);

            // Assert

            httpResponseAuthentication.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Authenticate_ReturnsError_WhenCredentialsAreNotValid()
        {
            // Arrange
            // Register valid user
            var userCredentials = _registerCommandGenerator.Generate();
            var httpResponseRegistration = await _apiClient.PostAsJsonAsync("api/Users/register", userCredentials);
            httpResponseRegistration.StatusCode.Should().Be(HttpStatusCode.OK);

            var authenticateCommand = new AuthenticateCommand() { Password = "IncorrectPassword123!", UserName = userCredentials.UserName };

            // Act

            var httpResponseAuthentication = await _apiClient.PostAsJsonAsync(EndpointUnderTest, authenticateCommand);

            // Assert

            httpResponseAuthentication.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}