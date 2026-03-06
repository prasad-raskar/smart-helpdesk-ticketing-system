using FluentAssertions;
using Helpdesk.Application.DTOs;
using Helpdesk.Application.Interfaces;
using Helpdesk.Application.Services;
using Helpdesk.Domain.Entities;
using Helpdesk.Domain.Enums;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Helpdesk.Tests;

public class TicketServiceTests
{
    private readonly Mock<ITicketRepository> _ticketRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<ILogger<TicketService>> _loggerMock;
    private readonly Mock<IMemoryCache> _cacheMock;
    private readonly TicketService _ticketService;

    public TicketServiceTests()
    {
        _ticketRepoMock = new Mock<ITicketRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _loggerMock = new Mock<ILogger<TicketService>>();
        _cacheMock = new Mock<IMemoryCache>();
        
        _ticketService = new TicketService(
            _ticketRepoMock.Object, 
            _userRepoMock.Object, 
            _loggerMock.Object,
            _cacheMock.Object);
    }

    [Fact]
    public async Task CreateTicketAsync_ShouldReturnTicket_WhenRequestIsValid()
    {
        // Arrange
        var request = new CreateTicketRequest
        {
            Title = "Network Issue",
            Description = "Internet is not working for 10 users.",
            Priority = TicketPriority.High,
            CreatedByUserId = 1
        };

        var user = new User { Id = 1, Username = "testuser" };
        _userRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(user);

        // Act
        var result = await _ticketService.CreateTicketAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be(request.Title);
        result.Status.Should().Be(TicketStatus.Open);
        _ticketRepoMock.Verify(x => x.AddAsync(It.IsAny<Ticket>()), Times.Once);
        _ticketRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateTicketAsync_ShouldThrowException_WhenUserNotFound()
    {
        // Arrange
        var request = new CreateTicketRequest { CreatedByUserId = 99 };
        _userRepoMock.Setup(x => x.GetByIdAsync(99)).ReturnsAsync((User)null!);

        // Act
        var act = () => _ticketService.CreateTicketAsync(request);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Creator user not found.");
    }

    [Fact]
    public async Task AssignTicketAsync_ShouldUpdateStatusToInProgress_WhenSuccess()
    {
        // Arrange
        var ticket = new Ticket { Id = 1, Status = TicketStatus.Open };
        var assignee = new User { Id = 2, Username = "agent1" };
        var request = new AssignTicketRequest { TicketId = 1, AssignedToUserId = 2, AssignedByUserId = 3 };

        _ticketRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(ticket);
        _userRepoMock.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(assignee);

        // Act
        await _ticketService.AssignTicketAsync(request);

        // Assert
        ticket.Status.Should().Be(TicketStatus.InProgress);
        _ticketRepoMock.Verify(x => x.UpdateAsync(ticket), Times.Once);
        _ticketRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateTicketStatusAsync_ShouldThrowException_WhenTicketIsClosed()
    {
        // Arrange
        var ticket = new Ticket { Id = 1, Status = TicketStatus.Closed };
        _ticketRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(ticket);

        // Act
        var act = () => _ticketService.UpdateTicketStatusAsync(1, "Open");

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Closed tickets cannot be modified.");
    }
}
