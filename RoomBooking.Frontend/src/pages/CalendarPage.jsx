// src/pages/CalendarPage.jsx
import { useMemo, useRef, useState } from "react";
import Navbar from "../components/Navbar";
import DateNavigator from "../components/DateNavigator";
import RoomCalendar from "../components/RoomCalendar";
import BookingModal from "../components/BookingModal";

export default function CalendarPage() {
    const [currentDate, setCurrentDate] = useState(new Date());
    const calendarRef = useRef(null);
    const [selectedEvent, setSelectedEvent] = useState(null);
    const [events, setEvents] = useState([]);
    const [currentView, setCurrentView] = useState("resourceTimeGridDay");
    const [activeRoomId, setActiveRoomId] = useState("raum1");

    const resources = useMemo(
        () => [
            { id: "raum1", title: "Raum 1" },
            { id: "raum2", title: "Raum 2" },
            { id: "raum3", title: "Raum 3" },
        ],
        []
    );

    const filteredResources =
        currentView === "resourceTimeGridWeek"
            ? resources.filter((r) => r.id === activeRoomId)
            : resources;
    
    const handleSlotSelect = ({ start, end, resource }) => {
        setSelectedEvent({
            id: null, // null => neue Buchung
            title: "",
            start,
            end,
            resourceId: resource?.id,
            resourceTitle: resource?.title,
        });
    };

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
                // Neue Buchung
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
                // bestehende Buchung aktualisieren
                return prev.map((e) =>
                    e.id === selectedEvent.id
                        ? {
                            ...e,
                            title: newTitle,
                            start: startDate,
                            end: endDate,
                        }
                        : e
                );
            }
        });

        setSelectedEvent(null);
    };

    return (
        <div className="app-root">
            <Navbar
                currentView={currentView}
                onChangeView={(view) => {
                    setCurrentView(view);
                    const api = calendarRef.current?.getApi?.();
                    if (api) api.changeView(view);
                }}
            />

            <main className="app-main">
                <div className="calendar-card">
                    <DateNavigator
                        date={currentDate}
                        onPrevDay={() =>
                            setCurrentDate((prev) => {
                                const next = new Date(prev);
                                next.setDate(prev.getDate() - 1);
                                const api = calendarRef.current?.getApi?.();
                                if (api) api.gotoDate(next);
                                return next;
                            })
                        }
                        onNextDay={() =>
                            setCurrentDate((prev) => {
                                const next = new Date(prev);
                                next.setDate(prev.getDate() + 1);
                                const api = calendarRef.current?.getApi?.();
                                if (api) api.gotoDate(next);
                                return next;
                            })
                        }
                    />
                    {currentView === "resourceTimeGridWeek" && (
                        <div className="room-tabs">
                            {resources.map((r) => (
                                <button
                                    key={r.id}
                                    className={r.id === activeRoomId ? "room-tab active" : "room-tab"}
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
                />

                <pre className="debug-block">
          {JSON.stringify(selectedEvent, null, 2)}
        </pre>
            </main>
        </div>
    );
}
