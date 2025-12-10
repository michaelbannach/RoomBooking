// src/components/RoomCalendar.jsx
import FullCalendar from "@fullcalendar/react";
import resourceTimeGridPlugin from "@fullcalendar/resource-timegrid";
import timeGridPlugin from "@fullcalendar/timegrid";
import interactionPlugin from "@fullcalendar/interaction";
import { forwardRef } from "react";

const RoomCalendar = forwardRef(function RoomCalendar(
    { currentDate, resources, events, onEventClick, onSlotSelect },
    calendarRef
) {
    // >>> HIER: Hilfsfunktion innerhalb der Komponente definieren
    const isPast = (date) => {
        const now = new Date();
        return date.getTime() < now.getTime();
    };

    return (
        <div className="bg-white rounded-lg shadow overflow-hidden rb-calendar">
            <FullCalendar
                ref={calendarRef}
                plugins={[resourceTimeGridPlugin, timeGridPlugin, interactionPlugin]}
                initialView="resourceTimeGridDay"
                initialDate={currentDate}
                headerToolbar={false}
                slotLabelFormat={{
                    hour: "2-digit",
                    minute: "2-digit",
                    hour12: false,
                }}
                slotMinTime="07:00:00"
                slotMaxTime="20:00:00"
                allDaySlot={false}
                height="auto"
                resources={resources}
                events={events}
                eventClick={onEventClick}
                selectable={true}
                selectMirror={true}
                slotDuration="00:30:00"
                dateClick={(info) => {
                    if (!onSlotSelect) return;
                    onSlotSelect({
                        start: info.date,
                        end: new Date(info.date.getTime() + 30 * 60 * 1000),
                        resource: info.resource,
                    });
                }}
                select={(info) => {
                    if (!onSlotSelect) return;
                    onSlotSelect({
                        start: info.start,
                        end: info.end,
                        resource: info.resource,
                    });
                }}
            />

        </div>
    );
});

export default RoomCalendar;
