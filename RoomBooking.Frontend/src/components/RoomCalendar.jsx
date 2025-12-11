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
    const isPast = (date) => {
        const now = new Date();
        return date.getTime() < now.getTime();
    };
    
    const getWeekRange = (date) => {
        const d = new Date(date);
        const day = (d.getDay() + 6) % 7;   // Montag = 0
        const monday = new Date(d);
        monday.setDate(d.getDate() - day);
        const friday = new Date(monday);
        friday.setDate(monday.getDate() + 4);
        return { monday, friday };
    };

    const getWeekNumber = (date) => {
        const d = new Date(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate()));
        const dayNum = d.getUTCDay() || 7;
        d.setUTCDate(d.getUTCDate() + 4 - dayNum);
        const yearStart = new Date(Date.UTC(d.getUTCFullYear(), 0, 1));
        const weekNo = Math.ceil(((d - yearStart) / 86400000 + 1) / 7);
        return weekNo;
    };

    return (
        <div className="rb-calendar">
            <FullCalendar
                schedulerLicenseKey="CC-Attribution-NonCommercial-NoDerivatives"
                ref={calendarRef}
                plugins={[resourceTimeGridPlugin, timeGridPlugin, interactionPlugin]}
                initialView="resourceTimeGridDay"
                initialDate={currentDate}
                headerToolbar={false}
                views={{
                    resourceTimeGridDay: {
                        type: "resourceTimeGrid",
                        duration: { days: 1 },
                    },
                    resourceTimeGridWeek: {
                        type: "resourceTimeGrid",
                        duration: { days: 5 },      
                    },
                }}
                firstDay={1}                     
                locale="de"
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
                    if (isPast(info.date)) return;
                    onSlotSelect({
                        start: info.date,
                        end: new Date(info.date.getTime() + 30 * 60 * 1000),
                        resource: info.resource,
                    });
                }}
                select={(info) => {
                    if (!onSlotSelect) return;
                    if (isPast(info.start)) return;
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
