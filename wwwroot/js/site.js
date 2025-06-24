document.addEventListener("DOMContentLoaded", () => {
    // ✅ Hilfsfunktion für korrektes lokales Datum im Format YYYY-MM-DD
    function toDateStringLocal(date) {
        return date.getFullYear() + "-" +
            (date.getMonth() + 1).toString().padStart(2, "0") + "-" +
            date.getDate().toString().padStart(2, "0");
    }

    const dateDisplay = document.getElementById("dateDisplay");
    const modal = document.getElementById("modal");
    const modalRoom = document.getElementById("modalRoom");
    const modalHour = document.getElementById("modalHour");
    const calendarInput = document.getElementById("calendarInput");
    let currentDate = new Date();

    document.getElementById("prevDay").addEventListener("click", () => changeDay(-1));
    document.getElementById("nextDay").addEventListener("click", () => changeDay(1));
    document.getElementById("calendarIcon").addEventListener("click", () => calendarInput._flatpickr.open());

    function changeDay(offset) {
        const newDate = new Date(currentDate);
        newDate.setDate(newDate.getDate() + offset);
        currentDate = newDate;
        updateDateInput();
        renderDate();
        renderGrid();
    }

    function updateDateInput() {
        if (calendarInput._flatpickr) {
            calendarInput._flatpickr.setDate(currentDate, false);
        }
    }

    function renderDate() {
        dateDisplay.textContent = currentDate.toLocaleDateString("de-DE");
    }

    async function renderGrid() {
        const grid = document.getElementById("bookingGrid");
        grid.innerHTML = "";

        const halfHourSlots = Array.from({ length: 26 }, (_, i) => {
            const hour = 7 + Math.floor(i / 2);
            const minute = i % 2 === 0 ? "00" : "30";
            return `${hour.toString().padStart(2, "0")}:${minute}`;
        });
        const visibleHours = halfHourSlots.filter(h => h.endsWith(":00"));
        const rooms = [1, 2, 3];
        const headerLabels = ["Zeit", ...rooms.map(i => `Raum ${i}`)];
        headerLabels.forEach(label => grid.appendChild(makeCell(label, "time-cell")));

        const dateStr = toDateStringLocal(currentDate); // ✅ ersetzt
        let bookings = [];

        try {
            const res = await fetch(`/api/booking/date/${dateStr}`);
            if (res.ok) bookings = await res.json();
            else console.warn("Konnte Buchungen nicht laden:", await res.text());
        } catch (err) {
            console.error("Fehler beim Laden der Buchungen:", err);
        }

        visibleHours.forEach(hour => {
            const rowCells = [];
            rowCells.push(makeCell(hour, "time-cell"));

            rooms.forEach(roomId => {
                const cell = document.createElement("div");
                cell.className = "room-cell";
                cell.dataset.hour = hour;
                cell.dataset.room = roomId;

                const [h, m] = hour.split(":");
                const slotTime = new Date(`${dateStr}T${h.padStart(2, "0")}:00:00`);
                const nextSlot = new Date(slotTime.getTime() + 60 * 60 * 1000); // +1h

                const now = new Date();
                const todayStr = toDateStringLocal(now); // ✅ ersetzt

                if (dateStr < todayStr || (dateStr === todayStr && slotTime < now)) {
                    cell.classList.add("past", "disabled");
                } else {
                    cell.addEventListener("click", () => openModal(hour, roomId, cell));
                }

                const cellBookings = bookings.filter(b => {
                    const start = new Date(b.startTime);
                    const end = new Date(b.endTime);
                    return (
                        b.roomId === roomId &&
                        start < nextSlot &&
                        end > slotTime
                    );
                });

                cellBookings.forEach(booking => {
                    const block = document.createElement("div");
                    const start = new Date(booking.startTime);
                    const end = new Date(booking.endTime);

                    block.className = "booking-block";
                    if (!booking.canDelete) block.classList.add("others");

                    block.dataset.bookingId = booking.id;
                    block.dataset.canDelete = booking.canDelete;

                    const minutes = start.getMinutes();
                    block.style.top = minutes >= 30 ? "50%" : "0";
                    block.textContent = `${start.toTimeString().slice(0,5)}–${end.toTimeString().slice(0,5)} belegt`;

                    cell.appendChild(block);
                });

                rowCells.push(cell);
            });

            const row = document.createElement("div");
            row.style.display = "contents";
            rowCells.forEach(c => row.appendChild(c));
            grid.appendChild(row);
        });
    }

    function makeCell(text, className) {
        const div = document.createElement("div");
        div.className = className;
        div.textContent = text;
        return div;
    }

    function openModal(hour, room, cell) {
        modalHour.textContent = hour;
        modalRoom.textContent = room;
        modal.dataset.targetCell = `${hour}|${room}`;
        modal.classList.remove("hidden");

        const startField = document.getElementById("startTime");
        const endField = document.getElementById("endTime");
        const bookBtn = document.getElementById("bookBtn");
        const cancelBtn = document.getElementById("cancelBtn");

        bookBtn.classList.add("hidden");
        cancelBtn.classList.add("hidden");

        const bookingBlock = cell.querySelector(".booking-block");
        if (bookingBlock) {
            const text = bookingBlock.textContent.trim();
            const [start, end] = text.match(/\d{2}:\d{2}/g);
            startField.value = start;
            endField.value = end;

            if (bookingBlock.dataset.canDelete === "true") {
                cancelBtn.classList.remove("hidden");
                cancelBtn.dataset.bookingId = bookingBlock.dataset.bookingId;
            }
        } else {
            startField.value = hour;
            const [h, m] = hour.split(":");
            const endMin = m === "00" ? "30" : "00";
            const endHour = m === "00" ? h : (parseInt(h) + 1).toString().padStart(2, "0");
            endField.value = `${endHour}:${endMin}`;
            bookBtn.classList.remove("hidden");
        }
    }

    window.closeModal = () => modal.classList.add("hidden");

    window.cancelBooking = async () => {
        const bookingId = document.getElementById("cancelBtn").dataset.bookingId;
        if (!bookingId) return;
        if (!confirm("Buchung wirklich stornieren?")) return;
        const res = await fetch(`/api/booking/${bookingId}`, { method: "DELETE" });
        if (res.ok) {
            closeModal();
            renderGrid();
        } else alert("Stornierung fehlgeschlagen.");
    };

    window.saveBooking = async () => {
        const start = document.getElementById("startTime").value;
        const end = document.getElementById("endTime").value;
        const [hourStr, roomStr] = modal.dataset.targetCell.split("|");
        const room = parseInt(roomStr);

        const dateStr = toDateStringLocal(currentDate); // ✅ ersetzt

        const booking = {
            roomId: room,
            startTime: `${dateStr}T${start}:00`,
            endTime: `${dateStr}T${end}:00`
        };

        try {
            const response = await fetch("/api/booking", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(booking)
            });
            if (response.ok) {
                closeModal();
                renderGrid();
            } else alert("Fehler beim Speichern: " + await response.text());
        } catch (err) {
            alert("Verbindungsfehler zum Server: " + err.message);
        }
    };

    flatpickr(calendarInput, {
        dateFormat: "d.m.Y",
        defaultDate: currentDate,
        onChange: (selectedDates) => {
            if (selectedDates.length > 0) {
                currentDate = new Date(selectedDates[0]);
                updateDateInput();
                renderDate();
                renderGrid();
            }
        },
        appendTo: document.querySelector(".nav-center"),
        position: "below right",
        allowInput: false,
        clickOpens: false
    });

    renderDate();
    renderGrid();
});
