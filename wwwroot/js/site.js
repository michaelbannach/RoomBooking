import { renderGrid } from './grid.js';
import { toDateStringLocal } from './utils.js';
import { saveBooking, deleteBooking } from './api.js';

document.addEventListener("DOMContentLoaded", () => {
    let currentDate = new Date();

    function updateDateUI() {
        document.getElementById("dateDisplay").textContent = currentDate.toLocaleDateString("de-DE");
        if (calendarInput._flatpickr) {
            calendarInput._flatpickr.setDate(currentDate, false);
        }
    }

    function changeDay(offset) {
        currentDate.setDate(currentDate.getDate() + offset);
        updateDateUI();
        renderGrid(currentDate, openModal);
    }

    // Setup Events
    document.getElementById("prevDay").addEventListener("click", () => changeDay(-1));
    document.getElementById("nextDay").addEventListener("click", () => changeDay(1));
    document.getElementById("calendarIcon").addEventListener("click", () => calendarInput._flatpickr.open());

    flatpickr(calendarInput, {
        dateFormat: "d.m.Y",
        defaultDate: currentDate,
        onChange: ([d]) => {
            currentDate = d;
            updateDateUI();
            renderGrid(currentDate, openModal);
        },
        appendTo: document.querySelector(".nav-center"),
        position: "below right",
        allowInput: false,
        clickOpens: false
    });

    // Modal Handling
    window.openModal = function (hour, roomId, cell) {
        // Implementierung wie gehabt, oder extrahieren
    };

    window.closeModal = function () {
        document.getElementById("modal").classList.add("hidden");
    };

    window.cancelBooking = async function () {
        const id = document.getElementById("cancelBtn").dataset.bookingId;
        if (!id || !confirm("Wirklich stornieren?")) return;
        try {
            await deleteBooking(id);
            closeModal();
            renderGrid(currentDate, openModal);
        } catch (err) {
            alert("Fehler beim Stornieren: " + err.message);
        }
    };

    window.saveBooking = async function () {
        const start = document.getElementById("startTime").value;
        const end = document.getElementById("endTime").value;
        const [hour, room] = document.getElementById("modal").dataset.targetCell.split("|");
        const dateStr = toDateStringLocal(currentDate);
        const booking = {
            roomId: parseInt(room),
            startTime: `${dateStr}T${start}:00`,
            endTime: `${dateStr}T${end}:00`
        };

        try {
            await saveBooking(booking);
            closeModal();
            renderGrid(currentDate, openModal);
        } catch (err) {
            alert("Fehler beim Speichern: " + err.message);
        }
    };

    // Initial render
    updateDateUI();
    renderGrid(currentDate, openModal);
});
