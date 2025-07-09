export function toDateStringLocal(date) {
    return date.getFullYear() + "-" +
        (date.getMonth() + 1).toString().padStart(2, "0") + "-" +
        date.getDate().toString().padStart(2, "0");
}

export function isPastSlot(dateStr, hour) {
    const now = new Date();
    const todayStr = toDateStringLocal(now);
    const [h, m] = hour.split(":");
    const slotTime = new Date(`${dateStr}T${h.padStart(2, "0")}:${m}:00`);
    return dateStr < todayStr || (dateStr === todayStr && slotTime < now);
}