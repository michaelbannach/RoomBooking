export default function Navbar() {
    return (
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
    );
}
