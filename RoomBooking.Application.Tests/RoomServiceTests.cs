using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using RoomBooking.Application.Interfaces;
using RoomBooking.Application.Services;
using RoomBooking.Domain.Models;
using Xunit;

namespace RoomBooking.Application.Tests.Services
{
    public class RoomServiceTests
    {
        private readonly Mock<IRoomRepository> _roomRepoMock;
        private readonly Mock<ILogger<RoomService>> _loggerMock;
        private readonly RoomService _sut; // System Under Test

        public RoomServiceTests()
        {
            _roomRepoMock = new Mock<IRoomRepository>();
            _loggerMock = new Mock<ILogger<RoomService>>();
            _sut = new RoomService(_roomRepoMock.Object, _loggerMock.Object);
        }

        private Room CreateValidRoom() => new Room
        {
            Id = 1,
            Name = "Raum 1",
            Capacity = 10
        };

        // --------------------------------------------------------------------
        // GetRoomByIdAsync
        // --------------------------------------------------------------------

        [Fact]
        public async Task GetRoomByIdAsync_ThrowsArgumentException_WhenIdLessOrEqualZero()
        {
            // Act + Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _sut.GetRoomByIdAsync(0));
            Assert.Equal("Room Id must be greater than zero", ex.Message);
        }

        [Fact]
        public async Task GetRoomByIdAsync_CallsRepositoryWithCorrectId()
        {
            // Arrange
            var room = CreateValidRoom();
            _roomRepoMock.Setup(r => r.GetRoomByIdAsync(room.Id))
                .ReturnsAsync(room);

            // Act
            var result = await _sut.GetRoomByIdAsync(room.Id);

            // Assert
            Assert.Equal(room, result);
            _roomRepoMock.Verify(r => r.GetRoomByIdAsync(room.Id), Times.Once);
        }

        // --------------------------------------------------------------------
        // AddRoomAsync
        // --------------------------------------------------------------------

        [Fact]
        public async Task AddRoomAsync_ReturnsError_WhenRoomIsNull()
        {
            // Act
            var (added, error) = await _sut.AddRoomAsync(null!);

            // Assert
            Assert.False(added);
            Assert.Equal("Room ist NULL", error);
            _roomRepoMock.Verify(r => r.AddRoomAsync(It.IsAny<Room>()), Times.Never);
        }

        [Fact]
        public async Task AddRoomAsync_ReturnsError_WhenRoomNameAlreadyExists()
        {
            // Arrange
            var room = CreateValidRoom();

            _roomRepoMock
                .Setup(r => r.RoomExistsAsync(room.Name, room.Id))
                .ReturnsAsync(true);

            // Act
            var (added, error) = await _sut.AddRoomAsync(room);

            // Assert
            Assert.False(added);
            Assert.Equal("Raumname ist bereits vergeben", error);
            _roomRepoMock.Verify(r => r.AddRoomAsync(It.IsAny<Room>()), Times.Never);
        }

        [Fact]
        public async Task AddRoomAsync_ReturnsError_WhenCapacityIsLessOrEqualZero()
        {
            // Arrange
            var room = CreateValidRoom();
            room.Capacity = 0;

            _roomRepoMock
                .Setup(r => r.RoomExistsAsync(room.Name, room.Id))
                .ReturnsAsync(false);

            // Act
            var (added, error) = await _sut.AddRoomAsync(room);

            // Assert
            Assert.False(added);
            Assert.Equal("Capacity darf nicht kleiner gleich null sein", error);
            _roomRepoMock.Verify(r => r.AddRoomAsync(It.IsAny<Room>()), Times.Never);
        }

        [Fact]
        public async Task AddRoomAsync_ReturnsError_WhenRepositorySaveFails()
        {
            // Arrange
            var room = CreateValidRoom();

            _roomRepoMock
                .Setup(r => r.RoomExistsAsync(room.Name, room.Id))
                .ReturnsAsync(false);

            _roomRepoMock
                .Setup(r => r.AddRoomAsync(room))
                .ReturnsAsync(false);

            // Act
            var (added, error) = await _sut.AddRoomAsync(room);

            // Assert
            Assert.False(added);
            Assert.Equal("Fehler beim Speichern", error);
        }

        [Fact]
        public async Task AddRoomAsync_ReturnsTrue_WhenValid()
        {
            // Arrange
            var room = CreateValidRoom();

            _roomRepoMock
                .Setup(r => r.RoomExistsAsync(room.Name, room.Id))
                .ReturnsAsync(false);

            _roomRepoMock
                .Setup(r => r.AddRoomAsync(room))
                .ReturnsAsync(true);

            // Act
            var (added, error) = await _sut.AddRoomAsync(room);

            // Assert
            Assert.True(added);
            Assert.Null(error);
            _roomRepoMock.Verify(r => r.AddRoomAsync(room), Times.Once);
        }

        // --------------------------------------------------------------------
        // UpdateRoomAsync
        // --------------------------------------------------------------------

        [Fact]
        public async Task UpdateRoomAsync_ReturnsError_WhenRoomIsNull()
        {
            var (updated, error) = await _sut.UpdateRoomAsync(null!);

            Assert.False(updated);
            Assert.Equal("Room darf nicht NULL sein", error);
            _roomRepoMock.Verify(r => r.UpdateRoomAsync(It.IsAny<Room>()), Times.Never);
        }

        [Fact]
        public async Task UpdateRoomAsync_ReturnsError_WhenIdLessOrEqualZero()
        {
            var room = CreateValidRoom();
            room.Id = 0;

            var (updated, error) = await _sut.UpdateRoomAsync(room);

            Assert.False(updated);
            Assert.Equal("Ungültige Room ID", error);
            _roomRepoMock.Verify(r => r.UpdateRoomAsync(It.IsAny<Room>()), Times.Never);
        }

        [Fact]
        public async Task UpdateRoomAsync_ReturnsError_WhenCapacityLessOrEqualZero()
        {
            var room = CreateValidRoom();
            room.Capacity = 0;

            var (updated, error) = await _sut.UpdateRoomAsync(room);

            Assert.False(updated);
            Assert.Equal("Capacity darf nicht kleiner gleich null sein", error);
            _roomRepoMock.Verify(r => r.UpdateRoomAsync(It.IsAny<Room>()), Times.Never);
        }

        [Fact]
        public async Task UpdateRoomAsync_ReturnsError_WhenRoomNameAlreadyExists()
        {
            var room = CreateValidRoom();

            _roomRepoMock
                .Setup(r => r.RoomExistsAsync(room.Name, room.Id))
                .ReturnsAsync(true);

            var (updated, error) = await _sut.UpdateRoomAsync(room);

            Assert.False(updated);
            Assert.Equal("Raumname ist bereits vergeben", error);
            _roomRepoMock.Verify(r => r.UpdateRoomAsync(It.IsAny<Room>()), Times.Never);
        }

        [Fact]
        public async Task UpdateRoomAsync_ReturnsError_WhenRepositoryUpdateFails()
        {
            var room = CreateValidRoom();

            _roomRepoMock
                .Setup(r => r.RoomExistsAsync(room.Name, room.Id))
                .ReturnsAsync(false);

            _roomRepoMock
                .Setup(r => r.UpdateRoomAsync(room))
                .ReturnsAsync(false);

            var (updated, error) = await _sut.UpdateRoomAsync(room);

            Assert.False(updated);
            Assert.Equal("Fehler beim Aktualisieren", error);
        }

        [Fact]
        public async Task UpdateRoomAsync_ReturnsTrue_WhenValid()
        {
            var room = CreateValidRoom();

            _roomRepoMock
                .Setup(r => r.RoomExistsAsync(room.Name, room.Id))
                .ReturnsAsync(false);

            _roomRepoMock
                .Setup(r => r.UpdateRoomAsync(room))
                .ReturnsAsync(true);

            var (updated, error) = await _sut.UpdateRoomAsync(room);

            Assert.True(updated);
            Assert.Null(error);
            _roomRepoMock.Verify(r => r.UpdateRoomAsync(room), Times.Once);
        }

        // --------------------------------------------------------------------
        // DeleteRoomAsync
        // --------------------------------------------------------------------

        [Fact]
        public async Task DeleteRoomAsync_ReturnsError_WhenIdLessOrEqualZero()
        {
            var room = CreateValidRoom();
            room.Id = 0;

            var (deleted, error) = await _sut.DeleteRoomAsync(room);

            Assert.False(deleted);
            Assert.Equal("Ungültige Room ID", error);
            _roomRepoMock.Verify(r => r.DeleteRoomByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task DeleteRoomAsync_ReturnsError_WhenRepositoryDeleteFails()
        {
            var room = CreateValidRoom();

            _roomRepoMock
                .Setup(r => r.DeleteRoomByIdAsync(room.Id))
                .ReturnsAsync(false);

            var (deleted, error) = await _sut.DeleteRoomAsync(room);

            Assert.False(deleted);
            Assert.Equal("Fehler beim Löschen des Raums", error);
        }

        [Fact]
        public async Task DeleteRoomAsync_ReturnsTrue_WhenValid()
        {
            var room = CreateValidRoom();

            _roomRepoMock
                .Setup(r => r.DeleteRoomByIdAsync(room.Id))
                .ReturnsAsync(true);

            var (deleted, error) = await _sut.DeleteRoomAsync(room);

            Assert.True(deleted);
            Assert.Null(error);
            _roomRepoMock.Verify(r => r.DeleteRoomByIdAsync(room.Id), Times.Once);
        }
    }
}
