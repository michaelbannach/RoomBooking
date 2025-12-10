// src/App.jsx
import { BrowserRouter, Routes, Route } from "react-router-dom";
import { useRef, useState } from "react";
import Navbar from "./components/Navbar";
import CalendarPage from "./pages/CalendarPage";
import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";

export default function App() {
    const [currentView, setCurrentView] = useState("resourceTimeGridDay");
    const [currentDate, setCurrentDate] = useState(new Date());
    const calendarRef = useRef(null);

    const handleChangeView = (view) => {
        let nextDate = currentDate;

        if (view === "resourceTimeGridWeek") {
            // beim Wechsel in Wochenansicht: auf Montag der aktuellen Woche
            const d = new Date(currentDate);
            const day = (d.getDay() + 6) % 7; // Montag = 0
            d.setDate(d.getDate() - day);
            nextDate = d;
        }

        setCurrentView(view);
        setCurrentDate(nextDate);

        const api = calendarRef.current?.getApi?.();
        if (api) {
            api.changeView(view);
            api.gotoDate(nextDate);
        }
    };

    return (
        <BrowserRouter>
            <div className="app-root">
                <Navbar currentView={currentView} onChangeView={handleChangeView} />

                <main className="app-main">
                    <Routes>
                        <Route
                            path="/"
                            element={
                                <CalendarPage
                                    currentView={currentView}
                                    currentDate={currentDate}
                                    setCurrentDate={setCurrentDate}
                                    calendarRef={calendarRef}
                                    // optional: currentDate per Prop runterreichen,
                                    // falls du den State ganz nach oben ziehen möchtest
                                />
                            }
                        />
                        <Route path="/login" element={<LoginPage />} />
                        <Route path="/register" element={<RegisterPage />} />
                    </Routes>
                </main>
            </div>
        </BrowserRouter>
    );
}
