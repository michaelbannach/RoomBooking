import { IconButton, Typography, Box } from "@mui/material";
import ChevronLeftIcon from "@mui/icons-material/ChevronLeft";
import ChevronRightIcon from "@mui/icons-material/ChevronRight";
import CalendarTodayIcon from "@mui/icons-material/CalendarToday";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import { useState } from "react";

export default function DateNavigator({
                                          date,
                                          onPrevDay,
                                          onNextDay,
                                          isWeekView,
                                          weekFrom,
                                          weekTo,
                                          weekNumber,
                                          onPickDate,
                                      }) {
    const [open, setOpen] = useState(false);

    const formattedDate = new Intl.DateTimeFormat("de-DE", {
        weekday: "long",
        day: "2-digit",
        month: "2-digit",
        year: "numeric",
    })
        .format(date)
        .replace(",", "");

    const formattedRange =
        weekFrom && weekTo
            ? `${weekFrom.toLocaleDateString("de-DE")} – ${weekTo.toLocaleDateString(
                "de-DE"
            )}`
            : "";

    return (
        <Box
            className="date-nav"
            sx={{ display: "flex", flexDirection: "column", alignItems: "center", gap: 1 }}
        >
            <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
                <IconButton color="primary" size="small" onClick={onPrevDay}>
                    <ChevronLeftIcon />
                </IconButton>

                <Typography variant="h6" component="h2" sx={{ fontWeight: 600 }}>
                    {isWeekView ? formattedRange : formattedDate}
                </Typography>

                <IconButton color="primary" size="small" onClick={onNextDay}>
                    <ChevronRightIcon />
                </IconButton>

                {/* Kalender-Button */}
                <IconButton color="primary" size="small" onClick={() => setOpen(true)}>
                    <CalendarTodayIcon />
                </IconButton>

                {/* Versteckter DatePicker */}
                <DatePicker
                    open={open}
                    onClose={() => setOpen(false)}
                    value={date}
                    onChange={(newValue) => {
                        if (!newValue) return;
                        const d = newValue.toDate ? newValue.toDate() : new Date(newValue);
                        onPickDate?.(d);
                    }}
                    slotProps={{
                        textField: { style: { display: "none" } }, // Textfeld ausblenden
                    }}
                />
            </Box>

            {isWeekView && (
                <Typography variant="body2" sx={{ marginTop: -0.5 }}>
                    KW {weekNumber}
                </Typography>
            )}
        </Box>
    );
}

