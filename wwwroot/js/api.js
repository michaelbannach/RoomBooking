export async function fetchBookingsByDate(dateStr) {
    const res = await fetch(`/api/booking/date/${dateStr}`);
    if (!res.ok) throw new Error(await res.text());
    return await res.json();
}

export async function saveBooking(booking) {
    const res = await fetch("/api/booking", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(booking)
    });
    if (!res.ok) throw new Error(await res.text());
}

export async function deleteBooking(id) {
    const res = await fetch(`/api/booking/${id}`, { method: "DELETE" });
    if (!res.ok) throw new Error(await res.text());
}
