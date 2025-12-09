// src/components/BookingModal.jsx

function formatTimeRange(start, end) {
    if (!start || !end) return "";
    const fmt = new Intl.DateTimeFormat("de-DE", {
        hour: "2-digit",
        minute: "2-digit",
    });
    return `${fmt.format(start)} – ${fmt.format(end)}`;
}

export default function BookingModal({ event, roomName, onClose, onConfirm }) {
    if (!event) return null;

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
            <div className="bg-white rounded-lg shadow-lg max-w-sm w-full p-4">
                <h3 className="text-lg font-semibold mb-2">Buchung</h3>
                <p className="text-sm text-slate-700 mb-1">Raum: {roomName}</p>
                <p className="text-sm text-slate-700 mb-4">
                    Zeit: {formatTimeRange(event.start, event.end)}
                </p>

                {/* Platzhalter – hier später Formularfelder etc. */}
                <div className="flex justify-end gap-2 mt-2">
                    <button
                        className="px-3 py-1 rounded border border-slate-300 bg-white text-sm hover:bg-slate-50"
                        onClick={onClose}
                    >
                        Abbrechen
                    </button>
                    <button
                        className="px-3 py-1 rounded bg-blue-600 text-white text-sm hover:bg-blue-700"
                        onClick={onConfirm}
                    >
                        Buchen
                    </button>
                </div>
            </div>
        </div>
    );
}
