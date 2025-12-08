// src/App.jsx
import FullCalendar from "@fullcalendar/react";
import resourceTimeGridPlugin from "@fullcalendar/resource-timegrid";
import timeGridPlugin from "@fullcalendar/timegrid";
import { useMemo, useRef, useState } from "react";

export default function App() {
    return <RootLayout />;
}

function RootLayout() {
    // aktuelles Datum im State (Start: 08.12.2025)
    const [currentDate, setCurrentDate] = useState(new Date(2025, 11, 8));
    const calendarRef = useRef(null);

    // Räume (Resources) – wie gehabt
    const resources = useMemo(
        () => [
            { id: "raum1", title: "Raum 1" },
            { id: "raum2", title: "Raum 2" },
            { id: "raum3", title: "Raum 3" },
        ],
        []
    );

    // Hilfsfunktion: erstellt Date-Objekt am aktuellen Tag + Uhrzeit
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

    // Buchungen, aber dynamisch auf den aktuellen Tag gelegt
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

    // Datum für Überschrift formatieren
    const formattedDate = (() => {
        const formatter = new Intl.DateTimeFormat("de-DE", {
            weekday: "long",
            day: "2-digit",
            month: "2-digit",
            year: "numeric",
        });
        return formatter.format(currentDate).replace(",", ""); // "Montag 08.12.2025"
    })();

    // Tag wechseln (+1 oder -1) und FullCalendar mitziehen
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

    return (
        <div className="min-h-screen flex flex-col bg-slate-100">
            {/* Navbar oben (bleibt wie bei dir) */}
            <header className="h-16 bg-blue-500 text-white flex items-center shadow">
                <div className="max-w-6xl mx-auto w-full px-4 flex items-center justify-between">
                    <div className="flex items-center gap-2">
            <span className="font-semibold text-lg tracking-tight">
              RoomBooking
            </span>
                    </div>

                    <nav className="flex items-center gap-4 text-sm">
                        <button className="hover:underline">Tagesansicht</button>
                        <button className="hover:underline">Wochenansicht</button>
                        <button className="rounded-full border border-white/70 px-3 py-1 text-xs font-medium hover:bg-white/10">
                            Jetzt buchen
                        </button>
                    </nav>
                </div>
            </header>

            {/* Kalender-Bereich */}
            <main className="flex-1 max-w-6xl mx-auto w-full px-4 py-4">
                {/* zentrierter Header über dem Kalender */}
                <div className="mb-3 flex items-center justify-center gap-4">
                    <button
                        className="px-3 py-1 rounded border border-slate-300 bg-white text-sm hover:bg-slate-50"
                        onClick={() => changeDay(-1)}
                    >
                        &lt;
                    </button>

                    <h2 className="text-lg font-semibold text-slate-800">
                        {formattedDate}
                    </h2>

                    <button
                        className="px-3 py-1 rounded border border-slate-300 bg-white text-sm hover:bg-slate-50"
                        onClick={() => changeDay(1)}
                    >
                        &gt;
                    </button>
                </div>

                {/* Wrapper mit eigener Klasse für CSS */}
                <div className="bg-white rounded-lg shadow overflow-hidden rb-calendar">
                    <FullCalendar
                        ref={calendarRef}
                        plugins={[resourceTimeGridPlugin, timeGridPlugin]}
                        initialView="resourceTimeGridDay"
                        initialDate={currentDate}
                        headerToolbar={false}
                        slotMinTime="07:00:00"
                        slotMaxTime="20:00:00"
                        allDaySlot={false}
                        resources={resources}
                        events={events}
                        height="auto"
                        slotDuration="01:00:00"
                        eventMinHeight={57}
                        eventShortHeight={57}
                        slotLabelFormat={{
                            hour: "2-digit",
                            minute: "2-digit",
                            hour12: false,
                        }}
                        slotLabelInterval={{ hours: 1 }}
                        resourceAreaWidth="80px"
                        resourceAreaHeaderContent="Zeit"
                        nowIndicator={true}
                    />
                </div>
            </main>
        </div>
    );
}
