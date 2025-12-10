// src/components/DateNavigator.jsx
import { IconButton, Typography, Box } from "@mui/material";
import ChevronLeftIcon from "@mui/icons-material/ChevronLeft";
import ChevronRightIcon from "@mui/icons-material/ChevronRight";

export default function DateNavigator({ date, onPrevDay, onNextDay }) {
    const formattedDate = new Intl.DateTimeFormat("de-DE", {
        weekday: "long",
        day: "2-digit",
        month: "2-digit",
        year: "numeric",
    })
        .format(date)
        .replace(",", "");

    return (
        <Box
            className="date-nav"
            sx={{ display: "flex", alignItems: "center", justifyContent: "center", gap: 2 }}
        >
            <IconButton
                color="primary"
                size="small"
                onClick={onPrevDay}
            >
                <ChevronLeftIcon />
            </IconButton>

            <Typography
                variant="h6"
                component="h2"
                sx={{ fontWeight: 600 }}
                className="date-nav-title"
            >
                {formattedDate}
            </Typography>

            <IconButton
                color="primary"
                size="small"
                onClick={onNextDay}
            >
                <ChevronRightIcon />
            </IconButton>
        </Box>
    );
}
