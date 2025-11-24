using System;
using Microsoft.Extensions.Logging;
using Moq;
using RoomBooking.Application.Interfaces;
using RoomBooking.Application.Services;
using RoomBooking.Domain.Models;

namespace RoomBooking.Application.Tests.Services.RoomService;

public abstract class RoomServiceTestBase
{
    protected readonly Mock<IRoomRepository> RoomRepoMock;
    protected readonly Mock<ILogger<RoomBooking.Application.Services.RoomService>> LoggerMock;
    protected readonly RoomBooking.Application.Services.RoomService Sut; // System Under Test

    protected RoomServiceTestBase()
    {
        RoomRepoMock = new Mock<IRoomRepository>();
        LoggerMock = new Mock<ILogger<RoomBooking.Application.Services.RoomService>>();
        Sut = new RoomBooking.Application.Services.RoomService(RoomRepoMock.Object, LoggerMock.Object);
    }

    protected Room CreateValidRoom()
    {
        return new Room
        {
            Id = 1,
            Name = "Raum 1",
            Capacity = 10
        };
    }
}