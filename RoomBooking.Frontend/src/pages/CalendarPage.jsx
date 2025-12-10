// src/pages/CalendarPage.jsx
import { useMemo, useState } from "react";
import DateNavigator from "../components/DateNavigator";
import RoomCalendar from "../components/RoomCalendar";
import BookingModal from "../components/BookingModal";

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

    // statische Räume wie früher
    const resources = useMemo(
        () => [
            { id: "raum1", title: "Raum 1" },
            { id: "raum2", title: "Raum 2" },
            { id: "raum3", title: "Raum 3" },
        ],
        []
    );

    // in der Wochenansicht nur aktiven Raum zeigen
    const filteredResources =
        isWeekView && activeRoomId
            ? resources.filter((r) => r.id === activeRoomId)
            : resources;

    // Slot angeklickt -> neues Event, Modal öffnen
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

    // Bestehendes Event angeklickt -> Modal mit vorhandenen Daten öffnen
    const handleEventClick = (clickInfo) => {
        const evt = clickInfo.event;
        setSelectedEvent({
            id: evt.id,
            title: evt.title,
            start: evt.start,
            end: evt.end,
            resourceId:
                evt.getResources?.()[0]?.id ?? evt.extendedProps.resourceId,
            resourceTitle:
                evt.getResources?.()[0]?.title ?? evt.extendedProps.resourceTitle,
        });
    };

    const handleCloseModal = () => setSelectedEvent(null);

    // Bestätigen im Modal -> Event lokal anlegen/aktualisieren
    const handleConfirmBooking = ({ start, end }) => {
        if (!selectedEvent) return;

        const startDate = start instanceof Date ? start : new Date(start);
        const endDate = end instanceof Date ? end : new Date(end);

        const formatTime = (d) =>
            d.toLocaleTimeString("de-DE", {
                hour: "2-digit",
                minute: "2-digit",
                hour12: false,
            });

        const newTitle = `${formatTime(startDate)}–${formatTime(endDate)}`;

        setEvents((prev) => {
            if (!selectedEvent.id) {
                const newId = (prev.length + 1).toString();
                return [
                    ...prev,
                    {
                        id: newId,
                        title: newTitle,
                        start: startDate,
                        end: endDate,
                        resourceId: selectedEvent.resourceId,
                    },
                ];
            } else {
                return prev.map((e) =>
                    e.id === selectedEvent.id
                        ? { ...e, title: newTitle, start: startDate, end: endDate }
                        : e
                );
            }
        });

        setSelectedEvent(null);
    };

    return (
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
                        // auf Montag der gewählten Woche „einschnappen“
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

            <BookingModal
                event={selectedEvent}
                roomName={selectedEvent?.resourceTitle}
                onClose={handleCloseModal}
                onConfirm={handleConfirmBooking}
            />
        </div>
    );
}
