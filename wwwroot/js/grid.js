import { toDateStringLocal, isPastSlot } from './utils.js';
import { fetchBookingsByDate } from './api.js';

export async function renderGrid(currentDate, openModal) {
    const grid = document.getElementById("bookingGrid");
    grid.innerHTML = "";

    const rooms = [1, 2, 3];
    const halfHourSlots = Array.from({ length: 26 }, (_, i) => {
        const hour = 7 + Math.floor(i / 2);
        const minute = i % 2 === 0 ? "00" : "30";
        return `${hour.toString().padStart(2, "0")}:${minute}`;
    });
    const visibleHours = halfHourSlots.filter(h => h.endsWith(":00"));
    const headerLabels = ["Zeit", ...rooms.map(i => `Raum ${i}`)];
    headerLabels.forEach(label => grid.appendChild(makeCell(label, "time-cell")));

    const dateStr = toDateStringLocal(currentDate);
    let bookings = [];

    try {
        bookings = await fetchBookingsByDate(dateStr);
    } catch (err) {
        console.error("Buchungsladen fehlgeschlagen:", err.message);
    }

    visibleHours.forEach(hour => {
        const row = document.createElement("div");
        row.style.display = "contents";
        row.appendChild(makeCell(hour, "time-cell"));

        rooms.forEach(roomId => {
            const cell = document.createElement("div");
            cell.className = "room-cell";
            cell.dataset.hour = hour;
            cell.dataset.room = roomId;

            if (isPastSlot(dateStr, hour)) {
                cell.classList.add("past", "disabled");
            } else {
                cell.addEventListener("click", () => openModal(hour, roomId, cell));
            }

            const slotStart = new Date(`${dateStr}T${hour}:00`);
            const slotEnd = new Date(slotStart.getTime() + 60 * 60 * 1000);

            bookings
                .filter(b => b.roomId === roomId &&
                    new Date(b.startTime) < slotEnd &&
                    new Date(b.endTime) > slotStart)
                .forEach(booking => cell.appendChild(createBookingBlock(booking)));

            row.appendChild(cell);
        });

        grid.appendChild(row);
    });
}

function makeCell(text, className) {
    const div = document.createElement("div");
    div.className = className;
    div.textContent = text;
    return div;
}

function createBookingBlock(b) {
    const div = document.createElement("div");
    const start = new Date(b.startTime);
    const end = new Date(b.endTime);

    div.className = "booking-block";
    if (!b.canDelete) div.classList.add("others");

    div.dataset.bookingId = b.id;
    div.dataset.canDelete = b.canDelete;

    div.style.top = start.getMinutes() >= 30 ? "50%" : "0";
    div.textContent = `${start.toTimeString().slice(0,5)}–${end.toTimeString().slice(0,5)} belegt`;

    return div;
}
