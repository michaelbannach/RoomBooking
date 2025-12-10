import React, { useState } from "react";
import { useNavigate } from "react-router-dom";

const API_BASE = import.meta.env.VITE_API_BASE ?? "http://localhost:5135";

export default function LoginPage() {
    const navigate = useNavigate();
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [message, setMessage] = useState("");
    const [isSubmitting, setIsSubmitting] = useState(false);

    const handleLogin = async (e) => {
        e.preventDefault();
        setMessage("");
        setIsSubmitting(true);

        try {
            const resp = await fetch(`${API_BASE}/api/auth/login`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ email, password }),
            });

            if (!resp.ok) {
                let msg = "Login fehlgeschlagen";
                try {
                    const data = await resp.json();
                    msg = data.error ?? msg;
                } catch {
                    const text = await resp.text();
                    if (text) msg = text;
                }
                throw new Error(msg);
            }

            const data = await resp.json(); // { token }
            const token = data.token;
            
            localStorage.setItem("jwt", token);

            navigate("/");
        } catch (err) {
            setMessage(err.message ?? "Unbekannter Fehler");
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <div className="auth-page">
            <div className="auth-card">
                <h1>Login</h1>

                <form onSubmit={handleLogin} className="auth-form">
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
                        />
                    </label>

                    <button
                        type="submit"
                        className="auth-button"
                        disabled={isSubmitting}
                    >
                        {isSubmitting ? "Wird gesendet…" : "Einloggen"}
                    </button>
                </form>

                {message && <div className="auth-message">{message}</div>}

                <div className="auth-footer">
                    Noch kein Konto?{" "}
                    <button
                        type="button"
                        className="link-button"
                        onClick={() => navigate("/register")}
                    >
                        Registrieren
                    </button>
                </div>
            </div>
        </div>
    );
}
