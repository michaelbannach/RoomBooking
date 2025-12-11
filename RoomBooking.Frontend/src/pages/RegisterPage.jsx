import React, { useState } from "react";
import { useNavigate } from "react-router-dom";

const API_BASE = import.meta.env.VITE_API_BASE ?? "http://localhost:5135";

export default function RegisterPage() {
    const navigate = useNavigate();

    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    
    const [firstName, setFirstName] = useState("");
    const [lastName, setLastName] = useState("");
    
    
    const [message, setMessage] = useState("");
    const [isSubmitting, setIsSubmitting] = useState(false);

    const handleRegister = async (e) => {
        e.preventDefault();
        setMessage("");
        setIsSubmitting(true);

        

        try {
            const resp = await fetch(`${API_BASE}/api/auth/register`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    email,
                    password,
                    firstName,
                    lastName,
                    
                }),
            });

            if (!resp.ok) {
                let msg = "Registrierung fehlgeschlagen";
                try {
                    const data = await resp.json();
                    msg = data.error ?? msg;
                } catch {
                    const text = await resp.text();
                    if (text) msg = text;
                }
                throw new Error(msg);
            }

            setMessage("Benutzer erfolgreich registriert. Weiterleitung zum Login …");
            setTimeout(() => navigate("/login"), 1000);
        } catch (err) {
            setMessage(err.message ?? "Unbekannter Fehler");
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <div className="auth-page">
            <div className="auth-card">
                <h1>Registrieren</h1>

                <form onSubmit={handleRegister} className="auth-form">
                    <label>
                        Vorname
                        <input
                            type="text"
                            value={firstName}
                            onChange={(e) => setFirstName(e.target.value)}
                            required
                        />
                    </label>

                    <label>
                        Nachname
                        <input
                            type="text"
                            value={lastName}
                            onChange={(e) => setLastName(e.target.value)}
                            required
                        />
                    </label>

                    <label>
                        E-Mail
                        <input
                            type="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            required
                        />
                    </label>

                    <label>
                        Passwort
                        <input
                            type="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            required
                            minLength={6}
                        />
                    </label>

                  
                    <button
                        type="submit"
                        className="auth-button"
                        disabled={isSubmitting}
                    >
                        {isSubmitting ? "Wird gesendet…" : "Registrieren"}
                    </button>
                </form>

                {message && <div className="auth-message">{message}</div>}

                <div className="auth-footer">
                    Bereits ein Konto?{" "}
                    <button
                        type="button"
                        className="link-button"
                        onClick={() => navigate("/login")}
                    >
                        Zum Login
                    </button>
                </div>
            </div>
        </div>
    );
}
