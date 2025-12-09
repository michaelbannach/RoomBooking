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
    return (
        <div className="bg-white rounded-lg shadow overflow-hidden rb-calendar">
            <FullCalendar
                ref={calendarRef}
                plugins={[resourceTimeGridPlugin, timeGridPlugin, interactionPlugin]}
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
                /** EXISTING EVENT-CLICK */
                eventClick={onEventClick}
                /** NEU: Slots klickbar machen */
                selectable={true}
                selectLongPressDelay={0}
                selectMirror={true}
                select={(info) => {
                    // nur weiterreichen, wenn jemand das Prop gesetzt hat
                    if (onSlotSelect) {
                        onSlotSelect(info);
                    }
                }}
            />
        </div>
    );
});

export default RoomCalendar;
