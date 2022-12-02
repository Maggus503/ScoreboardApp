﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScoreboardApp.Application.DTOs.Enums;
using ScoreboardApp.Application.Habits.Commands;
using ScoreboardApp.Application.HabitTrackers.Commands;

namespace ScoreboardApp.Api.IntegrationTests.CompletionHabitsController
{
    public class CreateCompletionHabitsControllerTests : IClassFixture<ScoreboardAppApiFactory>
    {
        private const string Endpoint = TestHelpers.Endpoints.CompletionHabits;
        private readonly HttpClient _apiClient;
        private readonly ScoreboardAppApiFactory _apiFactory;

        private readonly Faker<CreateHabitTrackerCommand> _createTrackerCommandGenerator = new Faker<CreateHabitTrackerCommand>()
            .RuleFor(x => x.Title, faker => faker.Random.String2(1, 200))
            .RuleFor(x => x.Priority, faker => PriorityMapping.NotSet);

        private readonly Faker<CreateCompletionHabitCommand> _createCompletionHabitCommandGenerator = new Faker<CreateCompletionHabitCommand>()
            .RuleFor(x => x.Description, faker => faker.Random.String2(1, 400))
            .RuleFor(x => x.Title, faker => faker.Random.String2(1, 200));

        public CreateCompletionHabitsControllerTests(ScoreboardAppApiFactory apiFactory)
        {
            _apiFactory = apiFactory;
            _apiClient = apiFactory.CreateClient();

            _apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiFactory.TestUser1.Token);
        }

        [Fact]
        public async Task Create_CreatesHabit_WhenDataIsValid()
        {
            // Arrange
            var habitTracker = await TestHelpers.CreateHabitTracker(_apiClient, _createTrackerCommandGenerator);

            var habit = _createCompletionHabitCommandGenerator.Clone()
                .RuleFor(x => x.HabitTrackerId, faker => habitTracker!.Id)
                .Generate();

            // Act 
            var createHabitResponse = await _apiClient.PostAsJsonAsync(Endpoint, habit);

            // Assert
            createHabitResponse.Should().HaveStatusCode(HttpStatusCode.Created);

            var createdObject = await createHabitResponse.Content.ReadFromJsonAsync<CreateCompletionHabitCommandResponse>();

            createHabitResponse.Headers.Location!.ToString().Should()
                .Be($"http://localhost/{Endpoint}?Id={createdObject!.Id}");

            createdObject.Should().BeEquivalentTo(habit);
        }

        [Fact]
        public async Task Create_ReturnsValidationError_WhenTitleIsTooLong()
        {
            // Arrange
            var habitTracker = await TestHelpers.CreateHabitTracker(_apiClient, _createTrackerCommandGenerator);

            var habit = _createCompletionHabitCommandGenerator.Clone()
                .RuleFor(x => x.Title, faker => faker.Random.String2(201))
                .RuleFor(x => x.HabitTrackerId, faker => habitTracker!.Id)
                .Generate();

            // Act 
            var createHabitResponse = await _apiClient.PostAsJsonAsync(Endpoint, habit);

            // Assert
            createHabitResponse.Should().HaveStatusCode(HttpStatusCode.BadRequest);

            var createdObject = await createHabitResponse.Content.ReadFromJsonAsync<ValidationProblemDetails>();

            createdObject.Should().NotBeNull();
            var errors = createdObject!.Errors;

            errors.Should().ContainKey("Title").WhoseValue.Contains("The Title length cannot exceed 200 characters.");
        }

        [Fact]
        public async Task Create_ReturnsValidationError_WhenTitleIsEmpty()
        {
            // Arrange
            var habitTracker = await TestHelpers.CreateHabitTracker(_apiClient, _createTrackerCommandGenerator);

            var habit = _createCompletionHabitCommandGenerator.Clone()
                .RuleFor(x => x.Title, faker => string.Empty)
                .RuleFor(x => x.HabitTrackerId, faker => habitTracker!.Id)
                .Generate();

            // Act 
            var createHabitResponse = await _apiClient.PostAsJsonAsync(Endpoint, habit);

            // Assert
            createHabitResponse.Should().HaveStatusCode(HttpStatusCode.BadRequest);

            var createdObject = await createHabitResponse.Content.ReadFromJsonAsync<ValidationProblemDetails>();

            createdObject.Should().NotBeNull();
            var errors = createdObject!.Errors;

            errors.Should().ContainKey("Title").WhoseValue.Contains("The Title cannot be null or empty.");
        }

        [Fact]
        public async Task Create_ReturnsValidationError_WhenDescriptionIsTooLong()
        {
            // Arrange
            var habitTracker = await TestHelpers.CreateHabitTracker(_apiClient, _createTrackerCommandGenerator);

            var habit = _createCompletionHabitCommandGenerator.Clone()
                .RuleFor(x => x.Description, faker => faker.Random.String2(401))
                .RuleFor(x => x.HabitTrackerId, faker => habitTracker!.Id)
                .Generate();

            // Act 
            var createHabitResponse = await _apiClient.PostAsJsonAsync(Endpoint, habit);

            // Assert
            createHabitResponse.Should().HaveStatusCode(HttpStatusCode.BadRequest);

            var createdObject = await createHabitResponse.Content.ReadFromJsonAsync<ValidationProblemDetails>();

            createdObject.Should().NotBeNull();
            var errors = createdObject!.Errors;

            errors.Should().ContainKey("Description").WhoseValue.Contains("The Description length cannot exceed 400 characters.");
        }

        [Fact]
        public async Task Create_ReturnsUnauthorized_WhenUserIsNotLogedIn()
        {
            // Arrange
            var habitTracker = await TestHelpers.CreateHabitTracker(_apiClient, _createTrackerCommandGenerator);

            var habit = _createCompletionHabitCommandGenerator.Clone()
                .RuleFor(x => x.HabitTrackerId, faker => habitTracker!.Id)
                .Generate();

            var unauthenticatedClient = _apiFactory.CreateClient();

            // Act 
            var createHabitResponse = await unauthenticatedClient.PostAsJsonAsync(Endpoint, habit);

            // Assert
            createHabitResponse.Should().HaveStatusCode(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Create_ReturnsValidationError_WhenHabitTrackerIdIsNotValid()
        {
            // Arrange
            var habitTracker = await TestHelpers.CreateHabitTracker(_apiClient, _createTrackerCommandGenerator);

            var habit = _createCompletionHabitCommandGenerator.Clone()
                .RuleFor(x => x.HabitTrackerId, faker => Guid.NewGuid())
                .Generate();

            // Act 
            var createHabitResponse = await _apiClient.PostAsJsonAsync(Endpoint, habit);

            // Assert
            createHabitResponse.Should().HaveStatusCode(HttpStatusCode.BadRequest);

            var createdObject = await createHabitResponse.Content.ReadFromJsonAsync<ValidationProblemDetails>();

            createdObject.Should().NotBeNull();
            var errors = createdObject!.Errors;

            errors.Should().ContainKey("HabitTrackerId").WhoseValue.Contains("The HabitTrackerId must be a valid id.");
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenUserDoesntOwnTheHabitTracker()
        {
            // Arrange
            var habitTracker = await TestHelpers.CreateHabitTracker(_apiClient, _createTrackerCommandGenerator);

            var habit = _createCompletionHabitCommandGenerator.Clone()
                .RuleFor(x => x.HabitTrackerId, faker => Guid.NewGuid())
                .Generate();

            var secondUsersClient = _apiFactory.CreateClient();
            secondUsersClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiFactory.TestUser2.Token);

            // Act 
            var createHabitResponse = await secondUsersClient.PostAsJsonAsync(Endpoint, habit);

            // Assert
            createHabitResponse.Should().HaveStatusCode(HttpStatusCode.BadRequest);

            var createdObject = await createHabitResponse.Content.ReadFromJsonAsync<ValidationProblemDetails>();

            createdObject.Should().NotBeNull();
            var errors = createdObject!.Errors;

            errors.Should().ContainKey("HabitTrackerId").WhoseValue.Contains("The HabitTrackerId must be a valid id.");
        }
    }
}
