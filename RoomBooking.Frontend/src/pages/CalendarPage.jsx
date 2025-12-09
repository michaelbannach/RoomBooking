import { useMemo, useRef, useState } from "react";
import Navbar from "../components/Navbar";
import DateNavigator from "../components/DateNavigator";
import RoomCalendar from "../components/RoomCalendar";
import BookingModal from "../components/BookingModal";

export default function CalendarPage() {
    const [currentDate, setCurrentDate] = useState(new Date(2025, 11, 8));
    const calendarRef = useRef(null);
    
    const [selectedEvent, setSelectedEvent] = useState(null);
    
    const resources = useMemo(
        () => [
            { id: "raum1", title: "Raum 1" },
            { id: "raum2", title: "Raum 2" },
            { id: "raum3", title: "Raum 3" },
        ],
        []
    );

    const resourcesById = useMemo(() => {
        const map = {};
        for (const r of resources) {
            map[r.id] = r;
        }
        return map;
    }, [resources]);
    
    const createDateTime = (hour, minute = 0) =>
        new Date(
            currentDate.getFullYear(),
            currentDate.getMonth(),
            currentDate.getDate(),
            hour,
            minute,
            0,
            0
        );
    
    const events = useMemo(
        () => [
            {
                id: "1",
                resourceId: "raum1",
                title: "07:00–08:00",
                start: createDateTime(7, 0),
                end: createDateTime(8, 0),
            },
            {
                id: "2",
                resourceId: "raum3",
                title: "10:00–11:00",
                start: createDateTime(10, 0),
                end: createDateTime(11, 0),
            },
            {
                id: "3",
                resourceId: "raum1",
                title: "15:00–18:00",
                start: createDateTime(15, 0),
                end: createDateTime(18, 0),
            },
            {
                id: "4",
                resourceId: "raum2",
                title: "17:30–19:00",
                start: createDateTime(17, 30),
                end: createDateTime(19, 0),
            },
        ],
        [currentDate]
    );
    
    const formattedDate = (() => {
        const formatter = new Intl.DateTimeFormat("de-DE", {
            weekday: "long",
            day: "2-digit",
            month: "2-digit",
            year: "numeric",
        });
        return formatter.format(currentDate).replace(",", "");
    })();
    
    const changeDay = (delta) => {
        setCurrentDate((prev) => {
            const next = new Date(prev);
            next.setDate(prev.getDate() + delta);

            const api = calendarRef.current?.getApi?.();
            if (api) {
                api.gotoDate(next);
            }

            return next;
        });
    };

    // Event-Klick → Modal öffnen
    const handleEventClick = (info) => {
        const ev = info.event;
        const resourceId =
            ev.getResources && ev.getResources().length > 0
                ? ev.getResources()[0].id
                : ev.extendedProps.resourceId;

        setSelectedEvent({
            id: ev.id,
            title: ev.title,
            start: ev.start,
            end: ev.end,
            resourceId,
        });
    };
    const handleSlotSelect = (info) => {
        // info.start, info.end sind Date-Objekte
        // info.resource.id ist der Raum
        const resourceId = info.resource ? info.resource.id : null;

        setSelectedEvent({
            id: null,
            title: "Neue Buchung",
            start: info.start,
            end: info.end,
            resourceId,
        });
    };


    const handleCloseModal = () => setSelectedEvent(null);

    const handleConfirmBooking = () => {
        // TODO: später API-Call zum Backend
        setSelectedEvent(null);
    };

    const selectedRoomName = selectedEvent
        ? resourcesById[selectedEvent.resourceId]?.title ??
        selectedEvent.resourceId
        : "";

    return (
        <div className="min-h-screen flex flex-col bg-slate-100">
            <Navbar />

            <main className="flex-1 max-w-6xl mx-auto w-full px-4 py-4">
                <DateNavigator
                    formattedDate={formattedDate}
                    onPrevDay={() => changeDay(-1)}
                    onNextDay={() => changeDay(1)}
                />

                <RoomCalendar
                    ref={calendarRef}
                    currentDate={currentDate}
                    resources={resources}
                    events={events}
                    onEventClick={handleEventClick}
                    onSlotSelect={handleSlotSelect}
                />

                <BookingModal
                    event={selectedEvent}
                    roomName={selectedRoomName}
                    onClose={handleCloseModal}
                    onConfirm={handleConfirmBooking}
                />
            </main>
        </div>
    );
}
