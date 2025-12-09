export default function DateNavigator({ formattedDate, onPrevDay, onNextDay }) {
    return (
        <div className="mb-3 flex items-center justify-center gap-4">
            <button
                className="px-3 py-1 rounded border border-slate-300 bg-white text-sm hover:bg-slate-50"
                onClick={onPrevDay}
            >
                &lt;
            </button>

            <h2 className="text-lg font-semibold text-slate-800">
                {formattedDate}
            </h2>

            <button
                className="px-3 py-1 rounded border border-slate-300 bg-white text-sm hover:bg-slate-50"
                onClick={onNextDay}
            >
                &gt;
            </button>
        </div>
    );
}
