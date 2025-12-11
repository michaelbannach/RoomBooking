import { useEffect, useState } from "react";
import {
    Dialog,
    DialogContent,
    DialogTitle,
    DialogActions,
    Button,
    Box,
    Typography,
    FormControl,
    InputLabel,
    Select,
    MenuItem,
} from "@mui/material";

const MIN_HOUR = 7;
const MAX_HOUR = 20;

const pad2 = (n) => String(n).padStart(2, "0");

export default function BookingModal({
                                         event,
                                         roomName,
                                         onClose,
                                         onConfirm,
                                         onDelete,
                                     }) {
    if (!event) return null;

    const startDate =
        event.start instanceof Date ? event.start : new Date(event.start);

    const [startMinute, setStartMinute] = useState("00");
    const [endHour, setEndHour] = useState(startDate.getHours() + 1);
    const [endMinute, setEndMinute] = useState("00");
    const [error, setError] = useState("");

    useEffect(() => {
        if (!event) return;

        const s =
            event.start instanceof Date ? event.start : new Date(event.start);
        const e = event.end
            ? event.end instanceof Date
                ? event.end
                : new Date(event.end)
            : null;

        const sMinutes = s.getMinutes();
        setStartMinute(sMinutes === 30 ? "30" : "00");

        const defaultEndHour = s.getHours() + 1;
        const eh = e?.getHours() ?? defaultEndHour;
        const em = e?.getMinutes() ?? 0;

        setEndHour(eh);
        setEndMinute(em === 30 ? "30" : "00");
        setError("");
    }, [event]);

    const buildDate = (base, hour, minute) =>
        new Date(
            base.getFullYear(),
            base.getMonth(),
            base.getDate(),
            hour,
            minute,
            0,
            0
        );

    const validate = (start, end) => {
        if (end <= start) {
            return "Endzeit muss nach der Startzeit liegen.";
        }
        if (
            start.getHours() < MIN_HOUR ||
            end.getHours() > MAX_HOUR ||
            end.getHours() < MIN_HOUR
        ) {
            return `Zeitraum muss zwischen ${pad2(MIN_HOUR)}:00 und ${pad2(
                MAX_HOUR
            )}:00 liegen.`;
        }
        return "";
    };

    const handleSave = () => {
        const startMinNum = parseInt(startMinute, 10) || 0;
        const endHourNum = parseInt(endHour, 10) || startDate.getHours() + 1;
        const endMinNum = parseInt(endMinute, 10) || 0;

        const start = buildDate(startDate, startDate.getHours(), startMinNum);
        const end = buildDate(startDate, endHourNum, endMinNum);

        const validationError = validate(start, end);
        if (validationError) {
            setError(validationError);
            return;
        }

        setError("");
        onConfirm && onConfirm({ start, end });
    };

    const handleDelete = () => {
        if (!event?.id) return;
        onDelete && onDelete(event.id);
    };

    const displayDate = new Intl.DateTimeFormat("de-DE", {
        weekday: "long",
        day: "2-digit",
        month: "2-digit",
        year: "numeric",
    }).format(startDate);

    const isEdit = !!event.id;

    return (
        <Dialog open={!!event} onClose={onClose} maxWidth="sm" fullWidth>
            <DialogTitle>
                {isEdit ? "Buchung bearbeiten" : "Neue Buchung"}
            </DialogTitle>
            <DialogContent dividers>
                <Box mb={2}>
                    <Typography variant="subtitle2">
                        {displayDate}
                        {roomName ? ` · Raum: ${roomName}` : ""}
                    </Typography>
                </Box>

                {/* Startzeit */}
                <Box mb={2}>
                    <Typography variant="caption" color="text.secondary">
                        Startzeit
                    </Typography>
                    <Box display="flex" alignItems="center" gap={2} mt={0.5}>
                        <Box
                            px={2}
                            py={1}
                            border={1}
                            borderRadius={1}
                            bgcolor="grey.100"
                            fontSize={14}
                        >
                            {pad2(startDate.getHours())}
                        </Box>
                        <FormControl size="small">
                            <InputLabel id="start-minute-label">Min</InputLabel>
                            <Select
                                labelId="start-minute-label"
                                value={startMinute}
                                label="Min"
                                onChange={(e) => {
                                    setStartMinute(e.target.value);
                                    setError("");
                                }}
                                sx={{ minWidth: 80 }}
                            >
                                <MenuItem value="00">00</MenuItem>
                                <MenuItem value="30">30</MenuItem>
                            </Select>
                        </FormControl>
                    </Box>
                </Box>

                {/* Endzeit */}
                <Box mb={2}>
                    <Typography variant="caption" color="text.secondary">
                        Endzeit
                    </Typography>
                    <Box display="flex" alignItems="center" gap={2} mt={0.5}>
                        <FormControl size="small">
                            <InputLabel id="end-hour-label">Std</InputLabel>
                            <Select
                                labelId="end-hour-label"
                                value={String(endHour)}
                                label="Std"
                                onChange={(e) => {
                                    setEndHour(parseInt(e.target.value, 10));
                                    setError("");
                                }}
                                sx={{ minWidth: 80 }}
                            >
                                {Array.from(
                                    { length: MAX_HOUR - startDate.getHours() },
                                    (_, i) => startDate.getHours() + 1 + i
                                ).map((h) => (
                                    <MenuItem key={h} value={String(h)}>
                                        {pad2(h)}
                                    </MenuItem>
                                ))}
                            </Select>
                        </FormControl>

                        <FormControl size="small">
                            <InputLabel id="end-minute-label">Min</InputLabel>
                            <Select
                                labelId="end-minute-label"
                                value={endMinute}
                                label="Min"
                                onChange={(e) => {
                                    setEndMinute(e.target.value);
                                    setError("");
                                }}
                                sx={{ minWidth: 80 }}
                            >
                                <MenuItem value="00">00</MenuItem>
                                <MenuItem value="30">30</MenuItem>
                            </Select>
                        </FormControl>
                    </Box>
                </Box>

                {error && (
                    <Typography
                        variant="caption"
                        color="error"
                        fontWeight={500}
                        mt={1}
                        display="block"
                    >
                        {error}
                    </Typography>
                )}
            </DialogContent>

            <DialogActions>
                <Button variant="outlined" onClick={onClose}>
                    Abbrechen
                </Button>

                {isEdit && (
                    <Button
                        variant="outlined"
                        color="error"
                        onClick={handleDelete}
                    >
                        Löschen
                    </Button>
                )}

                <Button variant="contained" onClick={handleSave}>
                    {isEdit ? "Speichern" : "Buchen"}
                </Button>
            </DialogActions>
        </Dialog>
    );
}
