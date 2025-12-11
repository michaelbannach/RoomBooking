// src/pages/CalendarPage.jsx
import { useMemo, useState, useEffect } from "react";
import DateNavigator from "../components/DateNavigator";
import RoomCalendar from "../components/RoomCalendar";
import BookingModal from "../components/BookingModal";
import { authFetch } from "../apiClient";

// Wochenbereich (Mo–Fr) aus einem Datum berechnen
const getWeekRange = (date) => {
    const d = new Date(date);
    const day = (d.getDay() + 6) % 7; // Montag = 0
    const monday = new Date(d);
    monday.setDate(d.getDate() - day);
    const friday = new Date(monday);
    friday.setDate(monday.getDate() + 4);
    return { monday, friday };
};

// ISO-Kalenderwoche berechnen
const getWeekNumber = (date) => {
    const d = new Date(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate()));
    const dayNum = d.getUTCDay() || 7;
    d.setUTCDate(d.getUTCDate() + 4 - dayNum);
    const yearStart = new Date(Date.UTC(d.getUTCFullYear(), 0, 1));
    const weekNo = Math.ceil(((d - yearStart) / 86400000 + 1) / 7);
    return weekNo;
};

// Mapping zwischen Frontend-Resource-ID und Room.Id in der DB
const mapResourceIdToRoomId = (resourceId) => {
    switch (resourceId) {
        case "raum1":
            return 6; // DB: Rooms.Id = 6
        case "raum2":
            return 7; // DB: Rooms.Id = 7
        case "raum3":
            return 8; // DB: Rooms.Id = 8
        default:
            return null;
    }
};

const mapRoomIdToResourceId = (roomId) => {
    switch (roomId) {
        case 6:
            return "raum1";
        case 7:
            return "raum2";
        case 8:
            return "raum3";
        default:
            return null;
    }
};

function getUserIdFromToken() {
    const token = localStorage.getItem("jwt");
    if (!token) return null;

    const payload = token.split(".")[1];
    try {
        const json = JSON.parse(atob(payload));
        return json.userId ? parseInt(json.userId, 10) : null;
    } catch (e) {
        return null;
    }
}

export default function CalendarPage({
                                         currentView,
                                         currentDate,
                                         setCurrentDate,
                                         calendarRef,
                                     }) {
    const [selectedEvent, setSelectedEvent] = useState(null);
    const [events, setEvents] = useState([]);
    const [activeRoomId, setActiveRoomId] = useState("raum1");

    const isWeekView = currentView === "resourceTimeGridWeek";
    const { monday, friday } = getWeekRange(currentDate);
    const currentWeekNumber = getWeekNumber(currentDate);

    // statische Räume wie bisher
    const resources = useMemo(
        () => [
            { id: "raum1", title: "Raum 1" },
            { id: "raum2", title: "Raum 2" },
            { id: "raum3", title: "Raum 3" },
        ],
        []
    );

    const filteredResources =
        isWeekView && activeRoomId
            ? resources.filter((r) => r.id === activeRoomId)
            : resources;

    const formatTime = (d) =>
        d.toLocaleTimeString("de-DE", {
            hour: "2-digit",
            minute: "2-digit",
            hour12: false,
        });

    const buildTitle = (startDate, endDate) =>
        `${formatTime(startDate)}–${formatTime(endDate)}`;

    // 1) Beim Laden: alle Bookings vom Backend holen und in Events umwandeln
    useEffect(() => {
        const loadBookings = async () => {
            try {
                const resp = await authFetch("/api/booking", { method: "GET" });
                if (!resp.ok) {
                    console.error("Fehler beim Laden der Buchungen");
                    return;
                }

                const data = await resp.json(); // BookingDto[]

                const mapped = data.map((b) => {
                    const start = new Date(b.startDate);
                    const end = new Date(b.endDate);
                    const resourceId = mapRoomIdToResourceId(b.roomId);

                    return {
                        id: String(b.id),
                        title: buildTitle(start, end),
                        start,
                        end,
                        resourceId,
                        resourceTitle: resources.find((r) => r.id === resourceId)?.title,
                        userId: b.userId, // Besitzer der Buchung merken
                    };
                });

                setEvents(mapped);
            } catch (err) {
                console.error("Fehler beim Laden der Buchungen", err);
            }
        };

        loadBookings();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, []); // nur einmal beim Mount

    const handleSlotSelect = ({ start, end, resource }) => {
        setSelectedEvent({
            id: null,
            title: "",
            start,
            end,
            resourceId: resource?.id,
            resourceTitle: resource?.title,
        });
    };

    // Nur eigene Buchungen dürfen das Modal öffnen
    const handleEventClick = (clickInfo) => {
        const evt = clickInfo.event;
        const eventUserId = evt.extendedProps.userId;
        const currentUserId = getUserIdFromToken();

        // Fremde Buchungen: nichts tun
        if (!currentUserId || eventUserId !== currentUserId) {
            return;
        }

        setSelectedEvent({
            id: evt.id,
            title: evt.title,
            start: evt.start,
            end: evt.end,
            resourceId:
                evt.getResources?.()[0]?.id ?? evt.extendedProps.resourceId,
            resourceTitle:
                evt.getResources?.()[0]?.title ?? evt.extendedProps.resourceTitle,
            userId: eventUserId,
        });
    };

    const handleCloseModal = () => setSelectedEvent(null);

    // 2) Speichern im Modal -> POST/PUT gegen Backend
    const handleConfirmBooking = async ({ start, end }) => {
        if (!selectedEvent) return;

        const startDate = start instanceof Date ? start : new Date(start);
        const endDate = end instanceof Date ? end : new Date(end);

        const roomId = mapResourceIdToRoomId(selectedEvent.resourceId);
        if (!roomId) {
            console.error(
                "Kein gültiger Raum für die Buchung vorhanden (resourceId:",
                selectedEvent.resourceId,
                ")"
            );
            setSelectedEvent(null);
            return;
        }

        try {
            if (!selectedEvent.id) {
                // NEUE Buchung -> POST /api/booking
                const createDto = {
                    startDate,
                    endDate,
                    roomId,
                };

                const resp = await authFetch("/api/booking", {
                    method: "POST",
                    body: JSON.stringify(createDto),
                });

                if (!resp.ok) {
                    const text = await resp.text();
                    console.error("Fehler beim Anlegen der Buchung:", text);
                    alert(
                        text || "Die Buchung konnte nicht angelegt werden (evtl. Kollision?)."
                    );
                    return;
                }

                const created = await resp.json(); // BookingDto
                const resourceId = mapRoomIdToResourceId(created.roomId);
                const startFromServer = new Date(created.startDate);
                const endFromServer = new Date(created.endDate);

                const newEvent = {
                    id: String(created.id),
                    title: buildTitle(startFromServer, endFromServer),
                    start: startFromServer,
                    end: endFromServer,
                    resourceId,
                    resourceTitle: resources.find((r) => r.id === resourceId)?.title,
                    userId: created.userId,
                };

                setEvents((prev) => [...prev, newEvent]);
            } else {
                // EXISTIERENDE Buchung -> PUT /api/booking/{id}
                const updateDto = {
                    id: parseInt(selectedEvent.id, 10),
                    startDate,
                    endDate,
                    roomId,
                };

                const resp = await authFetch(`/api/booking/${selectedEvent.id}`, {
                    method: "PUT",
                    body: JSON.stringify(updateDto),
                });

                if (!resp.ok) {
                    const text = await resp.text();
                    console.error("Fehler beim Aktualisieren der Buchung:", text);
                    alert(
                        text ||
                        "Die Buchung konnte nicht aktualisiert werden (evtl. Kollision?)."
                    );
                    return;
                }

                const updated = await resp.json(); // BookingDto
                const resourceId = mapRoomIdToResourceId(updated.roomId);
                const startFromServer = new Date(updated.startDate);
                const endFromServer = new Date(updated.endDate);

                const updatedEvent = {
                    id: String(updated.id),
                    title: buildTitle(startFromServer, endFromServer),
                    start: startFromServer,
                    end: endFromServer,
                    resourceId,
                    resourceTitle: resources.find((r) => r.id === resourceId)?.title,
                    userId: updated.userId,
                };

                setEvents((prev) =>
                    prev.map((e) => (e.id === updatedEvent.id ? updatedEvent : e))
                );
            }
        } catch (err) {
            console.error("Fehler beim Speichern der Buchung", err);
            alert("Beim Speichern der Buchung ist ein Fehler aufgetreten.");
        } finally {
            setSelectedEvent(null);
        }
    };

    // 3) Löschen einer eigenen Buchung
    const handleDeleteBooking = async (bookingId) => {
        if (!bookingId) return;

        try {
            const resp = await authFetch(`/api/booking/${bookingId}`, {
                method: "DELETE",
            });

            if (!resp.ok) {
                const text = await resp.text();
                console.error("Fehler beim Löschen der Buchung:", text);
                alert(text || "Die Buchung konnte nicht gelöscht werden.");
                return;
            }

            setEvents((prev) => prev.filter((e) => e.id !== String(bookingId)));
        } catch (err) {
            console.error("Fehler beim Löschen der Buchung", err);
            alert("Beim Löschen der Buchung ist ein Fehler aufgetreten.");
        } finally {
            setSelectedEvent(null);
        }
    };

    return (
        <div className="app-root">
            <main className="app-main">
                <div className="calendar-card">
                    <DateNavigator
                        date={currentDate}
                        onPrevDay={() => {
                            const delta = isWeekView ? -7 : -1;
                            const next = new Date(currentDate);
                            next.setDate(currentDate.getDate() + delta);
                            setCurrentDate(next);
                            const api = calendarRef.current?.getApi?.();
                            if (api) api.gotoDate(next);
                        }}
                        onNextDay={() => {
                            const delta = isWeekView ? 7 : 1;
                            const next = new Date(currentDate);
                            next.setDate(currentDate.getDate() + delta);
                            setCurrentDate(next);
                            const api = calendarRef.current?.getApi?.();
                            if (api) api.gotoDate(next);
                        }}
                        isWeekView={isWeekView}
                        weekFrom={monday}
                        weekTo={friday}
                        weekNumber={currentWeekNumber}
                        onPickDate={(picked) => {
                            let next = new Date(picked);

                            if (isWeekView) {
                                const day = (next.getDay() + 6) % 7;
                                next.setDate(next.getDate() - day);
                            }

                            setCurrentDate(next);
                            const api = calendarRef.current?.getApi?.();
                            if (api) api.gotoDate(next); // FullCalendar springt auf diesen Tag/Woche
                        }}
                    />

                    {isWeekView && (
                        <div className="room-tabs">
                            {resources.map((r) => (
                                <button
                                    key={r.id}
                                    className={
                                        r.id === activeRoomId ? "room-tab active" : "room-tab"
                                    }
                                    onClick={() => setActiveRoomId(r.id)}
                                >
                                    {r.title}
                                </button>
                            ))}
                        </div>
                    )}

                    <RoomCalendar
                        ref={calendarRef}
                        currentDate={currentDate}
                        resources={filteredResources}
                        events={events}
                        onEventClick={handleEventClick}
                        onSlotSelect={handleSlotSelect}
                    />
                </div>

                <BookingModal
                    event={selectedEvent}
                    roomName={selectedEvent?.resourceTitle}
                    onClose={handleCloseModal}
                    onConfirm={handleConfirmBooking}
                    onDelete={handleDeleteBooking} // neu
                />
            </main>
        </div>
    );
}
